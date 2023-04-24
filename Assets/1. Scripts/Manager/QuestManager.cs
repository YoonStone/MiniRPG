using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NPC;

[System.Serializable]
public struct NPCSet
{
    public NPCInfo npcInfo;
    public NPC npcObj;
}

public class QuestManager : MonoBehaviour
{
    public delegate IEnumerator QuestList();
    public QuestList[] questList;
    public NPCSet[] npcs = new NPCSet[3];

    GameManager gm;
    DataManager dm;
    InventoryManager inventory;

    void Start()
    {
        gm = GameManager.instance;
        dm = DataManager.instance;
        inventory = InventoryManager.instance;

        questList = new QuestList[dm.questList.Count];
        questList[0] = new QuestList(Chatterer_Eat);
        questList[1] = new QuestList(Chatterer_Kill);
        questList[2] = new QuestList(Chatterer_Delivery);
        questList[3] = new QuestList(Merchant_Buy);
        questList[4] = new QuestList(Boy_Help);

        // 퀘스트가 진행 중이라면
        if (dm.data.questState != QuestState.None) StartQuest(dm.data.questNum);
    }

    // 퀘스트 조건 만족
    IEnumerator QuestComplete()
    {
        print("퀘스트 조건 만족");
        gm.QuestCompleteOpen();

        // 해당 NPC가 존재할 때만 NPC 상태 변경
        int curNpcIdx = int.Parse(dm.questList[dm.data.questNum]["ToNPC"].ToString());
        yield return new WaitUntil(() => npcs[curNpcIdx].npcObj);
        dm.data.questState = QuestState.Complete;
        RestNPCState();
    }

    // 모든 Npc의 상태 재정리
    public void RestNPCState()
    {
        // NPC가 존재할 때만 실행
        foreach (var npc in npcs)
        {
            if(npc.npcObj) npc.npcObj.SetQuestState();
        }
    }

    // 퀘스트 번호에 맞는 퀘스트 함수를 골라 실행
    public void StartQuest(int questNum)
    {
        RestNPCState();
        StartCoroutine(questList[questNum]());
        print(questNum + "번 퀘스트 실행");
    }

    #region [퀘스트 종류]
    IEnumerator Chatterer_Eat()
    {
        // 음식을 먹어 체력이 회복될 때까지 기다리기
        yield return new WaitUntil(() => gm.Hp >= 100);
        StartCoroutine(QuestComplete());
    }

    IEnumerator Chatterer_Kill()
    {
        float curExp = gm.Exp;

        // 몬스터를 죽여 경험치가 오를 때까지 기다리기
        yield return new WaitUntil(() => gm.Exp > curExp);
        StartCoroutine(QuestComplete());
    }


    IEnumerator Chatterer_Delivery()
    {
        // 뿔 아이템의 개수가 8개 이상이 될 때까지 기다리기
        yield return new WaitUntil(() => dm.data.questItemCount >= 8);
        StartCoroutine(QuestComplete());

        // NPC와 대화하여 해당 퀘스트가 끝나면 퀘스트용 아이템 삭제
        yield return new WaitUntil(() => dm.data.questState == QuestState.None);
        int curNpcIdx = int.Parse(dm.questList[dm.data.questNum - 1]["ToNPC"].ToString());
        npcs[curNpcIdx].npcObj.SendMessage("OutQuestItem_" + (dm.data.questNum - 1));
    }

    IEnumerator Merchant_Buy()
    {
        float curGold = gm.Gold;

        // 물건을 구매하여 골드가 줄어들 때까지 기다리기
        yield return new WaitUntil(() => gm.Gold < curGold);
        StartCoroutine(QuestComplete());
    }

    IEnumerator Boy_Help()
    {
        // 심부름 아이템의 개수가 3개 이상이 될 때까지 기다리기
        yield return new WaitUntil(() => dm.data.questItemCount >= 3);
        StartCoroutine(QuestComplete());

        // NPC와 대화하여 해당 퀘스트가 끝나면 퀘스트용 아이템 삭제
        yield return new WaitUntil(() => dm.data.questState == QuestState.None);
        int curNpcIdx = int.Parse(dm.questList[dm.data.questNum - 1]["ToNPC"].ToString());
        print(dm.data.questNum - 1);
        npcs[curNpcIdx].npcObj.SendMessage("OutQuestItem_" + (dm.data.questNum - 1));
    }
    #endregion
}
