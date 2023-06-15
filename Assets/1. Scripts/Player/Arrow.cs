using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Arrow : MonoBehaviour
{
    Transform player;
    Rigidbody rigid;

    [HideInInspector] public float shootPower;

    public IObjectPool<GameObject> myArrowPool;

    void Awake()
    {
        player = FindObjectOfType<PlayerAction>().transform;
        rigid = GetComponent<Rigidbody>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        // 이동하는 방향대로 바라보도록 (=포물선 운동)
        transform.forward = rigid.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.activeSelf) myArrowPool.Release(this.gameObject);
    }
}
