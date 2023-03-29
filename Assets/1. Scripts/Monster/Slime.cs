using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonsterBase
{
    // 충돌 시 부모 클래스에 전달
    private void OnTriggerEnter(Collider other)
    {
        base.Trigger(other);
    }
}
