using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Boy : NPC
{    
    // 상호작용
    void Interact()
    {
        // 던전 씬으로 전환, npc 연결 해제
        StartCoroutine(PlayUIManager.instance.Fade(Vector3.zero, Vector3.one));
        player.withNpc = null;
    }

    IEnumerator Quest_Help()
    {
        // 심부름 아이템의 개수가 3개 이상이 될 때까지 기다리기
        yield return new WaitUntil(() => inventory.questItemCount >= 3);
        gameObject.SendMessage("QuestComplete");

        // 퀘스트용 아이템 삭제
        InventoryManager.instance.questItemCount = 0;
        foreach (var itemSlot in inventory.itemSlots)
        {
            // 퀘스트 아이템을 가진 슬롯에서 아이템 제거
            if (itemSlot.Item == inventory.items[8] || itemSlot.Item == inventory.items[9] || itemSlot.Item == inventory.items[10])
            {
                itemSlot.Count = 0;
            }
        }
    }
}
