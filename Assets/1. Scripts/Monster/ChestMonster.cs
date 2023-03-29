using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestMonster : MonsterBase
{
    // 충돌 시 부모 클래스에 전달
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
            anim.SetFloat("attackDist", Vector3.Distance(transform.position, player.transform.position));
            yield return null;
        }

        yield break;
    }
}
