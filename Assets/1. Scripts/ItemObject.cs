using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public Item item;

    float moveSpeed = 4f;
    float rotateSpeed;
    public bool isGet; // 아이템을 먹었다면

    private IEnumerator Start()
    {
        rotateSpeed = Random.Range(0.5f, 0.8f);
        yield return new WaitForSeconds(1);
        gameObject.layer = 6;
    }

    // 제자리 회전
    private void Update()
    {
        transform.Rotate(Vector3.up  * rotateSpeed, Space.World);
    }

    // 자석 움직임
    public IEnumerator MagentMove(Transform player)
    {
        if (CompareTag("Untagged")) yield break;
        tag = "Untagged";

        Vector3 targetPos;
        do
        {
            targetPos = player.position + new Vector3(0, 1, 0);
            transform.position = Vector3.Slerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        while (!isGet); // 플레이어와 충돌할 때까지 반복

    }

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌하면 삭제
        if (collision.transform.CompareTag("Player"))
        {
            // 플레이어랑 충돌하면 삭제 및 아이템 획득
            isGet = true;
            Destroy(gameObject);
            InventoryManager.instance.AddItem(item);
            AudioManager.instance.AudioCtrl_SFX(SFX.GetItem);
        }
    }
}
