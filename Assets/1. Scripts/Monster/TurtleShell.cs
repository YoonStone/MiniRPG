using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleShell : Monster
{
    [Header("닿았을 때 공격력")]
    public float atk_normal;

    public MonsterState _state;
    private void Update()
    {
        _state = State;
    }

    private void OnTriggerEnter(Collider other)
    {
        base.Trigger(other);

        // 닿았을 때 데미지
        if (other.CompareTag("Player") && !isAttack)
        {
            player.GetHit(atk_normal);
        }
    }
}