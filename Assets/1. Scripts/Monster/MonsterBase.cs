using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Net.NetworkInformation;

public class MonsterBase : MonoBehaviour
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
                case MonsterState.Die: StartCoroutine(Die()); break;
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
            hpImg.fillAmount = hp / monsterInfo.hp_max;
        }
    }

    protected Animator anim;
    NavMeshAgent agent;
    protected PlayerAction player;
    Transform playerTr;
    CanvasGroup cg;

    [Header("체력바")]
    public Image hpImg;

    [Header("몬스터 정보")]
    public MonsterInfo monsterInfo;

    [HideInInspector]
    public bool isAttack; // 공격 중인지

    private void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerAction>();
        cg = GetComponentInChildren<CanvasGroup>();

        Hp = monsterInfo.hp_max;
        playerTr = player.transform;
        agent.stoppingDistance = monsterInfo.dist_Attack - 0.5f;
        State = MonsterState.Idle;
    }

    // 기본
    public IEnumerator Idle()
    {
        cg.alpha = 0;
        anim.SetBool("isMove", false);
        agent.isStopped = true;
        agent.ResetPath();

        while (State == MonsterState.Idle)
        {
            if (Vector3.Distance(transform.position, playerTr.position) <= monsterInfo.dist_Follow)
                State = MonsterState.Follow;
            yield return null;
        }
    }

    // 추격
    public IEnumerator Follow()
    {
        cg.alpha = 1;
        anim.SetBool("isMove", true);
        agent.isStopped = false;

        while (State == MonsterState.Follow)
        {
            float distance = Vector3.Distance(transform.position, playerTr.position);
            if (distance <= monsterInfo.dist_Attack) State = MonsterState.Attack;
            else if (distance > monsterInfo.dist_Follow) State = MonsterState.Idle;
            else agent.destination = playerTr.position;
            yield return null;
        }
    }

    // 공격
    protected virtual IEnumerator Attack()
    {
        anim.SetBool("isAttack", true);
        anim.SetTrigger("attack");
        agent.isStopped = true;
        agent.ResetPath();

        while (State == MonsterState.Attack)
        {
            if (Vector3.Distance(transform.position, playerTr.position) > monsterInfo.dist_Attack)
            {
                State = MonsterState.Follow;
            }
            else
            {
                transform.LookAt(playerTr);
                Quaternion rot = transform.rotation; rot.x = 0; rot.z = 0; // y축만 회전
                transform.rotation = rot;
            }
            yield return null;
        }

        anim.SetBool("isAttack", false);
    }

    // 피격
    MonsterState curState;
    protected IEnumerator GetHit()
    {
        Hp -= DataManager.instance.data.atk;

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
    protected virtual IEnumerator Die()
    {
        print("부모의 죽음 함수 호출");
        cg.alpha = 0;
        anim.SetTrigger("die");
        GetComponent<Collider>().enabled = false;
        this.enabled = false;

        // 경험치, 골드 증가
        GameManager.instance.Exp += Random.Range(monsterInfo.exp_min, monsterInfo.exp_max);
        GameManager.instance.Gold += Random.Range(monsterInfo.gold_min, monsterInfo.gold_max);

        // 죽는 애니메이션이 끝난 후
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length);

        // 아이템 드랍
        for (int i = 0; i < monsterInfo.dropItems.Length; i++)
        {
            // 랜덤한 확률
            float randPercent = Random.Range(0f, 1f);
            if (randPercent <= monsterInfo.dropItems[i].percent)
            {
                // 랜덤한 개수
                int minCount = monsterInfo.dropItems[i].dropCount_min;
                int maxCount = monsterInfo.dropItems[i].dropCount_max;
                int randCount = Random.Range(minCount, maxCount + 1);

                for (int j = 0; j < randCount; j++)
                {
                    Vector3 spawnPos = transform.position + Vector3.up;
                    GameObject item = Instantiate(monsterInfo.dropItems[i].itemPref, spawnPos, Quaternion.identity);
                    item.GetComponent<Rigidbody>().AddForce(Vector3.up * 5, ForceMode.Impulse);
                }
            }
        }

        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        // 피격
        if (other.CompareTag("PlayerAttack"))
        {
            other.enabled = false;
            StartCoroutine(GetHit());
        }

        // 기본 공격
        else if (other.CompareTag("Player"))
        {
            if (isAttack)
            {
                player.GetHit(monsterInfo.atk);
                isAttack = false;
            }
        }
    }
}
