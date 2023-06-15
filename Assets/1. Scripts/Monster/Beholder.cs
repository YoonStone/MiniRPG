using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Beholder : MonsterBase
{
    [Header("닿았을 때 공격력")]
    public float atk_touch;

    [Header("뼈다귀 던질 위치")]
    public Transform bonePos;

    [Header("던지기 용 뼈다귀")]
    public GameObject bonePref;

    [Header("뼈다귀 개수")]
    public int boneCount;

    [Header("던지는 힘")]
    public float throwPower = 10;

    IObjectPool<GameObject> bonePool;

    private void Start()
    {
        // 뼈다귀 오브젝트풀 생성
        bonePool = new ObjectPool<GameObject>
            (CreateItem, OnGetItem, OnReleaseItem, OnDestroyItem, true, boneCount, boneCount);

        for (int i = 0; i < boneCount; i++)
        {
            Bone bone = CreateItem().GetComponent<Bone>();
            bone.atk = monsterInfo.atk;
            bone.myBonePool.Release(bone.gameObject);
        }
    }

    // 뼈다귀 만들기 
    GameObject CreateItem()
    {
        GameObject bone = Instantiate(bonePref);
        bone.GetComponent<Bone>().myBonePool = bonePool;
        return bone;
    }

    // 뼈다귀 가져가기
    void OnGetItem(GameObject bone)
    {
        bone.GetComponent<Bone>().turnPower = Random.Range(-10, 10);
        bone.SetActive(true);
    }

    // 뼈다귀 돌려놓기
    void OnReleaseItem(GameObject bone)
    {
        bone.SetActive(false);
    }

    // 뼈다귀 삭제하기
    void OnDestroyItem(GameObject bone)
    {
        Destroy(bone);
    }

    // 충돌 시 부모 클래스에 전달
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        // 닿았을 때 데미지
        if (other.CompareTag("Player") && !isAttack)
        {
            player.GetHit(atk_touch);
        }
    }

    // 던지기 공격
    void Throw()
    {
        GameObject bone = bonePool.Get();
        bone.transform.SetPositionAndRotation(bonePos.position, Quaternion.identity);
        Vector3 dir = (player.transform.position + Vector3.up - transform.position).normalized;

        Rigidbody boneRigid = bone.GetComponent<Rigidbody>();
        boneRigid.velocity = Vector3.zero;
        boneRigid.angularVelocity = Vector3.zero;
        boneRigid.AddForce(dir * throwPower, ForceMode.Impulse);
    }
}
