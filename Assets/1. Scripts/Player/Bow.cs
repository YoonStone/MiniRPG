using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public Transform player;
    public PlayerAction playerAction;
    public RectTransform aiming;
    public Vector3 boxSize;
    public LayerMask MonsterLayer;

    List<Transform> monsters = new List<Transform>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Monster")
        {
            monsters.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Monster")
        {
            monsters.Remove(other.transform);
        }
    }

    private void Update()
    {
        if (monsters.Count > 0)
        {
            Transform nearMonster = monsters[0].transform;
            for (int i = 0; i < monsters.Count; i++)
            {
                // 이전에 가까운 몬스터보다 더 가까우면 가장 가까운 몬스터로 등록
                if (MonsterDistance(nearMonster) > MonsterDistance(monsters[i].transform))
                    nearMonster = monsters[i].transform;
            }

            // 조준된 몬스터 전달
            aiming.gameObject.SetActive(true);
            aiming.SetParent(nearMonster.GetComponentInChildren<Canvas>().transform);
            aiming.anchoredPosition3D = new Vector3(0, 350, 0);
            aiming.localRotation = Quaternion.identity;
        }

        // 조준된 몬스터가 없다면 비우기
        else
        {
            aiming.SetParent(player);
            aiming.gameObject.SetActive(false);
        }
    }

    // 몬스터와의 거리 계산 후 반환
    float MonsterDistance(Transform monster)
    {
        return Vector3.Distance(player.position, monster.position);
    }
}
