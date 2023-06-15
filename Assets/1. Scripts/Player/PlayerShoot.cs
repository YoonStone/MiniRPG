using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerShoot : MonoBehaviour
{
    public GameObject arrowPref;
    public int arrowCount = 10;
    public float shootPower = 8;

    PlayerAction playerAction;
    IObjectPool<GameObject> arrowPool;

    private void Start()
    {
        playerAction = GetComponent<PlayerAction>();

        // 활 오브젝트풀 생성
        arrowPool = new ObjectPool<GameObject>
            (CreateItem, OnGetItem, OnReleaseItem, OnDestroyItem, true, arrowCount, arrowCount);

        for (int i = 0; i < arrowCount; i++)
        {
            Arrow arrow = CreateItem().GetComponent<Arrow>();
            arrow.shootPower = shootPower;
            arrow.myArrowPool.Release(arrow.gameObject);
        }
    }

    // 화살 만들기
    GameObject CreateItem()
    {
        GameObject arrow = Instantiate(arrowPref,GameManager.instance.transform);
        arrow.GetComponent<Arrow>().myArrowPool = arrowPool;
        return arrow;
    }

    // 화살 가져가기
    void OnGetItem(GameObject arrow)
    {
        arrow.SetActive(true);
    }

    // 화살 돌려놓기
    void OnReleaseItem(GameObject arrow)
    {
        Rigidbody arrowRigid = arrow.GetComponent<Rigidbody>();
        arrowRigid.velocity = Vector3.zero;
        arrowRigid.angularVelocity = Vector3.zero;
        arrow.SetActive(false);
    }

    // 화살 삭제하기
    void OnDestroyItem(GameObject arrow)
    {
        Destroy(arrow);
    }

    // 화살 발사
    IEnumerator Shoot()
    {
        GameObject arrow = arrowPool.Get();
        arrow.transform.SetPositionAndRotation(
            transform.position + transform.forward + transform.up,
            playerAction.arrow.transform.rotation * Quaternion.Euler(0, 20, 0));

        // 조준된 몬스터가 있다면 방향 구해서 발사
        if (playerAction.target)
        {
            Vector3 dir = (playerAction.target.position + Vector3.up * 0.5f - transform.position).normalized;
            arrow.GetComponent<Rigidbody>().velocity = dir * shootPower;
        }
        // 조준된 몬스터가 없다면 일반 발사
        else
        {
            arrow.GetComponent<Rigidbody>().velocity = transform.forward * 5 + transform.up * 3;
        }

        AudioManager.instance.AudioCtrl_SFX(AttackAudio.Bow);

        yield return new WaitForSeconds(5f);
        if (arrow.activeSelf) arrowPool.Release(arrow.gameObject);
    }
}
