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

    //List<Transform> monsters = new List<Transform>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Monster")
        {
            //monsters.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Monster")
        {
            //monsters.Remove(other.transform);
        }
    }

    private void Update()
    {
        Collider[] monsters
            = Physics.OverlapBox(player.position + player.forward * 2f + player.up * 1.2f,
            boxSize, player.rotation, MonsterLayer);

        if (monsters.Length > 0)
        {
            Transform nearMonster = monsters[0].transform;
            for (int i = 0; i < monsters.Length; i++)
            {
                // 이전에 가까운 몬스터보다 더 가까우면 가장 가까운 몬스터로 등록
                if (MonsterDistance(nearMonster) > MonsterDistance(monsters[i].transform))
                    nearMonster = monsters[i].transform;
            }

            // 조준된 몬스터 전달
            aiming.gameObject.SetActive(true);
            aiming.SetParent(nearMonster.GetComponent<MonsterBase>().shootPos);
            aiming.anchoredPosition3D = new Vector3(0, 0, 0);
            aiming.localRotation = Quaternion.identity;
            aiming.gameObject.SetActive(true);
            playerAction.target = nearMonster;
        }

        // 조준된 몬스터가 없다면 비우기
        else
        {
            aiming.SetParent(player);
            aiming.gameObject.SetActive(false);
            playerAction.target = null;
        }
    }

    // 몬스터와의 거리 계산 후 반환
    float MonsterDistance(Transform monster)
    {
        return Vector3.Distance(player.position, monster.position);
    }

    void OnDrawGizmos()
    {
        //Gizmos.matrix = player.localToWorldMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(player.position + player.forward * 2f + player.up * 1.2f, boxSize * 2);
    }
}
