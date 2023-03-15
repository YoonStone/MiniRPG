using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody rigid;

    [Header("던지는 힘")]
    public float throwPower = 10;

    [HideInInspector]
    public float atk;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Shoot()
    {
        rigid.velocity = Vector3.zero;
        Vector3 dir;

        Collider[] monsters = Physics.OverlapSphere(transform.position, 3f, LayerMask.NameToLayer("Monster"));
        if(monsters.Length != 0)
        {
            float minDist = 0;
            Transform minMonster = monsters[0].transform;
            for (int i = 0; i < monsters.Length; i++)
            {
                float dist = Vector3.Distance(transform.position, monsters[i].transform.position);
                if (minDist > dist)
                {
                    minDist = dist;
                    minMonster = monsters[i].transform;
                }
            }

            dir = (minMonster.position - transform.position).normalized;
        }
        else dir = transform.forward;

        rigid.AddForce(dir * throwPower, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Player"))
        //{
        //    player.GetHit(atk);
        //}
        //gameObject.SetActive(false);
    }
}
