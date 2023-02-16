using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemObject : MonoBehaviour
{
    public Item item;

    [HideInInspector]
    public bool isTouch;

    bool isPlayer;
    float moveSpeed = 4f;

    private void Update()
    {
        transform.Rotate(Vector3.up,Space.World);
    }

    // 플레이어와 닿으면 자석 움직임 시작
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isPlayer)
        {
            isPlayer = true;
            StartCoroutine(MagentMove(other.transform));
        }
    }

    // 자석 움직임
    IEnumerator MagentMove(Transform player)
    {
        Vector3 targetPos;
        float distance;
        do
        {
            targetPos = player.position + new Vector3(0, 1, 0);
            transform.position = Vector3.Slerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
            distance = Vector3.Distance(transform.position, targetPos);
            yield return null;
        }
        while (distance > 0.5f); // 가까워질 때까지 반복

        // 충돌하면 삭제
        Destroy(gameObject);
        InventoryManager.instance.AddItem(item);
    }

}
