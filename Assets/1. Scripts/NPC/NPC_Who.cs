using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using UnityEngine;

public class NPC_Who : NPC
{
    IEnumerator Quest_Eat()
    {
        float curHp = DataManager.instance.data.hp;

        // 음식을 먹어 체력이 회복될 때까지 기다리기
        yield return new WaitUntil(() => DataManager.instance.data.hp > curHp);
        QuestComplete();
    }

    IEnumerator Quest_Kill()
    {
        float curExp = DataManager.instance.data.exp;

        // 몬스터를 죽여 경험치가 오를 때까지 기다리기
        yield return new WaitUntil(() => DataManager.instance.data.exp > curExp);
        QuestComplete();
    }


    IEnumerator Quest_Delivery()
    {
        // 뿔 아이템의 개수가 10개 이상이 될 때까지 기다리기
        yield return new WaitUntil(() => InventoryManager.instance.questItemCount >= 6);
        GameObject.Find("NPC_Merchant").SendMessage("QuestComplete");

        // 상인을 만나 해당 퀘스트가 끝나면
        yield return new WaitUntil(() => npcQuestState == NPCQuestState.None);
        // 퀘스트용 아이템 삭제
        InventoryManager.instance.questItemCount = 0;
        foreach (var itemSlot in InventoryManager.instance.itemSlots)
        {
            // 같은 아이템을 가진 슬롯에서 아이템 제거
            if (itemSlot.Item == InventoryManager.instance.items[4])
            {
                itemSlot.Count = 0;
                yield break;
            }
        }

    }
}
