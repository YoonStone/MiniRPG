using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    PlayerAction playerAction;

    private void Start()
    {
        playerAction = GetComponent<PlayerAction>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // NPC�� ������ ��
        if(other.tag == "NPC")
        {
            other.SendMessage("PlayerHI");
            playerAction.actionState = ActionState.WithNPC;
            playerAction.npcName = other.name;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // NPC�� ������� ��
        if (other.tag == "NPC")
        {
            other.SendMessage("PlayerBye");
            playerAction.actionState = ActionState.None;
            playerAction.npcName = "";
        }
    }
}
