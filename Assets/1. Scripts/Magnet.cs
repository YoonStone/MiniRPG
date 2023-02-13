using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public float moveSpeed = 4;
    public bool isTouch;

    // 플레이어와 닿으면 자석 움직임 시작
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            StartCoroutine(MagentMove(other.transform));
        }
    }

    // 자석 움직임
    IEnumerator MagentMove(Transform player)
    {
        while (!isTouch) // 충돌할 때까지 다가가기 (충돌 감지는 플레이어 담당)
        {
            Vector3 targetPos = player.position + new Vector3(0, 1, 0);
            transform.position = Vector3.Slerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // 충돌하면 삭제
        Destroy(gameObject);
    }
}
