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

    BoxCollider swordColl;   // 칼 콜라이더
    PlayerMove playerMove;
    Animator anim;
    Animator bowAnim;
    Animator arrowAnim;
    DataManager dm;
    PlayUIManager manager;

    GameObject[] arrowPool;
    NPC[] npcs;

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
        manager = PlayUIManager.instance;


        manager.playerActionBtn.onClick.AddListener(OnClickPlayerActionBtn);

        for (int i = 0; i < PlayUIManager.instance.skills.Length; i++)
        {
            int ii = i;
            manager.skills[ii].skillBtn.onClick.AddListener(() => Attack(2 + ii));
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

        npcs = FindObjectsOfType<NPC>();
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
        dm.data.atk = attackIdx <= 1 ? 10 : manager.skills[attackIdx - 2].skillAtk;

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
            StartCoroutine(manager.SkiilCoolTime(attackIdx - 2));
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

        switch (withNpc.npcQuestState)
        {
            case NPC.NPCQuestState.Have: // 퀘스트가 있음
                manager.ChatBubbleOpen(); break;

            case NPC.NPCQuestState.Wait: // 퀘스트 완료를 기다리는 중
                if (dm.data.questState == QuestState.Complete) QuestComplete();

                // 퀘스트를 기다리는 중이지만 완료하지 못했을 때 상호작용 시도
                else if (withNpc.npcName == "Merchant" || withNpc.npcName == "Boy")
                    withNpc.SendMessage("Interact");
                break;

            // 상호작용 가능한 상태라면 상호작용 시도
            default: if(withNpc.isInteractable) withNpc.SendMessage("Interact"); break;
        }

        // 상인은
        // 퀘스트 전 : 사용 불가
        // 퀘스트 있음 : 대화창
        // 퀘스트 수락 : 상점
        // 퀘스트 완료 : 대화창
        // 퀘스트 이후 : 상점

        // 남자아이는 던전 씬으로 전환 (npc 연결 해제해야 함)
        //StartCoroutine(PlayUIManager.instance.Fade(0, 1)); break;
    }

    // 퀘스트 완료 행동
    void QuestComplete()
    {
        // 보상 받기
        string getItem = dm.chatList[dm.data.chatNum]["GetItem"].ToString();
        string getExp = dm.chatList[dm.data.chatNum]["GetExp"].ToString();
        if (getItem != "") InventoryManager.instance.AddItem(int.Parse(getItem));
        if (getExp != "") manager.Exp += float.Parse(getExp);

        // 다음 대화
        dm.data.chatNum++;
        dm.data.questNum++;
        dm.data.questState = QuestState.None;
        manager.ChatBubbleOpen();

        // 모든 Npc에게 방금 퀘스트가 완료되었음을 전달
        foreach (var npc in npcs)
        {
            npc.SendMessage("SetQuestState");
        }
    }

    // 공격 받음
    public void GetHit(float atk)
    {
        playerMove.isCantMove = false;
        anim.SetTrigger("getHit");

        // 방패를 들고 있다면
        if (manager.defImg.color == Color.white)
        {
            manager.Def -= atk * 0.01f;
            manager.Hp -= atk * 0.6f;
        }
        else manager.Hp -= atk;

        StartCoroutine(manager.HpImgColor(Color.red));
    }

    // 장비 장착
    public void Equip(string itemName)
    {
        switch (itemName)
        {
            case "Sword":
                if (hasWeapon == WeaponType.Bow)
                {
                    bow.SetActive(false);
                    arrow.SetActive(false);
                    anim.SetTrigger("weaponChange");
                    anim.SetBool("isBow", false);
                    InventoryManager.instance.EquipItemMove(3, false);
                }
                sword.SetActive(true);
                InventoryManager.instance.EquipItemMove(0, true);
                hasWeapon = WeaponType.Sword; break;

            case "Shield":
                if (hasWeapon == WeaponType.Bow)
                {
                    bow.SetActive(false);
                    arrow.SetActive(false);
                    anim.SetTrigger("weaponChange");
                    anim.SetBool("isBow", false);
                    InventoryManager.instance.EquipItemMove(3, false);
                    hasWeapon = WeaponType.None;
                }
                shield.SetActive(true);
                InventoryManager.instance.EquipItemMove(2, true);
                manager.defImg.color = Color.white;
                break;

            case "Bow":
                if (hasWeapon == WeaponType.Sword)
                {
                    sword.SetActive(false);
                    InventoryManager.instance.EquipItemMove(0, false);
                }
                if (shield.activeSelf)
                {
                    shield.SetActive(false);
                    InventoryManager.instance.EquipItemMove(2, false);
                    manager.defImg.color = Color.clear;
                }
                anim.SetTrigger("weaponChange");
                anim.SetBool("isBow", true);
                bow.SetActive(true);
                arrow.SetActive(true);
                InventoryManager.instance.EquipItemMove(3, true);
                hasWeapon = WeaponType.Bow; break;
        }
    }
}
