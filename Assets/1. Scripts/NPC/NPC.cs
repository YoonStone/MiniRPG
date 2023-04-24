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

    public NPCInfo info;
    public GameObject onoff;
    public TextMeshProUGUI headTxt;

    public bool isInteractable; // 상호작용 가능한지

    public NPCQuestState npcQuestState;
    public NPCQuestState NPCState
    {
        get { return npcQuestState; }
        set
        {
            npcQuestState = value;

            switch (npcQuestState)
            {
                case NPCQuestState.None: headTxt.text = ""; break;
                case NPCQuestState.Have: headTxt.text = "?"; break;
                case NPCQuestState.Wait: headTxt.text
                        = dm.data.questState == QuestState.Complete
                        ? "!" : "..."; break;

            }
        }
    }

    protected DataManager dm;
    protected GameManager gm;
    protected InventoryManager inventory;
    protected PlayerAction player;
    QuestManager qm;

    void Start()
    {
        dm = DataManager.instance;
        gm = GameManager.instance;
        inventory = InventoryManager.instance;
        qm = gm.qm;
        player = FindObjectOfType<PlayerAction>();

        // NPC 번호로 정보 전달 후 상태 세팅
        qm.npcs[info.npcIndex].npcObj = this;
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

                if(info.npcName == "Merchant") gameObject.SendMessage("PlayerBye"); 
                break;
        }
    }

    // 퀘스트 시작
    void QuestStart(string questName)
    {
        NPCState = NPCQuestState.Wait;
        StartCoroutine("Quest_" + questName);
    }

    // 퀘스트 상태 재정비
    public void SetQuestState()
    {
        // 이후에 퀘스트가 없다면 끝
        if (dm.data.questNum >= dm.questList.Count)
        {
            print(info.npcName + "은/는 더이상 퀘스트가 없음");
            NPCState = NPCQuestState.None;
            return;
        }

        int fromNpc = int.Parse(dm.questList[dm.data.questNum]["FromNPC"].ToString());
        int toNpc = int.Parse(dm.questList[dm.data.questNum]["ToNPC"].ToString());


        // 퀘스트를 줄 NPC + 퀘스트 아직 안 받은 상태
        if (fromNpc == info.npcIndex && dm.data.questState == QuestState.None)
        {
            print(info.npcName + "은/는 퀘스트를 갖고 있어 대화를 기다리는 상태");
            NPCState = NPCQuestState.Have;
        }

        // 퀘스트를 완료할 NPC + 퀘스트 받은 상태
        else if (toNpc == info.npcIndex && dm.data.questState == QuestState.Accept)
        {
            print(info.npcName + "은/는 퀘스트 조건 만족을 기다리는 상태");
            NPCState = NPCQuestState.Wait;
        }

        // 퀘스트를 완료할 NPC + 퀘스트 완료한 상태
        else if (toNpc == info.npcIndex && dm.data.questState == QuestState.Complete)
        {
            print(info.npcName + "은/는 퀘스트 조건 만족하여 대화를 기다리는 상태");
            NPCState = NPCQuestState.Wait;
        }

        // 현재 퀘스트와 관련 없는 NPC라면
        else
        {
            print(info.npcName + "은/는 퀘스트 없는 상태");
            NPCState = NPCQuestState.None;
        }

        // 상호작용 가능한 상태
        if (dm.data.questNum >= info.interactableNumber) isInteractable = true;
    }
}
