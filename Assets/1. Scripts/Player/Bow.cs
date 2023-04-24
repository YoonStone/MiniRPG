using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public Transform player;
    public float radius;
    public LayerMask MonsterLayer;

    private void Update()
    {
        // 적만 감지할 수 있는 충돌체
        Collider[] monsters = Physics.OverlapSphere(player.position + player.forward, radius, MonsterLayer);
        foreach (var monster in monsters)
        {
            // 가장 가까운 몬스터를 찾아 플레이어액션으로 전달, 조준 표시
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(player.position + player.forward * 1.5f + player.up, radius);
    }
}
