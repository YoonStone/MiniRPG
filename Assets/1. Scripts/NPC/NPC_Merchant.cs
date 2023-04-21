using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Merchant : NPC
{
    // 상호작용
    void Interact()
    {
        // 열려있다면 상점 종료
        if (inventory.isOpenShop) PlayerBye();
        else
        {
            gm.anim_Shop.SetBool("isOpen", true);
            gm.anim_Inventory.SetBool("isOpen", true);
            inventory.isOpenShop = true;
        }
    }

    // 플레이어와 멀어졌을 때
    void PlayerBye()
    {
        gm.anim_Shop.SetBool("isOpen", false);
        inventory.isOpenShop = false;
    }

    IEnumerator Quest_Buy()
    {
        // 이제 상호작용 가능
        isInteractable = true;

        float curGold = gm.Gold;

        // 물건을 구매하여 골드가 줄어들 때까지 기다리기
        yield return new WaitUntil(() => gm.Gold < curGold);
        QuestComplete();
    }
}
