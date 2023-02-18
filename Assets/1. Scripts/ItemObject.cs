using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemObject : MonoBehaviour
{
    public Item item;

    float moveSpeed = 4f;

    private void Update()
    {
        transform.Rotate(Vector3.up,Space.World);
    }

    // 자석 움직임
    public IEnumerator MagentMove(Transform player)
    {
        if (CompareTag("Untagged")) yield break;
        tag = "Untagged";

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
