using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 액션 버튼 상호작용 종류
public enum ActionState
{
    None,
    WithNPC
}

public class PlayerAction : MonoBehaviour
{
    public ActionState actionState; // 상호작용 종류
    
    [HideInInspector]
    public string npcName; // 대화할 NPC의 이름

    PlayerMove playerMove;
    Animator anim;
    PlayUIManager uiMng;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        anim = GetComponent<Animator>();

        uiMng = FindObjectOfType<PlayUIManager>();
        uiMng.playerActionBtn.onClick.AddListener(OnClickPlayerActionBtn);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) OnClickPlayerActionBtn();
    }

    // 액션 버튼
    void OnClickPlayerActionBtn()
    {
        switch (actionState)
        {
            // 기본 검 공격 2가지 중 하나 실행
            case ActionState.None: Attack(Random.Range(0,2)); break;
            case ActionState.WithNPC: NPCInteract(); break;
        }
    }

    void Attack(int attackIdx)
    {
        // 이동 불가능 + 공격 애니메이션
        playerMove.isCantMove = true;
        playerMove.MoveEnd();
        anim.SetTrigger("Attack");
        anim.SetInteger("AttackIdx", attackIdx);

        StartCoroutine(AttackEndCheck());
    }

    // 공격 종료 (애니메이션이벤트)
    IEnumerator AttackEndCheck()
    {
        // 공격 애니메이션이 끝날 때까지 기다렸다가 이동 가능상태로 변경
        yield return new WaitUntil(() 
            => anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);
        playerMove.isCantMove = false;
    }

    // NPC와의 상호작용
    void NPCInteract()
    {
        // NPC 이름에 따라
        switch (npcName)
        {
            case "Boy": // 꼬마애 (페이드인 후 씬 전환)
                StartCoroutine(uiMng.Fade(0, 1)); break;
        }
    }
}
