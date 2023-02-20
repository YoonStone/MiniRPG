using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject nickName;

    PlayerAction player;

    void Start()
    {
        player = FindObjectOfType<PlayerAction>();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Player": // player가 근처에 있을 때
                nickName.SetActive(true);
                player.actionState = ActionState.WithNPC;
                player.npcName = other.name.Split('_')[1];
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Player": // player와 헤어졌을 때
                nickName.SetActive(false);
                player.actionState = ActionState.Attack;
                player.npcName = "";
                break;
        }
    }
}
