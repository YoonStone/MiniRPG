using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestMonster : Monster
{
    public MonsterState _state;
    public float distance;

    private void Update()
    {
        _state = State;
    }

    private void OnTriggerEnter(Collider other)
    {
        base.Trigger(other);
    }

    // 공격
    protected override IEnumerator Attack()
    {
        StartCoroutine(base.Attack());

        while (State == MonsterState.Attack)
        {
            // 플레이어와의 거리에 따라 다른 공격 애니메이션
            distance = Vector3.Distance(transform.position, player.transform.position);
            anim.SetFloat("attackDist", distance);
            yield return null;
        }

        yield break;
    }

}
