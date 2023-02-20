using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 액션 버튼 상호작용 종류
public enum ActionState
{
    Attack,
    WithNPC
}

public class PlayerAction : MonoBehaviour
{
    public ActionState actionState; // 상호작용 종류
    public BoxCollider swordColl;   // 칼 콜라이더
    
    [HideInInspector] public string npcName; // 대화할 NPC의 이름

    PlayerMove playerMove;
    Animator anim;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        anim = GetComponent<Animator>();

        PlayUIManager.instance.playerActionBtn.onClick.AddListener(OnClickPlayerActionBtn);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) InventoryManager.instance.AddItem(ItemManager.instance.items[1]);
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
        // 공격 중일 때 재공격 불가
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) return;

        // 이동 불가능 + 공격 애니메이션
        playerMove.isCantMove = true;
        playerMove.MoveEnd();
        anim.SetTrigger("attack");
        anim.SetInteger("attackIdx", attackIdx);

        StartCoroutine(AttackEndCheck());
    }

    // 공격 종료 (애니메이션이벤트)
    IEnumerator AttackEndCheck()
    {
        swordColl.enabled = true;
        
        // 공격 애니메이션이 끝날 때까지 기다렸다가 이동 가능상태로 변경
        yield return new WaitUntil(() 
            => anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
            && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);

        swordColl.enabled = false;
        playerMove.isCantMove = false;
    }

    // NPC와의 상호작용
    void NPCInteract()
    {
        // NPC 이름에 따라
        switch (npcName)
        {
            case "Boy": // 꼬마애 (페이드인 후 씬 전환)
                actionState = ActionState.Attack; npcName = "";
                StartCoroutine(PlayUIManager.instance.Fade(0, 1)); break;
        }
    }

    public void GetHit(float atk)
    {
        anim.SetTrigger("getHit");
        PlayUIManager.instance.Hp -= atk;
        StartCoroutine(PlayUIManager.instance.GetHitEffect());
    }
}
