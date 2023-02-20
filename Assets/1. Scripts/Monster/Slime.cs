using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{
    public MonsterState state;

    private void Update()
    {
        state = State;   
    }

    private void OnTriggerEnter(Collider other)
    {
        // 피격
        if (other.CompareTag("PlayerAttack"))
        {
            other.enabled = false;
            StartCoroutine(GetHit());
        }

        // 공격
        else if (other.CompareTag("Player"))
        {
            AttackAction();
        }
    }

    public override void AttackAction()
    {
        // 진짜 공격
        base.AttackAction();
    }
}
