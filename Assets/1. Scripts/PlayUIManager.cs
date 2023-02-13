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

    [Header("-- 버튼 -- ")]
    public Button playerActionBtn; // 플레이어 액션 버튼

    //static public UIManager instance;
    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else if (instance != this) Destroy(gameObject);
    //}

    private void Start()
    {
        // 머리 위 닉네임 설정
        FindObjectOfType<PlayerAction>().GetComponentInChildren<TextMeshPro>().text = $"[ {DataManager.instance.data.nickname} ]";
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

}
