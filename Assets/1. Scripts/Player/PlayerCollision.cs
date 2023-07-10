using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public float radius;
    public LayerMask itemLayer;

    // 주변에 아이템이 있는지 확인 
    private void Update()
    {
        // Item 용
        Collider[] itemColls = Physics.OverlapSphere(transform.position, radius, itemLayer);
        foreach (var coll in itemColls)
        {
            StartCoroutine(coll.GetComponent<ItemObject>().MagentMove(transform));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);   
    }
}
