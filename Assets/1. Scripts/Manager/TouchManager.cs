using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TouchManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI text2;
    public CameraTurn cameraTurn;
    public PlayerMove playerMove;

    public GameObject joyStick_Move;
    public GameObject joyStick_Camera;

    Transform stick_Move;
    Transform stick_Camera;

    float joyStickRadiius_M; // 조이스틱 배경의 반지름
    float joyStickRadiius_C; // 조이스틱 배경의 반지름

    float screenWidthHalf; // 화면의 절반

    int moveId = -1;
    int cameraId = -1;

    void Start()
    {
        screenWidthHalf = Screen.width * 0.5f;

        stick_Move = joyStick_Move.transform.GetChild(0);
        stick_Camera = joyStick_Camera.transform.GetChild(0);

        // 조이스틱 배경의 반지름, 화면의 절반 계산
        joyStickRadiius_M = joyStick_Move.GetComponent<RectTransform>().rect.width * 0.5f;
        joyStickRadiius_C = joyStick_Camera.GetComponent<RectTransform>().rect.width * 0.5f;
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

                // 터치 시작
                if (touchPhase == TouchPhase.Began)
                {
                    if (EventSystem.current.IsPointerOverGameObject(i)) continue;

                    // 왼쪽 터치 = 조이스틱
                    if (touchPosX < screenWidthHalf)
                    {
                        moveId = fingerId;
                        joyStick_Move.SetActive(true);
                        joyStick_Move.transform.position = touch.position;
                        //text2.text += $"+joy({fingerId})\n";
                    }

                    // 오른쪽 터치 = 카메라 회전
                    else if (touchPosX >= screenWidthHalf)
                    {
                        cameraId = fingerId;
                        joyStick_Camera.SetActive(true);
                        joyStick_Camera.transform.position = touch.position;
                        //text2.text += $"+cam({fingerId})\n";
                    }
                }

                // 터치 종료
                else if (touchPhase == TouchPhase.Ended)
                {
                    if (fingerId == moveId)
                    {
                        // 더 낮은 번호의 손가락이 떨어지면 윗번호가 달라짐
                        if (moveId < cameraId)
                        {
                            //--cameraId;
                        }

                        moveId = -1;
                        joyStick_Move.SetActive(false);
                        playerMove.moveDir = Vector3.zero;
                        playerMove.StopMove();
                    }
                    else if (fingerId == cameraId)
                    {
                        // 더 낮은 번호의 손가락이 떨어지면 윗번호가 달라짐
                        if (cameraId < moveId)
                        {
                            //--moveId;
                        }

                        cameraId = -1;
                        joyStick_Camera.SetActive(false);
                        cameraTurn.StopRotate();
                    }
                }

                else if(touchPhase == TouchPhase.Moved && Input.touchCount == 1)
                {
                    text2.text = $"Alone&Move({fingerId})";
                }
            }
        }
        text.text = $"j({moveId}), c({cameraId})";

        // 켜져있는 조이스틱만 계산
        if(joyStick_Move.activeSelf) JoyStick_Move();
        if(joyStick_Camera.activeSelf) JoyStick_Camera();
    }

    //void JoystickContinue(int id)
    //{
    //    playerMove.touchId = id;
    //    playerMove.joyStick.transform.position = Input.GetTouch(id).position;
    //}

    //void CameraContinue(int id)
    //{
    //    cameraTurn.touchId = id;
    //    cameraTurn.currentPos = Input.GetTouch(id).position;
    //}

    // 이동용 조이스틱 결과물 전달
    void JoyStick_Move()
    {
        // 이동 조이스틱 조작 중
        if (moveId != -1)
        {
            // 드래그
            stick_Move.position = Input.GetTouch(moveId).position;
            stick_Move.localPosition = Vector3.ClampMagnitude(stick_Move.localPosition, joyStickRadiius_M);

            // 조이스틱 이동 방향 구하기
            Vector3 dir = stick_Move.position - joyStick_Move.transform.position;
            playerMove.moveDir = dir.normalized;    
        }
    }


    // 카메라 회전용 조이스틱 결과물 전달
    void JoyStick_Camera()
    {
        // 이동 조이스틱 조작 중
        if (cameraId != -1)
        {
            // 드래그
            stick_Camera.position = Input.GetTouch(cameraId).position;
            stick_Camera.localPosition = Vector3.ClampMagnitude(stick_Camera.localPosition, joyStickRadiius_C);

            // 조이스틱 이동 방향 구하기
            Vector3 dir = stick_Camera.position - joyStick_Camera.transform.position;
            cameraTurn.DoRotate(dir.normalized);
        }
    }
}
