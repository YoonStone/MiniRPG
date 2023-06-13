using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Merchant : NPC
{
    // 상호작용
    void Interact()
    {
        // 열려있다면 상점 종료
        if (inventory.isOpenShop)
        {
            gm.anim_Shop.SetBool("isOpen", false);
            AudioManager.instance.AudioCtrl_Effect(Effect.EffectDown);
            inventory.isOpenShop = false;
        }
        else
        {
            gm.anim_Shop.SetBool("isOpen", true);
            gm.anim_Inventory.SetBool("isOpen", true);
            AudioManager.instance.AudioCtrl_Effect(Effect.EffectUp);
            inventory.isOpenShop = true;
        }
    }

    // 플레이어와 멀어졌을 때
    void PlayerBye()
    {
        if (inventory.isOpenShop)
        {
            gm.anim_Shop.SetBool("isOpen", false);
            AudioManager.instance.AudioCtrl_Effect(Effect.EffectDown);
            inventory.isOpenShop = false;
        }
    }

    // 2번 퀘스트 완료 시 퀘스트 아이템 삭제
    void OutQuestItem_2()
    {
        // 퀘스트 체크용 변수는 초기화, 뿔 8개 삭제하기
        dm.data.questItemCount = 0;
        foreach (var itemSlot in inventory.itemSlots)
        {
            // 같은 아이템을 가진 슬롯에서 아이템 제거
            if (itemSlot.Item == inventory.items[4])
            {
                itemSlot.Count -= 8;
                return;
            }
        }
    }

}
