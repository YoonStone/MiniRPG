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

    [Header("-- Fade -- ")]
    public Transform fadeImg; // 페이드인,페이드아웃

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
}
