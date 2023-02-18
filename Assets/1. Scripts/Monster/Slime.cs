using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{
    public MonsterState state;

    private void Update()
    {
        state = State;

        if (Input.GetMouseButtonDown(1))
        {
            State = MonsterState.GetHit;
        }
    }
}
