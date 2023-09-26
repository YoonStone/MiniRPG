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

    BoxCollider swordColl;
    PlayerMove playerMove;
    Animator anim;
    Animator bowAnim;
    Animator arrowAnim;
    DataManager dm;
    GameManager gm;
    InventoryManager inventory;
    CharacterController cc;

    public Transform target;

    // 들고 있는 무기 종류
    public enum WeaponType
    {
        None,
        Sword,
        Bow
    }

    public WeaponType hasWeapon; // 들고 있는 무기

    // Start 함수보다 빨리 호출
    public void BeforeStart()
    {
        gm = GameManager.instance;
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        swordColl = sword.GetComponent<BoxCollider>();
        playerMove = GetComponent<PlayerMove>();
        bowAnim = bow.GetComponent<Animator>();
        arrowAnim = arrow.GetComponent<Animator>();
        cc = GetComponent<CharacterController>();

        dm = DataManager.instance;
        inventory = InventoryManager.instance;

        gm.actionBtn.onClick.AddListener(OnClickActionBtn);
        gm.skillBtn.onClick.AddListener(OnClickSkillBtn);
    }

    // PC용
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) OnClickActionBtn();
    }

    // 액션 버튼
    void OnClickActionBtn()
    {
        // NPC와 함께 있다면 상호작용
        if (withNpc) NPCInteract();

        // 무기를 들고 있다면 공격 
        else if(hasWeapon != WeaponType.None) Attack();
    }

    void Attack()
    {
        // 공격 중일 때는 공격 불가
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            return;

        // 공격력 수정
        dm.data.atk = gm.skills[(int)hasWeapon].actionAtk;

        // 이동 불가능 + 공격 애니메이션
        playerMove.isCantMove = true;
        playerMove.StopMove();

        anim.SetTrigger("attack");

        switch (hasWeapon)
        {
            case WeaponType.Sword:
                anim.SetInteger("attackIdx", Random.Range(0, 2));
                AudioManager.instance.AudioCtrl_SFX(AttackAudio.Sword);
                break;

            case WeaponType.Bow:
                anim.SetInteger("attackIdx", 2);
                bowAnim.SetTrigger("bowAttack");
                bowAnim.SetInteger("bowIdx", 2);
                arrowAnim.SetTrigger("arrowAttack");
                arrowAnim.SetInteger("arrowIdx", 2);
                break;
        }

        StartCoroutine(AttackEndCheck());
    }

    void OnClickSkillBtn()
    {
        // 공격 중이거나 무기가 없을 때는 스킬 공격 불가
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
            || hasWeapon == WeaponType.None)
            return;

        // 공격력 수정
        dm.data.atk = gm.skills[(int)hasWeapon].skillAtk;

        // 이동 불가능 + 공격 애니메이션
        playerMove.isCantMove = true;
        playerMove.StopMove();

        anim.SetTrigger("attack");

        switch (hasWeapon)
        {
            case WeaponType.Sword:
                anim.SetInteger("attackIdx", 3);
                AudioManager.instance.AudioCtrl_SFX(AttackAudio.SwordSpin);
                break;

            case WeaponType.Bow:
                anim.SetInteger("attackIdx", 4);
                bowAnim.SetTrigger("bowAttack");
                bowAnim.SetInteger("bowIdx", 4);
                arrowAnim.SetTrigger("arrowAttack");
                arrowAnim.SetInteger("arrowIdx", 4);
                break;
        }

        StartCoroutine(AttackEndCheck());
        StartCoroutine(gm.SkiilCoolTime((int)hasWeapon));
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
        // 퀘스트 진행 숫자 초기화 및 완료 표시
        dm.data.questItemCount = 0;
        gm.QuestCompleteUI(dm.data.questNum);

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
        // 방패를 들고 있다면
        if (gm.defImg.color == Color.white)
        {
            gm.Def -= atk * 0.01f;
            gm.Hp -= atk * 0.6f;
        }
        else gm.Hp -= atk;

        // *** 피 닳지 않도록
        gm.Hp = 100;

        StartCoroutine(gm.HpImgColor(Color.red));

        // 죽었다면(한 번만 실행하도록)
        if (gm.Hp <= 0 && !CompareTag("Untagged"))
        {
            StartCoroutine(Dead());
            return;
        }

        // 죽지 않았으면 피격만
        anim.SetTrigger("getHit");
        AudioManager.instance.AudioCtrl_SFX(PlayerAudio.GetHit);
    }

    // 죽음
    public IEnumerator Dead()
    {
        anim.SetTrigger("dead");
        AudioManager.instance.AudioCtrl_SFX(PlayerAudio.Dead);
        tag = "Untagged";

        // 액션 버튼, 스킬 버튼, 이동, 카메라 회전 비활성화
        gm.dontTouch.SetActive(true);
        playerMove.isCantMove = true;
        gm.cameraTurn.enabled = false;

        yield return new WaitForSeconds(1f);
        StartCoroutine(gm.SceneFade(Vector3.zero, Vector3.one, 1));
    }

    // 부활한 후 할 일
    public void ResetAll()
    {
        tag = "Player";
        gm.Hp = 100;

        // 액션 버튼, 스킬 버튼, 이동, 카메라 회전 활성화
        gm.dontTouch.SetActive(false);
        playerMove.isCantMove = false;
        gm.cameraTurn.enabled = true;

        cc.enabled = false;
        cc.transform.position = new Vector3(-5.5f, -6, -12);
        cc.transform.rotation = Quaternion.identity;
        cc.enabled = true;
    }

    // 장비 장착
    public void EquipPutOn(int itemIdx)
    {
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
                hasWeapon = WeaponType.Sword;

                // 액션, 스킬 이미지 변경
                gm.ChangeSkill(1);
                break;

            // 방패
            case 2:
                if (hasWeapon == WeaponType.Bow)
                {
                    anim.SetTrigger("weaponChange");
                    anim.SetBool("isBow", false);
                    EquipPutOff(3);
                    hasWeapon = WeaponType.None;

                    // 액션, 스킬 이미지 변경
                    gm.ChangeSkill(0);
                }
                gm.defImg.color = Color.white;
                shield.SetActive(true); break;

            // 활
            case 3:
                if (hasWeapon == WeaponType.Sword) EquipPutOff(0);
                if (shield.activeSelf) EquipPutOff(2);

                anim.SetTrigger("weaponChange");
                anim.SetBool("isBow", true);
                bow.SetActive(true);
                arrow.SetActive(true);
                hasWeapon = WeaponType.Bow;

                // 액션, 스킬 이미지 변경
                gm.ChangeSkill(2);
                break;
        }
    }

    // 장비 장착 해제
    public void EquipPutOff(int itemIdx)
    {
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
