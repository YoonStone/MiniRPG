using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleShell : MonsterBase
{
    [Header("닿았을 때 공격력")]
    public float atk_touch;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        // 공격 상태가 아닐 때 닿아도 데미지
        if (other.CompareTag("Player") && !isAttack)
        {
            player.GetHit(atk_touch);
        }
    }
}