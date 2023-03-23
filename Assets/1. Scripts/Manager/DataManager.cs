using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 퀘스트 상태
public enum QuestState
{
    None,    // 퀘스트 받기 전 + 퀘스트 완료, 대화 완료
    Accept,  // 퀘스트 수락, 완료 전
    Complete // 퀘스트 완료, 대화 전
}

[System.Serializable]
public class Data
{
    public string nickname;
    public float hp = 90;

    public QuestState questState = QuestState.None;
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
