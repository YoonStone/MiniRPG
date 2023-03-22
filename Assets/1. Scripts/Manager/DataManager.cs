﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Data
{
    public string nickname;
    public int questNum = 1;
    public int chatNum = 0;
}

public class DataManager : MonoBehaviour
{
    public Data data;
    public List<Dictionary<string, object>> chatList;

    // 싱글톤
    static public DataManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            chatList = CSVReader.Read("ChatList");
        }
        else if (instance != this) Destroy(gameObject);
    }

    // 같은 이름의 닉네임 저장 파일이 있는지 확인
    public bool IsExistNickname(string nickname)
    {
        string path = Application.persistentDataPath + $"/{nickname}.json";
        return File.Exists(path);
    }
}
