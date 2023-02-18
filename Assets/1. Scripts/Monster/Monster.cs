using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public enum MonsterState
    {
        Idle,   // 기본
        Follow, // 플레이어 추격
        Attack, // 공격
        GetHit, // 피격
        Die     // 죽음
    }

    private MonsterState state;
    public MonsterState State
    {
        get { return state; }
        set
        {
            state = value;

            switch (state)
            {
                case MonsterState.Idle: StartCoroutine(Idle()); break;
                case MonsterState.Follow: StartCoroutine(Follow()); break;
                case MonsterState.Attack: StartCoroutine(Attack()); break;
                case MonsterState.GetHit: StartCoroutine(GetHit()); break;
                case MonsterState.Die: Die(); break;
            }
        }
    }

    Animator anim;
    NavMeshAgent agent;
    PlayerAction player;
    Transform playerTr;

    [Header("플레이어와의 거리")]
    public float dist_Follow, dist_Attack;

    [Header("공격력")]
    public float atk;

    [HideInInspector]
    public bool isAttack; // 공격 중인지

    public float hp = 100;

    private void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerAction>();

        playerTr = player.transform;
        agent.stoppingDistance = dist_Attack - 0.5f;
        State = MonsterState.Idle;
    }

    // 기본
    public virtual IEnumerator Idle()
    {
        anim.SetBool("isMove", false);
        agent.isStopped = true;
        agent.ResetPath();

        while (State == MonsterState.Idle)
        {
            if (Vector3.Distance(transform.position, playerTr.position) <= dist_Follow)
                State = MonsterState.Follow;
            yield return null;
        }
    }

    // 추격
    public virtual IEnumerator Follow()
    {
        anim.SetBool("isMove", true);
        agent.isStopped = false;

        while (State == MonsterState.Follow)
        {
            float distance = Vector3.Distance(transform.position, playerTr.position);
            if (distance <= dist_Attack) State = MonsterState.Attack;
            else if (distance > dist_Follow) State = MonsterState.Idle;
            else agent.destination = playerTr.position;
            yield return null;
        }
    }

    // 공격
    public virtual IEnumerator Attack()
    {
        anim.SetBool("isAttack", true);
        anim.SetTrigger("attack");
        agent.isStopped = true;
        agent.ResetPath();

        while (State == MonsterState.Attack)
        {
            if (Vector3.Distance(transform.position, playerTr.position) > dist_Attack)
            {
                State = MonsterState.Follow;
            }
            else
            {
                transform.LookAt(playerTr, Vector3.up);
            }
            yield return null;
        }

        anim.SetBool("isAttack", false);
    }

    public virtual IEnumerator GetHit()
    {
        hp -= 10;

        if(hp > 0)
        {
            anim.SetTrigger("getHit");
            yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length);
            State = MonsterState.Idle;
        }
        else
        {
            State = MonsterState.Die;
        }
    }

    public virtual void Die()
    {
        anim.SetTrigger("die");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && isAttack)
        {
            PlayUIManager.instance.Hp -= atk;
            StartCoroutine(PlayUIManager.instance.GetHitEffect());
            isAttack = false;
        }
    }
}
