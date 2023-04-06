using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 퀘스트 상태
public enum QuestState
{
    None,    // 퀘스트 없음
    Accept,  // 퀘스트 진행 중
    Complete // 퀘스트 조건 달성
}

[System.Serializable]
public class Data
{
    public string nickname;
    public float hp = 90;
    public float exp = 0;
    public int level = 1;
    public int gold = 100;
    public float atk; // 현재 공격력
    public float def = 1; // 현재 방패 내구력

    public QuestState questState = QuestState.None;
    public int chatNum = 1;
    public int questNum = 1;
}

public class DataManager : MonoBehaviour
{
    public Data data;
    public List<Dictionary<string, object>> chatList;
    public List<Dictionary<string, object>> questList;

    // 싱글톤
    static public DataManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            chatList = CSVReader.Read("ChatList");
            questList = CSVReader.Read("QuestList");
        }
        else if (instance != this) Destroy(gameObject);
    }

    // 같은 이름의 닉네임 저장 파일이 있는지 확인
    public bool IsExistNickname(string nickname)
    {
        string path = Application.persistentDataPath + $"/{nickname}.json";
        return File.Exists(path);
    }

    // 저장
    public void Save()
    {
        string path = Application.persistentDataPath + $"/{data.nickname}.json";
        string saveData = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, saveData);
    }

    // 불러오기
    public void Load()
    {
        string path = Application.persistentDataPath + $"/{data.nickname}.json";
        string loadData = File.ReadAllText(path);

        data = JsonUtility.FromJson<Data>(loadData);
    }

    // 게임 종료 시 자동 저장
    private void OnApplicationQuit()
    {
        Save();
    }
}
