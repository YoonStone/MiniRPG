using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartManager : MonoBehaviour
{
    public Animator anim_Popup;
    public TextMeshProUGUI popupText;
    public TMP_InputField inputNickname;
    public Button startBtn, settingBtn, exitBtn;

    DataManager dataManager;

    public enum PopupState
    {
        None,
        Ok,
        No
    }

    public PopupState popupState = PopupState.None;

    private void Start()
    {
        dataManager = DataManager.instance;
    }

    // 시작 버튼
    public void OnClickStartBtn()
    {
        StartCoroutine(OnClickStart());
    }

    // 설정 버튼
    public void OnClickSettingBtn()
    {
        AudioManager.instance.OpenSetting();
    }

    // 종료 버튼
    public void OnClickExitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 시작 버튼 누른 후 할 일
    public IEnumerator OnClickStart()
    {
        if (inputNickname.text == "") yield break;

        // 버튼 비활성화
        startBtn.interactable = false;
        settingBtn.interactable = false;
        exitBtn.interactable = false;

        // 이미 있는 닉네임이라면
        string nickname = inputNickname.text;
        if (dataManager.IsExistNickname(nickname))
        {
            dataManager.isLoad = true;
            popupText.text = "이미 존재하는 이름입니다.\n이어서 플레이하시겠습니까?";
        }
        else
        {
            dataManager.isLoad = false;
            popupText.text = "새로운 닉네임입니다.\n처음부터 플레이하시겠습니까?";
        }

        // 팝업창 열리기
        anim_Popup.SetTrigger("Open");
        AudioManager.instance.AudioCtrl_SFX(SFX.EffectDown);

        // 버튼 선택할 때까지 기다리기
        yield return new WaitUntil(() => popupState != PopupState.None);

        switch (popupState)
        {
            // 오케이 누름 > 게임 시작
            case PopupState.Ok:

                // 닉네임 설정
                dataManager.data.nickname = nickname;

                // 불러오기 했더니 저장되어있던 씬이 던전인 경우 바로 던전으로 시작
                if (dataManager.isLoad && dataManager.Load() == 2) 
                    SceneManager.LoadScene(2);
            
                // 처음 플레이하거나 저장되어있던 씬이 마을인 경우
                else SceneManager.LoadScene(1);

                break;

            // 오케이 안 누름 > 버튼 누를 수 있게
            case PopupState.No:
                startBtn.interactable = true;
                settingBtn.interactable = true;
                exitBtn.interactable = true;
                popupState = PopupState.None;
                break;
        }
    }

    // 팝업창의 버튼
    public void OnClickPopupBtn(bool isOk)
    {
        // 팝업창을 사용한 오브젝트에게 어떤 버튼을 선택했는지 알려주기
        popupState = isOk ? PopupState.Ok : PopupState.No;
        anim_Popup.SetTrigger("Close");
        AudioManager.instance.AudioCtrl_SFX(SFX.EffectUp);
    }
}
