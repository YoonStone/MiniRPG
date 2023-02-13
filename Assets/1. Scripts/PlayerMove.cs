using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("-- 이동 관련 --")]
    public float moveSpeed;
    public float turnSpeed;

    Rigidbody rigid;
    Animator anim;
    Transform cam; // 팔로우 카메라

    Vector3 moveDir;    // 이동할 방향
    float turnVelocity; // 현재 회전 속도
    bool isMoving;      // 이동 중인지

    [HideInInspector]
    public bool isCantMove; // 이동 불가능한 상태인지

    [Header("-- 조이스틱 관련 --")]
    public GameObject joyStick;

    Transform stick;
    float joyStickRadiius; // 조이스틱 배경의 반지름
    float screenWidthHalf; // 화면의 절반
    bool isJoyStick;       // 조이스틱 사용 중인지

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        cam = Camera.main.transform;
        stick = joyStick.transform.GetChild(0);

        // 조이스틱 배경의 반지름, 화면의 절반 계산
        joyStickRadiius = joyStick.GetComponent<RectTransform>().rect.width * 0.5f;
        screenWidthHalf = Screen.width * 0.5f;
    }

    void Update()
    {
        if (isCantMove) return;

        moveDir = JoyStickMove();
        isMoving = moveDir.magnitude != 0;

        if (isMoving)
        {
            // 이동 애니메이션
            anim.SetFloat("velocity", moveDir.magnitude);
            Turn();
        }
    }

    private void FixedUpdate()
    {
        if (isCantMove) return;
        if (isMoving) Move();
    }

    // 조이스틱 담당
    Vector3 JoyStickMove()
    {
        // 드래그 시작
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x < screenWidthHalf)
        {
            isJoyStick = true;
            joyStick.SetActive(true);
            joyStick.transform.position = Input.mousePosition;
        }
        // 드래그 종료
        else if (Input.GetMouseButtonUp(0))
        {
            isJoyStick = false;
            joyStick.SetActive(false);
            MoveEnd();
        }

        // 드래그 중
        if (isJoyStick)
        {
            // 조이스틱 드래그
            stick.position = Input.mousePosition;
            stick.localPosition = Vector3.ClampMagnitude(stick.localPosition, joyStickRadiius);

            // 조이스틱 이동 방향 구하기
            Vector3 dir = stick.position - joyStick.transform.position;
            return dir.normalized;
        }
        else return Vector3.zero;        
    }

    // 이동 담당
    void Move() { rigid.velocity = transform.forward * moveSpeed;}

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
    public void MoveEnd()
    {
        anim.SetFloat("velocity", 0);
        rigid.velocity = Vector3.zero;
    }
}
