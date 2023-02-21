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
        base.Trigger(other);
    }
}
