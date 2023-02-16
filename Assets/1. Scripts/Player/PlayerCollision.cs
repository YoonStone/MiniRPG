using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    PlayerAction playerAction;

    private void Start()
    {
        playerAction = GetComponentInParent<PlayerAction>();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "NPC": // NPC이 근처에 있을 때
                other.SendMessage("PlayerHI");
                playerAction.actionState = ActionState.WithNPC;
                playerAction.npcName = other.name.Split('_')[1];
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "NPC": // NPC와 헤어졌을 때
                other.SendMessage("PlayerBye");
                playerAction.actionState = ActionState.None;
                playerAction.npcName = "";
                break;
        }
    }
}
