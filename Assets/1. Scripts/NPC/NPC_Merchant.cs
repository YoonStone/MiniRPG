using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using UnityEngine;

public class NPC_Merchant : NPC
{
    // 상호작용
    void Interact()
    {
        PlayUIManager.instance.anim_Shop.SetBool("isOpen", true);
        PlayUIManager.instance.anim_Inventory.SetBool("isOpen", true);
        InventoryManager.instance.isOpenShop = true;
    }

    void PlayerBye()
    {
        PlayUIManager.instance.anim_Shop.SetBool("isOpen", false);
        InventoryManager.instance.isOpenShop = false;
    }

    IEnumerator Quest_Buy()
    {
        // 이제 상호작용 가능
        isInteractable = true;

        float curHp = DataManager.instance.data.hp;

        // 물건을 구매하여 골드가 줄어들 때까지 기다리기
        yield return new WaitUntil(() => DataManager.instance.data.hp > curHp);
        QuestComplete();
    }
}
