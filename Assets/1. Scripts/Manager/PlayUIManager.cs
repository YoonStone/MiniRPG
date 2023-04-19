using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

[System.Serializable]
public struct Skill
{
    public Button skillBtn;
    public Image skillCool;
    public float skillAtk;
    public float skillCoolTime;
}

// 팝업창 상태
public enum PopupState
{
    None, // 팝업창에 답변 없음
    Left, // 팝업창 왼쪽 버튼 클릭
    Right // 팝업창 오른쪽 버튼 클릭
}

public class PlayUIManager : MonoBehaviour
{
    [Header("-- UI 애니메이터 -- ")]
    public Animator anim_Setting;
    public Animator anim_Inventory;
    public Animator anim_Popup;
    public Animator anim_Chat;
    public Animator anim_Shop;

    [Header("-- 버튼 -- ")]
    public Button playerActionBtn;   // 플레이어 액션 버튼
    public Skill[] skills;           // 공격 스킬 배열
    public GameObject chatNextBtn;   // 말풍선 다음 버튼
    public GameObject chatCancleBtn; // 말풍선 종료 버튼

    [Header("-- 텍스트 -- ")]
    public TextMeshProUGUI levelTxt;
    public TextMeshProUGUI goldTxt;
    public TextMeshProUGUI popupTxt;
    public TextMeshProUGUI popupBtn1Txt;
    public TextMeshProUGUI popupBtn2Txt;
    public TextMeshProUGUI chatTxt;

    [Header("-- 이미지 -- ")]
    public Image hpImg;
    public Image expImg;
    public Image defImg;

    [Header("-- Fade -- ")]
    public Transform fadeImg; // 페이드인,페이드아웃

    #region [프로퍼티]
    private float maxHp = 100;
    public float Hp // 플레이어 체력
    {
        get { return dm.data.hp; }
        set
        {
            dm.data.hp = Mathf.Clamp(value, 0, maxHp);
            hpImg.fillAmount = dm.data.hp / maxHp;
        }
    }

    public float Exp // 경험치
    {
        get { return dm.data.exp; }
        set
        {
            StartCoroutine(IncreaseExp(dm.data.exp, value));
            dm.data.exp = value;
        }
    }

    public int Level // 레벨
    {
        get { return dm.data.level; }
        set
        {
            dm.data.level = value;
            levelTxt.text = value.ToString();
        }
    }

    public int Gold // 골드 (화폐)
    {
        get { return dm.data.gold; }
        set
        {
            dm.data.gold = value;
            goldTxt.text = value.ToString() + " G";
        }
    }

    public float Def // 방패 내구력
    {
        get { return dm.data.def; }
        set
        {
            dm.data.def = value;
            value = Mathf.Clamp(value, 0, 1);
            defImg.fillAmount = value;

            if(value == 0)
            {
                // 방패 아이템 삭제
                Slot shieldSlot = InventoryManager.instance.FindItemSlot(2);
                shieldSlot.Count--;

                // 방패가 없다면 투명하게
                if(shieldSlot.Count <= 0)
                {
                    defImg.color = Color.clear;
                    playerAction.shield.SetActive(false);
                }

                dm.data.def = 1;
            }
        }
    }
    #endregion

    [HideInInspector] public PlayerAction playerAction;
    [HideInInspector] public PlayerMove playerMove;
    CameraTurn cameraTurn;
    DataManager dm;

    static public PlayUIManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        playerAction = FindObjectOfType<PlayerAction>();
        playerMove = playerAction.GetComponent<PlayerMove>();
        cameraTurn = FindObjectOfType<CameraTurn>();
        dm = DataManager.instance;

        // 머리 위 닉네임 설정
        playerAction.GetComponentInChildren<TextMeshPro>().text = $"[ {dm.data.nickname} ]";

        if (dm.isLoad) dm.Load();
        else
        {
            dm.Save();
            InventoryManager.instance.AddItem(0); // 칼 지급
        }

        // 저장되어있던 체력 불러오기 (저장할 때 체력할 것*)
        Hp = dm.data.hp;
    }

    // 설정 버튼
    public void OnClickSettingBtn(bool isOpen)
    {
        anim_Setting.SetBool("isOpen", isOpen);
    }

    // 인벤토리 버튼
    public void OnClickInventoryBtn(bool isOpen)
    {
        anim_Inventory.SetBool("isOpen", isOpen);
    }

    #region [팝업창]
    [HideInInspector]
    public bool isPopup;
    public PopupState popupState;

    // 팝업창 열기
    public bool PopupOpen(string message, string btn1, string btn2)
    {
        // 이미 팝업창이 열려있다면 실행 금지
        if (isPopup) return false;

        isPopup = true;
        popupTxt.text = message;
        popupBtn1Txt.text = btn1;
        popupBtn2Txt.text = btn2;
        anim_Popup.SetTrigger("Open");

        cameraTurn.enabled = false;

        return true;
    }

    // 팝업창의 버튼
    public void OnClickPopupBtn(bool isLeft)
    {
        // 팝업창을 사용한 오브젝트에게 어떤 버튼을 선택했는지 알려주기
        popupState = isLeft ? PopupState.Left : PopupState.Right;
        anim_Popup.SetTrigger("Close");
        isPopup = false;
        cameraTurn.enabled = true;
    }


    // 팝업창 사용 (퀘스트)
    IEnumerator PopupCall_Quest()
    {
        bool isCanPopup = PopupOpen("퀘스트를 수락하시겠습니까?", "예", "아니오");

        // 이미 팝업창이 열려있었다면 실행 금지
        if (!isCanPopup) yield break;

        // 예/아니오를 누를 때까지 기다리기
        popupState = PopupState.None;
        yield return new WaitUntil(() => popupState != PopupState.None);

        // 퀘스트 수락
        if (popupState == PopupState.Left)
        {
            dm.data.questState = QuestState.Accept;
            playerAction.withNpc.SendMessage("QuestStart", dm.questList[dm.data.questNum]["QuestName"].ToString());
        }

        OnClickChatCancle();
        popupState = PopupState.None;
    }
    #endregion

    #region [대화창]

    // 지금 열릴 말풍선이 대화인지 퀘스트인지
    public void CheckBubble()
    {
        // 액션 버튼, 이동 비활성화
        playerActionBtn.interactable = false;
        playerMove.isCantMove = true;

        // 다음에 퀘스트가 나올 차례
        if (dm.chatList[dm.data.chatNum]["NPC"].ToString() == "")
            QuestBubbleOpen();

        // 다음에도 대사가 나올 차례
        else ChatBubbleOpen();
    }

    // 채팅 대화창 열기
    public void ChatBubbleOpen()
    {
        chatTxt.text = dm.chatList[dm.data.chatNum]["Script"].ToString();
        anim_Chat.SetTrigger("Open");

        // 다음 대화가 있다면
        if (dm.data.chatNum + 1 < dm.chatList.Count)
        {
            // 다음 대화가 퀘스트거나 대사의 주인이 현재 대화 중인 NPC라면
            string nextNpc = dm.chatList[dm.data.chatNum + 1]["NPC"].ToString();

            if(nextNpc == "" || (nextNpc != "" && nextNpc == playerAction.withNpc.npcName))
            {
                chatNextBtn.SetActive(true);
                chatCancleBtn.SetActive(true);
                return;
            }
        }

        // 다음 대사가 없거나 다른 NPC의 것이라면
        chatNextBtn.SetActive(false);
        chatCancleBtn.SetActive(true);
    }

    // 퀘스트 대화창 열기
    public void QuestBubbleOpen()
    {
        switch (dm.data.questState)
        {
            // 퀘스트 받기 전
            case QuestState.None:
                chatTxt.text = dm.questList[dm.data.questNum]["Request"].ToString();
                chatNextBtn.SetActive(false);
                chatCancleBtn.SetActive(false);
                StartCoroutine(PopupCall_Quest());
                break;

            // 퀘스트 완료
            case QuestState.Complete:
                chatTxt.text = dm.questList[dm.data.questNum]["Complete"].ToString();
                dm.data.questState = QuestState.None;
                dm.data.questNum++;

                // 다음 대화가 있고, 대사의 주인이 현재 대화 중인 NPC라면
                if (dm.data.chatNum + 1 < dm.chatList.Count
                    && dm.chatList[dm.data.chatNum + 1]["NPC"].ToString() == playerAction.withNpc.npcName)
                {
                    chatNextBtn.SetActive(true);
                    chatCancleBtn.SetActive(true);
                }
                // 다음 대사가 없거나 다른 NPC의 것이라면
                else
                {
                    chatNextBtn.SetActive(false);
                    chatCancleBtn.SetActive(true);
                    dm.data.chatNum++;
                }
                break;
        }
        anim_Chat.SetTrigger("Open");
    }

    // 퀘스트 완료창 열었다가 닫기
    public IEnumerator QuestCompleteOpen(string npcName)
    {
        chatTxt.text = $"퀘스트를 완료했습니다!\nNPC에게 돌아가세요\n({npcName})";
        chatNextBtn.SetActive(false);
        chatCancleBtn.SetActive(false);
        anim_Chat.SetTrigger("Open");

        yield return new WaitForSeconds(1.5f);

        //anim_Chat.SetTrigger("Close");
    }

    // 대화창 끄기
    public void OnClickChatCancle()
    {
        anim_Chat.SetTrigger("Close");
        playerActionBtn.interactable = true;
        playerMove.isCantMove = false;
    }

    // 대화창 다음
    public void OnClickChatNext()
    {
        dm.data.chatNum++;
        CheckBubble();
    }
    #endregion
    
    // 페이드인, 페이드아웃
    public IEnumerator Fade(Vector3 fromScale, Vector3 toScale)
    {
        float time = 0;
        while(time < 1)
        {
            time += Time.deltaTime;
            fadeImg.localScale = Vector3.Lerp(fromScale, toScale, time);
            yield return null;
        }

        if(toScale == Vector3.one) LoadingManager.LoadScene(2);
    }

    // 체력바 이미지 색상 변경
    public IEnumerator HpImgColor(Color color)
    {
        hpImg.color = color;
        yield return new WaitForSeconds(0.5f);
        hpImg.color = Color.white;
    }

    // 스킬 쿨타임
    public IEnumerator SkiilCoolTime(int skillNumber)
    {
        skills[skillNumber].skillBtn.interactable = false;
        skills[skillNumber].skillCool.gameObject.SetActive(true);

        // 쿨타임 이미지
        Image coolImg = skills[skillNumber].skillCool;
        float coolTime = skills[skillNumber].skillCoolTime;

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime / coolTime;
            coolImg.fillAmount = Mathf.Lerp(1, 0, time);
            yield return null;
        }

        skills[skillNumber].skillCool.gameObject.SetActive(false);
        skills[skillNumber].skillBtn.interactable = true;
    }


    // 경험치 증가 효과
    IEnumerator IncreaseExp(float from, float to)
    {
        float timer = 0;
        while (timer < 0.5f)
        {
            expImg.fillAmount = Mathf.Lerp(from, to, timer * 2);
            timer += Time.deltaTime;
            yield return null;
        }

        if (expImg.fillAmount >= 1)
        {
            Exp = 0;
            Level++;
        }
    }
}
