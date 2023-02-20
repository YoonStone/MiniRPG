using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Net.NetworkInformation;

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
                case MonsterState.Die: Die(); break;
            }
        }
    }

    private float hp;
    public float Hp
    {
        get { return hp; }
        set
        {
            hp = value;
            hpImg.fillAmount = hp / maxHp;
        }
    }

    Animator anim;
    NavMeshAgent agent;
    protected PlayerAction player;
    Transform playerTr;
    CanvasGroup cg;

    [Header("체력바")]
    public Image hpImg;

    [Header("최대 체력")]
    public float maxHp;

    [Header("플레이어와의 거리")]
    public float dist_Follow, dist_Attack;

    [Header("공격력")]
    public float atk;

    [HideInInspector]
    public bool isAttack; // 공격 중인지

    private void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerAction>();
        cg = GetComponentInChildren<CanvasGroup>();

        Hp = maxHp;
        playerTr = player.transform;
        agent.stoppingDistance = dist_Attack - 0.5f;
        State = MonsterState.Idle;
    }

    // 기본
    public virtual IEnumerator Idle()
    {
        cg.alpha = 0;
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
        cg.alpha = 1;
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

    // 피격
    MonsterState curState;
    public virtual IEnumerator GetHit()
    {
        Hp -= 10;

        cg.alpha = 1;
        agent.isStopped = true;
        agent.ResetPath();

        if(hp > 0)
        {
            // 이전 상태 저장 후 상태 변경
            if (State != MonsterState.GetHit)
            {
                curState = State;
                State = MonsterState.GetHit;
            }

            anim.SetTrigger("getHit");
            yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length);
            if(State == MonsterState.GetHit) State = curState; // 이전 상태로 돌아가기
        }
        else
        {
            State = MonsterState.Die;
        }
    }

    // 죽음
    public virtual void Die()
    {
        cg.alpha = 0;
        anim.SetTrigger("die");
        GetComponent<Collider>().enabled = false;
        this.enabled = false;
    }

    // 실제 공격 
    public virtual void AttackAction()
    {
        if (isAttack)
        {
            player.GetHit(atk);
            isAttack = false;
        }
    }
}
