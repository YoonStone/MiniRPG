using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Who : NPC
{
    IEnumerator Quest_Eat()
    {
        float curHp = manager.Hp;

        // 음식을 먹어 체력이 회복될 때까지 기다리기
        yield return new WaitUntil(() => manager.Hp > curHp);
        QuestComplete();
    }

    IEnumerator Quest_Kill()
    {
        float curExp = manager.Exp;

        // 몬스터를 죽여 경험치가 오를 때까지 기다리기
        yield return new WaitUntil(() => manager.Exp > curExp);
        QuestComplete();
    }


    IEnumerator Quest_Delivery()
    {
        // 뿔 아이템의 개수가 8개 이상이 될 때까지 기다리기
        yield return new WaitUntil(() => inventory.questItemCount >= 8);
        GameObject.Find("NPC_Merchant").SendMessage("QuestComplete");

        // 상인을 만나 해당 퀘스트가 끝나면
        yield return new WaitUntil(() => npcQuestState == NPCQuestState.None);

        // 퀘스트용 아이템 삭제
        InventoryManager.instance.questItemCount = 0;
        foreach (var itemSlot in inventory.itemSlots)
        {
            // 같은 아이템을 가진 슬롯에서 아이템 제거
            if (itemSlot.Item == inventory.items[4])
            {
                itemSlot.Count = 0;
                yield break;
            }
        }
    }
}
