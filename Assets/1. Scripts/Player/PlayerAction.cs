using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerAction;

// 액션 버튼 상호작용 종류
public enum ActionState
{
    Attack,
    WithNPC
}

public class PlayerAction : MonoBehaviour
{
    public ActionState actionState; // 상호작용 종류
    public GameObject sword, shield, bow, arrow; // 칼, 방패, 활
    public GameObject arrowPref;
    public int arrowCount;
    public float shootPower;

    [HideInInspector] public string npcName; // 대화할 NPC의 이름
    [HideInInspector] public float atk;      // 현재 공격력

    BoxCollider swordColl;   // 칼 콜라이더
    PlayerMove playerMove;
    Animator anim;
    Animator bowAnim;
    Animator arrowAnim;

    GameObject[] arrowPool;

    // 들고 있는 무기 종류
    enum WeaponType
    {
        None,
        Sword,
        Bow
    }

    WeaponType hasWeapon; // 들고 있는 무기

    void Start()
    {
        swordColl = sword.GetComponent<BoxCollider>();
        playerMove = GetComponent<PlayerMove>();
        anim = GetComponent<Animator>();
        bowAnim = bow.GetComponent<Animator>();
        arrowAnim = arrow.GetComponent<Animator>();

        PlayUIManager.instance.playerActionBtn.onClick
            .AddListener(OnClickPlayerActionBtn);

        for (int i = 0; i < PlayUIManager.instance.skills.Length; i++)
        {
            int ii = i;
            PlayUIManager.instance.skills[ii].skillBtn.onClick
                .AddListener(() => Attack(2 + ii));
        }

        // 활 오브젝트풀 생성
        arrowPool = new GameObject[arrowCount];
        for (int i = 0; i < arrowPool.Length; i++)
        {
            arrowPool[i] = Instantiate(arrowPref);
            arrowPool[i].GetComponent<Rigidbody>().centerOfMass = arrowPool[i].transform.forward * 1.5f;
            arrowPool[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Attack(Random.Range(0, 2));
    }

    // 액션 버튼
    void OnClickPlayerActionBtn()
    {
        switch (actionState)
        {
            // 공격 기능 실행
            case ActionState.Attack: Attack(Random.Range(0,2)); break;
            case ActionState.WithNPC: NPCInteract(); break;
        }
    }

    void Attack(int attackIdx)
    {
        // 공격 중이거나 알맞은 무기를 들고 있지 않을 때 공격 불가
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
            || hasWeapon == WeaponType.None
            || (attackIdx < 3 && hasWeapon != WeaponType.Sword)
            || (attackIdx >= 3 && hasWeapon != WeaponType.Bow))
            return;

        // 공격력 수정
        atk = attackIdx <= 1 ?
            10 : PlayUIManager.instance.skills[attackIdx - 2].skillAtk;

        // 이동 불가능 + 공격 애니메이션
        playerMove.isCantMove = true;
        playerMove.MoveEnd();
        anim.SetTrigger("attack");
        anim.SetInteger("attackIdx", attackIdx);
        if(attackIdx >= 3)
        {
            bowAnim.SetTrigger("bowAttack");
            bowAnim.SetInteger("bowIdx", attackIdx);
            arrowAnim.SetTrigger("arrowAttack");
            arrowAnim.SetInteger("arrowIdx", attackIdx);
        }

        StartCoroutine(AttackEndCheck());

        if (attackIdx > 1)
            StartCoroutine(PlayUIManager.instance.SkiilCoolTime(attackIdx - 2));
    }

    // 공격 종료 (애니메이션이벤트)
    IEnumerator AttackEndCheck()
    {
        swordColl.enabled = true;
        
        // 공격 애니메이션이 끝날 때까지 기다렸다가 이동 가능상태로 변경
        yield return new WaitUntil(() 
            => (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
            && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            || !playerMove.isCantMove);

        swordColl.enabled = false;
        playerMove.isCantMove = false;
    }

    void Shoot()
    {
        foreach (var arrowPref in arrowPool)
        {
            if (!arrowPref.activeSelf)
            {
                arrowPref.transform.position = arrow.transform.position + arrowPref.transform.forward;
                Rigidbody rigid = arrowPref.GetComponent<Rigidbody>();
                rigid.velocity = Vector3.zero;

                Collider[] monsters = Physics.OverlapSphere(transform.position, 3f, 1 << 7);
                print(monsters.Length);
                if (monsters.Length != 0)
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

                    Vector3 dir = (minMonster.position + Vector3.up - arrowPref.transform.position).normalized;
                    arrowPref.transform.rotation = Quaternion.LookRotation(dir);
                }
                else arrowPref.transform.rotation = arrow.transform.rotation;
                arrowPref.transform.rotation *= Quaternion.Euler(0, 0, 20);
                arrowPref.SetActive(true);

                rigid.AddForce(arrowPref.transform.forward * shootPower, ForceMode.Impulse);
                break;
            }
        }
    }

    // NPC와의 상호작용
    void NPCInteract()
    {
        // NPC 이름에 따라
        switch (npcName)
        {
            case "Who": // 아저씨
                PlayUIManager.instance.ChatBubbleOpen();
                break;


            case "Merchant": // 상인
                // 퀘스트 전 : 사용 불가
                // 퀘스트 있음 : 대화창
                // 퀘스트 수락 : 상점
                // 퀘스트 완료 : 대화창
                // 퀘스트 이후 : 상점
                break;


            case "Boy": // 꼬마애 (페이드인 후 씬 전환)
                actionState = ActionState.Attack; npcName = "";
                StartCoroutine(PlayUIManager.instance.Fade(0, 1)); break;
        }
    }

    public void GetHit(float atk)
    {
        playerMove.isCantMove = false;
        anim.SetTrigger("getHit");
        PlayUIManager.instance.Hp -= atk;
        StartCoroutine(PlayUIManager.instance.HpImgColor(Color.red));
    }

    // 장비 장착
    public void Equip(string itemName)
    {
        switch (itemName)
        {
            case "Sword":
                sword.SetActive(true);
                if (hasWeapon == WeaponType.Bow)
                {
                    bow.SetActive(false);
                    arrow.SetActive(false);
                    InventoryManager.instance.AddItem(3);
                }
                anim.SetTrigger("weaponChange");
                anim.SetBool("isBow", false);
                hasWeapon = WeaponType.Sword; break;
            case "Shield":
                shield.SetActive(true);
                if (hasWeapon == WeaponType.Bow)
                {
                    bow.SetActive(false);
                    arrow.SetActive(false);
                    InventoryManager.instance.AddItem(3);
                    anim.SetTrigger("weaponChange");
                    anim.SetBool("isBow", false);
                    hasWeapon = WeaponType.None;
                }
                break;
            case "Bow":
                bow.SetActive(true);
                arrow.SetActive(true);
                if (hasWeapon == WeaponType.Sword)
                {
                    sword.SetActive(false);
                    InventoryManager.instance.AddItem(0);
                }
                if (shield.activeSelf)
                {
                    shield.SetActive(false);
                    InventoryManager.instance.AddItem(2);
                }
                anim.SetTrigger("weaponChange");
                anim.SetBool("isBow", true);
                hasWeapon = WeaponType.Bow; break;
        }
    }
}
