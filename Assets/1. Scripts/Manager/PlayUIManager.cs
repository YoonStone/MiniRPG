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
    public Image questOKImg; // 퀘스트 수락 이미지

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

    private bool isChatOK; // 퀘스트 수락인지
    public bool IsChatOK
    {
        get { return isChatOK; }
        set
        {
            isChatOK = value;
            questOKImg.color = isChatOK ? Color.yellow : Color.white;
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
        Hp = maxHp;
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
    GameObject popupFrom; // 팝업창을 사용한 오브젝트

    // 팝업창 열기
    public void PopupOpen(GameObject from, string message, string btn1, string btn2)
    {
        isPopup = true;
        popupFrom = from;
        popupTxt.text = message;
        popupBtn1Txt.text = btn1;
        popupBtn2Txt.text = btn2;
        anim_Popup.SetTrigger("Open");
    }

    // 팝업창의 버튼
    public void OnClickPopupBtn(bool isChoice1)
    {
        // 팝업창을 사용한 오브젝트에게 어떤 버튼을 선택했는지 알려주기
        popupFrom.SendMessage("PopupCallback", isChoice1);
        anim_Popup.SetTrigger("Close");
        isPopup = false;
    }

    // 대화창 열기
    public void ChatBubbleOpen()
    {
        DataManager dm = DataManager.instance;
        chatTxt.text = dm.chatList[dm.data.chatNum]["Script"].ToString();
        anim_Chat.SetTrigger("Open");
    }

    // 대화창 끄기
    public void OnClickChatCancle()
    {
        IsChatOK = false;
        anim_Chat.SetTrigger("Close");
    }

    // 대화창 다음 or 수락
    public void OnClickChatNextOK()
    {
        if (IsChatOK) // 퀘스트 수락
        {
            OnClickChatCancle();
        }
        else // 다음 대화창
        {
            DataManager dm = DataManager.instance;
            dm.data.chatNum++;
            ChatBubbleOpen();

            // 이번 대화창이 마지막이라면 IsChatOK 트루로 변경
            // (= 다음 대화의 퀘스트번호가 다르다면)
            if (dm.chatList.Count > dm.data.chatNum + 1 &&
                int.Parse(dm.chatList[dm.data.chatNum + 1]["QuestNum"].ToString()) != dm.data.questNum)
            {
                dm.data.questNum++;
                IsChatOK = true;
            }
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
}
