using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    PlayerAction playerAction;
    InventoryManager inventory;

    private void Start()
    {
        playerAction = GetComponent<PlayerAction>();
        inventory = FindObjectOfType<InventoryManager>();
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

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Item": // Item과 닿았을 때
                collision.gameObject.GetComponent<Magnet>().isTouch = true;
                inventory.AddItem(collision.gameObject.GetComponent<ItemObject>().item);
                break;
        }
    }
}
