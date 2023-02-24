using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{
    public MonsterState _state;

    private void Update()
    {
        _state = State;   
    }

    private void OnTriggerEnter(Collider other)
    {
        base.Trigger(other);
    }
}
