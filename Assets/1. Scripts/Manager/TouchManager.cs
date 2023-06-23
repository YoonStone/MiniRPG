using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TouchManager : MonoBehaviour
{
    public CameraTurn cameraTurn;
    public PlayerMove playerMove;

    public GameObject joyStick_L;
    public GameObject joyStick_R;

    Transform stick_L;
    Transform stick_R;

    float joyStickRadiius_L; // 조이스틱 배경의 반지름
    float joyStickRadiius_R; // 조이스틱 배경의 반지름

    int leftFingerId = -1;
    int rightFingerId = -1;

    float screenWidthHalf; // 화면의 절반

    bool isWaitingLeft;  // 왼쪽 손가락이 나가서 기다리는 중인지
    bool isWaitingRight; // 오른쪽 손가락이 나가서 기다리는 중인지

    void Start()
    {
        // 화면의 절반 계산
        screenWidthHalf = Screen.width * 0.5f;

        stick_L = joyStick_L.transform.GetChild(0);
        stick_R = joyStick_R.transform.GetChild(0);

        // 조이스틱 배경의 반지름
        joyStickRadiius_L = joyStick_L.GetComponent<RectTransform>().rect.width * 0.5f;
        joyStickRadiius_R = joyStick_R.GetComponent<RectTransform>().rect.width * 0.5f;
   }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                int fingerId = touch.fingerId;
                float touchPosX = touch.position.x;
                TouchPhase touchPhase = touch.phase;

                switch (touchPhase)
                {
                    case TouchPhase.Began: // 터치 시작

                        // UI 예외처리
                        if (EventSystem.current.IsPointerOverGameObject(i)) continue;

                        // 왼쪽 터치 = 조이스틱 (드래그 중일 때는 제한)
                        if (touchPosX < screenWidthHalf && leftFingerId == -1)
                        {
                            leftFingerId = fingerId;
                            joyStick_L.SetActive(true);
                            joyStick_L.transform.position = touch.position;

                            // 빠졌던 손가락이 다시 들어온거라면
                            if (isWaitingLeft)
                            {
                                rightFingerId++;
                                isWaitingLeft = false;
                            }

                        }

                        // 오른쪽 터치 = 카메라 회전 (드래그 중일 때는 제한)
                        else if (touchPosX >= screenWidthHalf && rightFingerId == -1)
                        {
                            rightFingerId = fingerId;
                            joyStick_R.SetActive(true);
                            joyStick_R.transform.position = touch.position;

                            // 빠졌던 손가락이 다시 들어오면
                            if (isWaitingRight)
                            {
                                leftFingerId++;
                                isWaitingLeft = false;
                            }
                        }
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:

                        // 왼쪽 손가락 아웃
                        if (!isWaitingRight && fingerId == leftFingerId)
                        {
                            // 더 낮은 숫자가 빠진거라면
                            if (leftFingerId < rightFingerId)
                            {
                                rightFingerId--;
                                isWaitingLeft = true;
                            }

                            leftFingerId = -1;
                            joyStick_L.SetActive(false);
                            playerMove.moveDir = Vector3.zero;
                            playerMove.StopMove();
                        }

                        // 오른손이 먼저 나갔었고, 왼쪽 손가락 아웃
                        else if (isWaitingRight && fingerId == leftFingerId + 1)
                        {
                            isWaitingRight = false;

                            leftFingerId = -1;
                            joyStick_L.SetActive(false);
                            playerMove.moveDir = Vector3.zero;
                            playerMove.StopMove();
                        }

                        // 오른쪽 손가락 아웃
                        else if (!isWaitingRight && fingerId == rightFingerId)
                        {
                            // 더 낮은 숫자가 빠진거라면
                            if (rightFingerId < leftFingerId)
                            {
                                leftFingerId--;
                                isWaitingRight = true;
                            }

                            rightFingerId = -1;
                            joyStick_R.SetActive(false);
                            cameraTurn.StopRotate();
                        }

                        // 왼손이 먼저 나갔었고, 오른쪽 손가락 아웃
                        else if (isWaitingLeft && fingerId == rightFingerId + 1)
                        {
                            isWaitingLeft = false;

                            rightFingerId = -1;
                            joyStick_R.SetActive(false);
                            cameraTurn.StopRotate();
                        }
                        break;
                }

            }
        }
        else
        {
            leftFingerId = -1;
            rightFingerId = -1;
        }
        // 켜져있는 조이스틱만 계산, 꺼져있는데 중단되지 않았다면 중단
        if (leftFingerId != -1) JoyStick_Move();
        else if (playerMove.moveDir != Vector3.zero) playerMove.moveDir = Vector3.zero;

        if (rightFingerId != -1) JoyStick_Camera();
        else if (cameraTurn.GetYAxisValue() != 0 || cameraTurn.GetXAxisValue() != 0) cameraTurn.StopRotate();
    }

    // 이동용 조이스틱 결과물 전달
    void JoyStick_Move()
    {
        // 드래그
        stick_L.position = Input.GetTouch(leftFingerId).position;
        stick_L.localPosition = Vector3.ClampMagnitude(stick_L.localPosition, joyStickRadiius_L);

        // 조이스틱 이동 방향 구해서 전달
        Vector3 dir = stick_L.position - joyStick_L.transform.position;
        playerMove.moveDir = dir.normalized;
    }


    // 카메라 회전용 조이스틱 결과물 전달
    void JoyStick_Camera()
    {
        // 드래그
        stick_R.position = Input.GetTouch(rightFingerId).position;
        stick_R.localPosition = Vector3.ClampMagnitude(stick_R.localPosition, joyStickRadiius_R);

        // 조이스틱 이동 방향 구해서 전달
        Vector3 dir = stick_R.position - joyStick_R.transform.position;
        cameraTurn.DoRotate(dir.normalized);
    }
}
