using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("-- 이동 관련 --")]
    public float moveSpeed;
    public float turnSpeed;
    public float gravityScale;

    CharacterController cc;
    Animator anim;
    Transform cam; // 팔로우 카메라

    float turnVelocity; // 현재 회전 속도
    bool isMoving;      // 이동 중인지

    [HideInInspector] public Vector3 moveDir; // 이동할 방향
    [HideInInspector] public bool isCantMove; // 이동 불가능한 상태인지

    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        if (isCantMove)
        {
            StopMove();
            return;
        }

        //float h = Input.GetAxisRaw("Horizontal");
        //float v = Input.GetAxisRaw("Vertical");
        //moveDir = new Vector3(h, 0, v).normalized * 5;
        anim.SetFloat("velocity", moveDir.magnitude);

        isMoving = moveDir.magnitude != 0;
        if (isMoving)
        {
            // 이동 애니메이션
            anim.SetFloat("velocity", moveDir.magnitude);
            AudioManager.instance.AudioCtrl_Walk(true);
            Turn();
        }
    }

    private void FixedUpdate()
    {
        Vector3 moveDir = transform.forward;

        // 이동하지 않는 상태에서는 
        if (isCantMove || !isMoving) moveDir = Vector3.zero;

        // 이동하지 않아도 중력은 작동
        moveDir.y -= gravityScale;
        cc.Move(moveDir * moveSpeed);
    }

    // 회전 담당
    void Turn()
    {
        // 카메라를 기준으로 이동하려는 방향 서서히 바라보기
        moveDir = cam.transform.TransformDirection(moveDir).normalized;
        Quaternion rot = Quaternion.LookRotation(moveDir); rot.x = 0; rot.z = 0; // y축만 회전
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rot.eulerAngles.y,
                                ref turnVelocity, turnSpeed);
    }

    // 이동 종료
    public void StopMove()
    {
        anim.SetFloat("velocity", 0);
    }
}
