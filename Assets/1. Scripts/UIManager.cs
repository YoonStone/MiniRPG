using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("-- UI 애니메이터 -- ")]
    public Animator anim_Setting;
    public Animator anim_Inventory;

    public Button playerActionBtn;

    static public UIManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) Destroy(gameObject);
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
