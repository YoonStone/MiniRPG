using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayUIManager : MonoBehaviour
{
    [Header("-- UI 애니메이터 -- ")]
    public Animator anim_Setting;
    public Animator anim_Inventory;
    public Animator anim_Popup;

    [Header("-- 버튼 -- ")]
    public Button playerActionBtn; // 플레이어 액션 버튼

    [Header("-- 텍스트 -- ")]
    public TextMeshProUGUI popupTxt;
    public TextMeshProUGUI popupBtn1Txt;
    public TextMeshProUGUI popupBtn2Txt;

    [Header("-- 이미지 -- ")]
    public Image hpImg;

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
            hpImg.fillAmount = hp / maxHp;
        }
    }

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
        // 머리 위 닉네임 설정
        FindObjectOfType<PlayerAction>().GetComponentInChildren<TextMeshPro>().text = $"[ {DataManager.instance.data.nickname} ]";

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

    GameObject popupFrom; // 팝업창을 사용한 오브젝트

    // 팝업창 열기
    public void PopupOpen(GameObject from, string message, string btn1, string btn2)
    {
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

    public IEnumerator GetHitEffect()
    {
        hpImg.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        hpImg.color = Color.white;
    }
}
