using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestMonster : MonsterBase
{
    public GameObject[] questItems;

    // 공격
    protected override IEnumerator Attack()
    {
        StartCoroutine(base.Attack());

        while (State == MonsterState.Attack)
        {
            // 플레이어와의 거리에 따라 다른 공격 애니메이션
            anim.SetFloat("attackDist", Vector3.Distance(transform.position, player.transform.position));
            yield return null;
        }
    }

    protected override IEnumerator Die()
    {
        DataManager dm = DataManager.instance;
        Item[] items = InventoryManager.instance.items;

        // 남자아이 돕는 퀘스트 진행 중일 때만 퀘스트 아이템 드랍
        if (dm.data.questNum == 4 && dm.data.questState == QuestState.Accept)
        {
            for (int i = 0; i < questItems.Length; i++)
            {
                Vector3 spawnPos = transform.position + Vector3.up;
                GameObject item = Instantiate(questItems[i], spawnPos, Quaternion.identity);
                item.GetComponent<Rigidbody>().AddForce(Vector3.up * 5, ForceMode.Impulse);
            }
        }

        return base.Die();
    }
}