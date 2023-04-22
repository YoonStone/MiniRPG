using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Boy : NPC
{    
    // 상호작용
    void Interact()
    {
        // 던전 씬으로 전환, npc 연결 해제
        StartCoroutine(GameManager.instance.SceneFade(Vector3.zero, Vector3.one, 2));
        player.withNpc = null;
    }

    // 4번 퀘스트 완료 시 퀘스트 아이템 삭제
    void OutQuestItem_4()
    {
        // 퀘스트 체크용 변수는 초기화, 심부름 아이템 3가지 삭제하기
        dm.data.questItemCount = 0;
        foreach (var itemSlot in inventory.itemSlots)
        {
            // 퀘스트 아이템을 가진 슬롯에서 아이템 제거
            if (itemSlot.Item == inventory.items[8]
                || itemSlot.Item == inventory.items[9]
                || itemSlot.Item == inventory.items[10])
            {
                itemSlot.Count = 0;
            }
        }
    }
}
