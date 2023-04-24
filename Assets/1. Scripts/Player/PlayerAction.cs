using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NPC;
using static PlayerAction;

public class PlayerAction : MonoBehaviour
{
    [Header("상호작용 중인 NPC")]
    public NPC withNpc; // 가까이 있는 npc

    [Header("칼, 방패")]
    public GameObject sword;
    public GameObject shield;

    [Header("활, 화살")]
    public GameObject bow;
    public GameObject arrow;
    public GameObject arrowPref;
    public int arrowCount;
    public float shootPower;

    BoxCollider swordColl;
    PlayerMove playerMove;
    Animator anim;
    Animator bowAnim;
    Animator arrowAnim;

    DataManager dm;
    GameManager gm;
    InventoryManager inventory;

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

        dm = DataManager.instance;
        gm = GameManager.instance;
        inventory = InventoryManager.instance;

        gm.playerActionBtn.onClick.AddListener(OnClickPlayerActionBtn);

        for (int i = 0; i < gm.skills.Length; i++)
        {
            int ii = i;
            gm.skills[ii].skillBtn.onClick.AddListener(() => Attack(2 + ii));
        }

        // 활 오브젝트풀 생성
        arrowPool = new GameObject[arrowCount];
        for (int i = 0; i < arrowPool.Length; i++)
        {
            arrowPool[i] = Instantiate(arrowPref);
            arrowPool[i].GetComponent<Rigidbody>().centerOfMass
                = arrowPool[i].transform.forward * 1.5f;
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
        if (withNpc) NPCInteract();
        else Attack(Random.Range(0, 2));
    }

    void Attack(int attackIdx)
    {
        // 공격 중이거나 알맞은 무기를 들고 있지 않을 때 공격 불가
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
            || hasWeapon == WeaponType.None
            || (attackIdx < 3 && hasWeapon != WeaponType.Sword)
            || (attackIdx >= 3 && hasWeapon != WeaponType.Bow))
            return;

        // 공격력 수정 (기본 공격력 10)
        dm.data.atk = attackIdx <= 1 ? 10 : gm.skills[attackIdx - 2].skillAtk;

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
            StartCoroutine(gm.SkiilCoolTime(attackIdx - 2));
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
        // 퀘스트를 갖고 있는 상태
        if(withNpc.NPCState == NPCQuestState.Have) gm.CheckBubble();

        // 퀘스트를 기다리고 있는 상태
        else if(withNpc.NPCState == NPCQuestState.Wait)
        {
            // 퀘스트 조건에 만족한다면 퀘스트 완료
            if(dm.data.questState == QuestState.Complete) AfterQuestComplete();

            // 퀘스트를 기다리는 중이지만 완료하지 못했을 때 상호작용 시도
            else if (withNpc.info.npcName == "Merchant" || withNpc.info.npcName == "Boy")
                withNpc.SendMessage("Interact");
        }

        // 현재 퀘스트와 관련 없지만 상호작용 가능한 상태
        else if (withNpc.isInteractable) withNpc.SendMessage("Interact");
    }

    // 퀘스트 완료 행동
    void AfterQuestComplete()
    {
        // 보상 받기
        string getItem = dm.questList[dm.data.questNum]["GetItemIndex"].ToString();
        string getExp = dm.questList[dm.data.questNum]["GetExp"].ToString();
        if (getItem != "") inventory.AddItem(int.Parse(getItem));
        if (getExp != "") gm.Exp += float.Parse(getExp);

        // 보상에 대한 대화
        gm.CheckBubble();
    }

    // 공격 받음
    public void GetHit(float atk)
    {
        playerMove.isCantMove = false;
        anim.SetTrigger("getHit");

        // 방패를 들고 있다면
        if (gm.defImg.color == Color.white)
        {
            gm.Def -= atk * 0.01f;
            gm.Hp -= atk * 0.6f;
        }
        else gm.Hp -= atk;

        StartCoroutine(gm.HpImgColor(Color.red));
    }

    // 장비 장착
    public void EquipPutOn(int itemIdx)
    {
        print(itemIdx + "장비장착");
        switch (itemIdx)
        {
            // 칼
            case 0:
                if (hasWeapon == WeaponType.Bow)
                {
                    anim.SetTrigger("weaponChange");
                    anim.SetBool("isBow", false);
                    EquipPutOff(3);
                }
                sword.SetActive(true);
                hasWeapon = WeaponType.Sword; break;

            // 방패
            case 2:
                if (hasWeapon == WeaponType.Bow)
                {
                    anim.SetTrigger("weaponChange");
                    anim.SetBool("isBow", false);
                    EquipPutOff(3);
                    hasWeapon = WeaponType.None;
                }
                gm.defImg.color = Color.white;
                shield.SetActive(true); break;

            // 활
            case 3:
                if (hasWeapon == WeaponType.Sword)
                {
                    EquipPutOff(0);
                }
                if (shield.activeSelf)
                {
                    EquipPutOff(2);
                }
                anim.SetTrigger("weaponChange");
                anim.SetBool("isBow", true);
                bow.SetActive(true);
                arrow.SetActive(true);
                hasWeapon = WeaponType.Bow; break;
        }
    }

    // 장비 장착 해제
    public void EquipPutOff(int itemIdx)
    {
        print(itemIdx + "장비해제");
        inventory.EquipItemMove(itemIdx, false);

        switch (itemIdx)
        {
            // 칼
            case 0:
                sword.SetActive(false);
                break;

            // 방패
            case 2:
                gm.defImg.color = Color.clear;
                shield.SetActive(false); break;

            // 활
            case 3:
                bow.SetActive(false);
                arrow.SetActive(false); break;
        }
    }

}
