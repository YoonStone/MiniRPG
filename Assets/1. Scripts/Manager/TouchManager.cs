using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TouchManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public CameraTurn cameraTurn;
    public PlayerMove playerMove;

    float screenWidthHalf; // 화면의 절반

    int joyStickId = -1;
    int cameraTurnId = -1;

    void Start()
    {
        screenWidthHalf = Screen.width * 0.5f;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                float touchPosX = touch.position.x;
                TouchPhase touchPhase = touch.phase;

                // 터치 시작
                if(touchPhase == TouchPhase.Began)
                {
                    // 왼쪽 터치 = 조이스틱
                    if (touchPosX < screenWidthHalf && !playerMove.isJoyStick)
                    {
                        joyStickId = i;

                        playerMove.touchId = i;
                        playerMove.isJoyStick = true;
                        playerMove.joyStick.SetActive(true);
                        playerMove.joyStick.transform.position = touch.position;
                    }

                    // 오른쪽 터치 = 카메라 회전
                    else if (touchPosX >= screenWidthHalf && !cameraTurn.isDrag)
                    {
                        cameraTurnId = i;

                        cameraTurn.touchId = i;
                        cameraTurn.isDrag = true;
                        cameraTurn.currentPos = touch.position;
                    }
                }

                // 터치 종료
                else if(touchPhase == TouchPhase.Ended)
                {
                    if (i == joyStickId)
                    {
                        joyStickId = -1;
                        playerMove.isJoyStick = false;
                        playerMove.joyStick.SetActive(false);
                        playerMove.MoveEnd();

                        // 더 낮은 번호의 손가락이 떨어지면 윗번호가 달라짐
                        if (joyStickId < cameraTurnId) cameraTurnId--;
                    }
                    else if (i == cameraTurnId)
                    {
                        cameraTurnId = -1;
                        cameraTurn.isDrag = false;
                        cameraTurn.cineFreeLook.m_YAxis.m_InputAxisValue = 0;
                        cameraTurn.cineFreeLook.m_XAxis.m_InputAxisValue = 0;

                        // 더 낮은 번호의 손가락이 떨어지면 윗번호가 달라짐
                        if (cameraTurnId < joyStickId) joyStickId--;
                    }
                }
            }
        }
        text.text = $"j({joyStickId}):{playerMove.isJoyStick}, c({cameraTurnId}):{cameraTurn.isDrag}";
    }
}
