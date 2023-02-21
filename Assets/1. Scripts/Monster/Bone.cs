using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : MonoBehaviour
{
    Rigidbody rigid;
    PlayerAction player;

    [Header("던지는 힘")]
    public float throwPower = 10;

    [HideInInspector]
    public float atk;

    float turnPower;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerAction>();
    }

    private void OnEnable()
    {
        rigid.velocity = Vector3.zero;
        Vector3 dir = (player.transform.position + Vector3.up - transform.position).normalized;
        rigid.AddForce(dir * throwPower, ForceMode.Impulse);

        turnPower = Random.Range(-10, 10);
    }

    private void Update()
    {
        transform.Rotate(0, turnPower, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.GetHit(atk);
        }
        gameObject.SetActive(false);
    }
}
