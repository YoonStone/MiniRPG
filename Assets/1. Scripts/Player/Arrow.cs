using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Transform player;
    Rigidbody rigid;

    void Awake()
    {
        player = FindObjectOfType<PlayerAction>().transform;
        rigid = GetComponent<Rigidbody>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Invoke("SetDisable", 5);
    }

    void Update()
    {
        // 포물선으로 떨어지도록
        transform.forward = rigid.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        SetDisable();
    }

    void SetDisable()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
