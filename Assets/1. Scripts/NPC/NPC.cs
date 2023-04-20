using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class NPC : MonoBehaviour
{
    public enum NPCQuestState
    {
        None, // 갖고 있는 퀘스트가 없거나 현재 순서가 아님
        Have, // 갖고 있는 퀘스트가 현재 순서
        Wait  // 플레이어가 수락했고, 기다리는 중
    }

    public GameObject onoff;
    public TextMeshProUGUI headTxt;
    public NPCQuestState npcQuestState;
    public string npcName;
    public string koreanName;

    DataManager dm;
    protected PlayUIManager manager;
    protected InventoryManager inventory;
    protected PlayerAction player;

    //[HideInInspector]
    public bool isInteractable; // 상호작용 가능한지

    void Start()
    {
        dm = DataManager.instance;
        manager = PlayUIManager.instance;
        inventory = InventoryManager.instance;
        player = FindObjectOfType<PlayerAction>();

        npcName = name.Split('_')[1];
        koreanName = onoff.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

        SetQuestState();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Player": // player가 근처에 있을 때
                onoff.SetActive(true);
                player.withNpc = this;
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Player": // player와 헤어졌을 때
                onoff.SetActive(false);
                player.withNpc = null;

                if(npcName == "Merchant") gameObject.SendMessage("PlayerBye"); 
                break;
        }
    }

    // 퀘스트 시작
    void QuestStart(string questName)
    {
        print(questName + " 퀘스트 시작");
        npcQuestState = NPCQuestState.Wait;
        headTxt.text = "...";
        StartCoroutine("Quest_" + questName);
    }

    // 퀘스트 조건 완료
    protected void QuestComplete()
    {
        dm.data.questState = QuestState.Complete;
        npcQuestState = NPCQuestState.Wait;
        headTxt.text = "!";

        StartCoroutine(PlayUIManager.instance.QuestCompleteOpen(koreanName));
    }

    // 퀘스트 상태 재정비
    void SetQuestState()
    {
        string fromNpc = dm.questList[dm.data.questNum]["FromNPC"].ToString();
        string toNpc = dm.questList[dm.data.questNum]["ToNPC"].ToString();


        // 퀘스트를 줄 NPC + 퀘스트 아직 안 받은 상태
        if (fromNpc == npcName && dm.data.questState == QuestState.None)
        {
            print(npcName + "은/는 퀘스트를 갖고 있는 상태");
            npcQuestState = NPCQuestState.Have;
            headTxt.text = "?";
        }

        // 퀘스트를 준 NPC + 퀘스트 받은 상태
        else if (fromNpc == npcName && dm.data.questState == QuestState.Accept)
        {
            print(npcName + "은/는 퀘스트를 준 상태");
            gameObject.SendMessage("QuestStart", dm.questList[dm.data.questNum]["QuestName"].ToString());
            
        }

        // 퀘스트를 완료할 NPC + 퀘스트 받은 상태
        else if (toNpc == npcName && dm.data.questState == QuestState.Accept)
        {
            print(npcName + "은/는 퀘스트 완료를 기다리는 상태");
            gameObject.SendMessage("QuestStart", dm.questList[dm.data.questNum]["QuestName"].ToString());

        }

        // 퀘스트를 완료할 NPC + 퀘스트 완료한 상태
        else if (toNpc == npcName && dm.data.questState == QuestState.Complete)
        {
            print(npcName + "은/는 퀘스트 완료된 상태");
            npcQuestState = NPCQuestState.Have;
            headTxt.text = "!";
        }

        // 현재 퀘스트와 관련 없는 NPC라면
        else
        {
            print(npcName + "은/는 퀘스트 없는 상태");
            npcQuestState = NPCQuestState.None;
            headTxt.text = "";
        }
    }
}
