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

    PlayerAction player;
    DataManager dm;
    protected PlayUIManager manager;
    protected InventoryManager inventory;

    [HideInInspector]
    public bool isInteractable; // 상호작용 가능한지

    bool isFirst = true; // 게임이 시작되는 순간의 퀘스트 상태 정리

    void Start()
    {
        player = FindObjectOfType<PlayerAction>();
        dm = DataManager.instance;
        manager = PlayUIManager.instance;
        inventory = InventoryManager.instance;

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
        dm.data.questState = QuestState.None;
        npcQuestState = NPCQuestState.Wait;
        headTxt.text = "!";

        StartCoroutine(PlayUIManager.instance.QuestCompleteOpen(koreanName));
    }

    // 퀘스트 상태 재정비
    void SetQuestState()
    {
        // 현재 퀘스트를 진행해야 하는 npc가 아니라면
        if (dm.chatList[dm.data.chatNum]["NPCName"].ToString() != npcName)
        {
            print(npcName + "은/는 퀘스트 없는 상태");
            npcQuestState = NPCQuestState.None;
            headTxt.text = "";
        }
        // 현재 퀘스트를 진행해야 하는 npc이고,
        else
        {
            print(npcName + "은/는 퀘스트 있는 상태");
            switch (dm.data.questState)
            {
                case QuestState.None: // 퀘스트 수락 전
                    npcQuestState = NPCQuestState.Have;
                    headTxt.text = "?"; break;

                case QuestState.Accept: // 퀘스트 수락한 상태 (수행 중)
                    npcQuestState = NPCQuestState.Wait;
                    headTxt.text = "...";
                    gameObject.SendMessage("QuestStart", dm.chatList[dm.data.chatNum]["QuestName"].ToString());
                    break;
            }
        }

        isFirst = false;
    }
}
