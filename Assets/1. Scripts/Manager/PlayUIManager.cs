using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("-- 버튼 -- ")]
    public Button playerActionBtn; // 플레이어 액션 버튼
    public Skill[] skills;         // 공격 스킬 배열

    [Header("-- 텍스트 -- ")]
    public TextMeshProUGUI popupTxt;
    public TextMeshProUGUI popupBtn1Txt;
    public TextMeshProUGUI popupBtn2Txt;
    public TextMeshProUGUI chatTxt;

    [Header("-- 이미지 -- ")]
    public Image hpImg;
    public GameObject chatBtns;

    [Header("-- Fade -- ")]
    public Transform fadeImg; // 페이드인,페이드아웃

    private float maxHp = 100;
    private float hp; // 플레이어 체력
    public float Hp
    {
        get { return hp; }
        set
        {
            hp = value;
            hp = Mathf.Clamp(hp, 0, maxHp);
            hpImg.fillAmount = hp / maxHp;
        }
    }

    [HideInInspector]
    public PlayerAction player;

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
        player = FindObjectOfType<PlayerAction>();

        // 머리 위 닉네임 설정
        player.GetComponentInChildren<TextMeshPro>().text = $"[ {DataManager.instance.data.nickname} ]";

        // 저장되어있던 체력 불러오기 (저장할 때 체력할 것*)
        Hp = DataManager.instance.data.hp;
    }

    // 설정 버튼
    public void OnClickSettingBtn(string triggerName)
    {
        anim_Setting.SetTrigger(triggerName);
    }

    // 인벤토리 버튼
    public void OnClickInventoryBtn(string triggerName)
    {
        anim_Inventory.SetTrigger(triggerName);
    }

    [HideInInspector]
    public bool isPopup;
    //GameObject popupFrom; // 팝업창을 사용한 오브젝트
    public PopupState popupState;

    // 팝업창 열기
    public void PopupOpen(string message, string btn1, string btn2)
    {
        isPopup = true;
        popupTxt.text = message;
        popupBtn1Txt.text = btn1;
        popupBtn2Txt.text = btn2;
        anim_Popup.SetTrigger("Open");
    }

    // 팝업창의 버튼
    public void OnClickPopupBtn(bool isLeft)
    {
        // 팝업창을 사용한 오브젝트에게 어떤 버튼을 선택했는지 알려주기
        popupState = isLeft ? PopupState.Left : PopupState.Right;
        anim_Popup.SetTrigger("Close");
        isPopup = false;
    }

    // 대화창 열기
    public void ChatBubbleOpen()
    {
        DataManager dm = DataManager.instance;
        chatTxt.text = dm.chatList[dm.data.chatNum]["Script"].ToString();
        chatBtns.SetActive(true);

        // 이번 대화창이 마지막이라면 (= 다음 대화의 퀘스트번호가 다르다면)
        if (dm.chatList.Count > dm.data.chatNum + 1
            && int.Parse(dm.chatList[dm.data.chatNum + 1]["QuestNum"].ToString()) != dm.data.questNum
            && dm.data.questState == QuestState.None)
        {
            chatBtns.SetActive(false);
            StartCoroutine(PopupCall_Quest());
        }

        anim_Chat.SetTrigger("Open");
    }

    // 대화창 끄기
    public void OnClickChatCancle()
    {
        anim_Chat.SetTrigger("Close");
    }

    // 대화창 다음
    public void OnClickChatNext()
    {
        DataManager dm = DataManager.instance;
        dm.data.chatNum++;
        ChatBubbleOpen();

        // 이번 대화창이 마지막이라면 (= 다음 대화의 퀘스트번호가 다르다면)
        if (dm.chatList.Count > dm.data.chatNum + 1 &&
            int.Parse(dm.chatList[dm.data.chatNum + 1]["QuestNum"].ToString()) != dm.data.questNum)
        {
            chatBtns.SetActive(false);
            StartCoroutine(PopupCall_Quest());
        }
    }

    // 페이드인, 페이드아웃
    public IEnumerator Fade(int from, int to)
    {
        Vector3 fromScale = new Vector3(from, from, from);
        Vector3 toScale = new Vector3(to, to, to);

        float time = 0;
        while(time < 1)
        {
            time += Time.deltaTime;
            fadeImg.localScale = Vector3.Lerp(fromScale, toScale, time);
            yield return null;
        }

        if(to == 1) LoadingManager.LoadScene(2);
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

    // 팝업창 사용 (퀘스트)
    IEnumerator PopupCall_Quest()
    {
        PlayUIManager.instance.popupState = PopupState.None;
        PlayUIManager.instance.PopupOpen("퀘스트를 수락하시겠습니까?", "예", "아니오");

        // 예/아니오를 누를 때까지 기다리기
        yield return new WaitUntil(() => PlayUIManager.instance.popupState != PopupState.None);
        DataManager dm = DataManager.instance;

        // 퀘스트 수락
        if (PlayUIManager.instance.popupState == PopupState.Left)
        {
            //dm.data.questNum++;
            dm.data.questState = QuestState.Accept;
            player.withNpc.SendMessage("QuestStart", dm.chatList[dm.data.chatNum]["QuestName"].ToString());
        }

        OnClickChatCancle();
        PlayUIManager.instance.popupState = PopupState.None;
    }
}
