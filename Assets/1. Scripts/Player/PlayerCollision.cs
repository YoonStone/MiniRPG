using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public float radius;
    public LayerMask overlapLayer;
    public LayerMask castLayer;

    PlayerAction playerAction;
    RaycastHit hit;

    private void Start()
    {
        playerAction = GetComponent<PlayerAction>();
    }

    private void Update()
    {
        // Item 용
        Collider[] colls = Physics.OverlapSphere(transform.position, radius, overlapLayer);
        foreach (var coll in colls)
        {
            StartCoroutine(coll.GetComponent<ItemObject>().MagentMove(transform));
        }

        //// NPC, Monster 용
        //if(Physics.SphereCast(transform.position, radius, Vector3.up, out hit, 0, castLayer))
        //{
        //    Debug.Log(hit.transform.name);
        //}
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);   
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    switch (other.tag)
    //    {
    //        case "NPC": // NPC이 근처에 있을 때
    //            other.SendMessage("PlayerHI");
    //            playerAction.actionState = ActionState.WithNPC;
    //            playerAction.npcName = other.name.Split('_')[1];
    //            break;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    switch (other.tag)
    //    {
    //        case "NPC": // NPC와 헤어졌을 때
    //            other.SendMessage("PlayerBye");
    //            playerAction.actionState = ActionState.None;
    //            playerAction.npcName = "";
    //            break;
    //    }
    //}
}
