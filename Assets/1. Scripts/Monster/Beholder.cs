using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    GameObject[] bonePool;

    private void Start()
    {
        // 뼈다귀 오브젝트풀 생성
        bonePool = new GameObject[boneCount];
        for (int i = 0; i < bonePool.Length; i++)
        {
            bonePool[i] = Instantiate(bonePref);
            bonePool[i].GetComponent<Bone>().atk = monsterInfo.atk;
            bonePool[i].SetActive(false);
        }
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
        print("던짐");
        foreach (var bone in bonePool)
        {
            if (!bone.activeSelf)
            {
                bone.transform.position = bonePos.position;
                bone.transform.rotation = Quaternion.identity;
                bone.SetActive(true);
                break;
            }
        }
    }
}
