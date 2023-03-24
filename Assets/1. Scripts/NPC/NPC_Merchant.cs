using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using UnityEngine;

public class NPC_Merchant : NPC
{
    IEnumerator Quest_Buy()
    {
        float curHp = DataManager.instance.data.hp;

        // 물건을 구매하여 골드가 줄어들 때까지 기다리기
        yield return new WaitUntil(() => DataManager.instance.data.hp > curHp);
        QuestComplete();
    }
}
