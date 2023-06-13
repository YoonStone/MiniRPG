using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public Vector3 curPos;
    public Quaternion curRot;
    public int curSceneIdx; // 현재 씬 번호

    public QuestState questState = QuestState.None;
    public int chatNum = 0;
    public int questNum = 0;
    public int questItemCount = 0;

    public int[] itemSlots_Number = new int[8];
    public int[] itemSlots_Count = new int[8];
    public int[] equipSlots_Number = new int[2];
    public int[] equipSlots_Count = new int[2];

    public bool[] skillOpen = new bool[3];

    // 0 : BGM, 1 : SFX
    public float[] volumes = { 1, 1 };
}

public class DataManager : MonoBehaviour
{
    public Data data;
    public List<Dictionary<string, object>> chatList;
    public List<Dictionary<string, object>> questList;

    public bool isLoad; // 이어하기

    // 싱글톤
    static public DataManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // CSV 읽어오기
            chatList = CSVReader.Read("ChatList");
            questList = CSVReader.Read("QuestList");

            // 씬 전환 이벤트 추가
            SceneManager.sceneLoaded += OnSceneLoaded;
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
        // 위치 저장
        Transform player = FindObjectOfType<PlayerAction>().transform;
        data.curPos = player.position;
        data.curRot = player.rotation;

        // 인벤토리 저장 (아이템)
        Slot[] _itemSlots = InventoryManager.instance.itemSlots;
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            // 아이템이 없다면 -1 저장
            if (!_itemSlots[i].Item) data.itemSlots_Number[i] = -1;

            // 아이템이 있다면 아이템 번호 저장
            else
            {
                data.itemSlots_Number[i] = _itemSlots[i].Item.itemIdx;
                data.itemSlots_Count[i] = _itemSlots[i].Count;
            }
        }

        // 인벤토리 저장 (장비)
        Slot[] _equipSlots = InventoryManager.instance.equipSlots;
        for (int i = 0; i < _equipSlots.Length; i++)
        {
            // 아이템이 없다면 -1 저장
            if (!_equipSlots[i].Item) data.equipSlots_Number[i] = -1;

            // 아이템이 있다면 아이템 번호 저장
            else
            {
                data.equipSlots_Number[i] = _equipSlots[i].Item.itemIdx;
                data.equipSlots_Count[i] = _equipSlots[i].Count;
            }
        }

        string path = Application.persistentDataPath + $"/{data.nickname}.json";
        string saveData = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, saveData);
    }

    // 불러오기
    public int Load()
    {
        string path = Application.persistentDataPath + $"/{data.nickname}.json";
        if(!File.Exists(path)) return -1;

        string loadData = File.ReadAllText(path);

        data = JsonUtility.FromJson<Data>(loadData);

        // 마지막에 위치했던 씬 번호 반환
        return data.curSceneIdx;
    }

    // 불러오기한 내용 적용
    public void AfterLoad(GameManager gm)
    {
        // UI 표시하기
        gm.Hp = data.hp;
        gm.Exp = data.exp; 
        gm.Level = data.level;
        gm.Gold = data.gold;
        gm.Def = data.def;

        PlayerAction player = FindObjectOfType<PlayerAction>();

        // 위치 불러오기
        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        cc.transform.position = data.curPos;
        cc.transform.rotation = data.curRot;
        cc.enabled = true;

        // 인벤토리 불러오기
        InventoryManager inventory = InventoryManager.instance;
        for (int i = 0; i < data.itemSlots_Number.Length; i++)
        {
            // -1이 아닌 수가 저장되어있다면 해당 번호의 아이템 가져다가 넣어주기
            if (data.itemSlots_Number[i] != -1)
            {
                inventory.itemSlots[i].Item = inventory.items[data.itemSlots_Number[i]];
                inventory.itemSlots[i].Count = data.itemSlots_Count[i];
            }
        }
        for (int i = 0; i < data.equipSlots_Number.Length; i++)
        {
            // -1이 아닌 수가 저장되어있다면 해당 번호의 아이템 가져다가 넣어주기
            if (data.equipSlots_Number[i] != -1)
            {
                inventory.equipSlots[i].Item = inventory.items[data.equipSlots_Number[i]];
                inventory.equipSlots[i].Count = data.equipSlots_Count[i];
                player.EquipPutOn(data.equipSlots_Number[i]);

            }
        }

        // 사운드 불러오기
        for (int i = 0; i < 2; i++)
        {
            AudioManager.instance.SetVolume(i, data.volumes[i]);
        }
    }

    // 게임 종료 시 자동 저장
    private void OnApplicationQuit()
    {
        // 시작 화면이 아닌 상태로 끄면 자동 저장 
        if(SceneManager.GetActiveScene().buildIndex != 0) Save();
    }


    // 씬 전환 완료 시 실행할 이벤트
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameManager gm = GameManager.instance;

        // 시작 화면으로 갈 때는 삭제
        if (gm && scene.buildIndex == 0)
        {
            // 던전에서 넘어가는 경우에는 배경음악 교체
            if (data.curSceneIdx == 2) AudioManager.instance.AudioCtrl_BGM(false);
            Destroy(GameManager.instance.gameObject);
            return;
        }

        // 전환 후 불러오기
        Load();

        // 위치 이동 기능
        switch (scene.buildIndex)
        {
            // 마을
            case 1:
                AudioManager.instance.audios[1].clip = AudioManager.instance.clip_Walk[0];

                // 죽었다가 부활한 경우
                if (gm.Hp == 0)
                {
                    gm.playerAction.ResetAll();

                    // 페이드 기능
                    StartCoroutine(gm.SceneFade(Vector3.one, Vector3.zero, -1));

                    if (data.curSceneIdx == 2) AudioManager.instance.AudioCtrl_BGM(false);
                }

                // 던전 > 마을
                else if (data.curSceneIdx == 2)
                {
                    AudioManager.instance.AudioCtrl_BGM(false);

                    CharacterController cc = FindObjectOfType<CharacterController>();
                    gm.playerCC.enabled = false;
                    gm.playerCC.transform.SetPositionAndRotation(new Vector3(-18, 3.45f, -17), Quaternion.Euler(0, 145, 0));
                    gm.playerCC.enabled = true;

                    // 페이드 기능
                    StartCoroutine(gm.SceneFade(Vector3.one, Vector3.zero, -1));
                }
                break;

            // 던전
            case 2:
                AudioManager.instance.AudioCtrl_BGM(true);
                AudioManager.instance.audios[1].clip = AudioManager.instance.clip_Walk[1];

                // 마을 > 던전
                if (data.curSceneIdx == 1)
                {
                    CharacterController cc = FindObjectOfType<CharacterController>();
                    gm.playerCC.enabled = false;
                    gm.playerCC.transform.SetPositionAndRotation(new Vector3(3, 0, 0), Quaternion.Euler(0, 180, 0));
                    gm.playerCC.enabled = true;

                    // 페이드 기능
                    StartCoroutine(gm.SceneFade(Vector3.one, Vector3.zero, -1));
                }
                break;
        }

        // 전환된 씬 번호로 저장
        data.curSceneIdx = scene.buildIndex;
    }
}
