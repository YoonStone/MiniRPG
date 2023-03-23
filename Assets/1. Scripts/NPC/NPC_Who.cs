using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using UnityEngine;

public class NPC_Who : NPC
{
    IEnumerator Quest_Eat()
    {
        // 음식을 먹어 체력이 회복될 때까지 기다리기
        yield return new WaitUntil(() => PlayUIManager.instance.hpImg.color == UnityEngine.Color.green);
        QuestComplete();
    }

    IEnumerator Quest_Kill()
    {
        // 몬스터를 죽여 경험치가 오를 때까지 기다리기
        yield return new WaitUntil(() => PlayUIManager.instance.hpImg.color == UnityEngine.Color.green);
        QuestComplete();
    }

}
