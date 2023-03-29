using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleShell : MonsterBase
{
    [Header("닿았을 때 공격력")]
    public float atk_touch;

    // 충돌 시 부모 클래스에 전달
    private void OnTriggerEnter(Collider other)
    {
        base.Trigger(other);

        // 공격 상태가 아닐 때 닿아도 데미지
        if (other.CompareTag("Player") && !isAttack)
        {
            player.GetHit(atk_touch);
        }
    }
}