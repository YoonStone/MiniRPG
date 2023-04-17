﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartUIManager : MonoBehaviour
{
    public Animator anim;
    public TMP_InputField inputNickname;

    DataManager dataManager;

    bool isStart;

    private void Start()
    {
        dataManager = DataManager.instance;
    }

    // 닉네임 입력 종료
    public void OnEndEditNickname()
    {
        if (inputNickname.text == "" || isStart) return;
        //anim.SetTrigger("start");

        string nickname = inputNickname.text;

        // 닉네임 설정
        dataManager.data.nickname = nickname;

        // 이미 있는 닉네임이라면
        if (dataManager.IsExistNickname(nickname))
        {
            Debug.Log("이미 존재하는 닉네임입니다.");
            // 이어서 할건지 물어보기
            LoadPlay();
        }
        else
        {
            Debug.Log("새로운 닉네임입니다.");
            // 새로 할건지 물어보기
            FirstPlay();
        }
    }

    // 새로 플레이
    void FirstPlay()
    {

        dataManager.isLoad = false;
    }

    void LoadPlay()
    {
        dataManager.isLoad = true;
    }

    // 시작 버튼
    public void OnClickStart()
    {
        SceneManager.LoadScene(1);
    }
}
