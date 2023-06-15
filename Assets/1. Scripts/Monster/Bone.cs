using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bone : MonoBehaviour
{
    Rigidbody rigid;
    PlayerAction player;
    Transform spinMesh;

    public IObjectPool<GameObject> myBonePool;

    [HideInInspector] public float atk;
    [HideInInspector] public float turnPower;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerAction>();
        spinMesh = transform.GetChild(0);
    }

    private void Update()
    {
        spinMesh.Rotate(turnPower * 0.5f, turnPower, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.GetHit(atk);
        }

        if(gameObject.activeSelf) myBonePool.Release(gameObject);
    }
}
