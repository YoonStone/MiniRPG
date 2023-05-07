using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTurn : MonoBehaviour
{
    [Header("카메라 회전 속도")]
    public float turnSpeed;

    CinemachineFreeLook cineFreeLook;

    Vector2 currentPos;
    float screenWidthHalf;
    bool isDrag;

    private void Awake()
    {
        cineFreeLook = GetComponent<CinemachineFreeLook>();
        screenWidthHalf = Screen.width * 0.5f;
    }

    private void OnDisable()
    {
        isDrag = false;
        cineFreeLook.m_YAxis.m_InputAxisValue = 0;
        cineFreeLook.m_XAxis.m_InputAxisValue = 0;
    }

    void Update()
    {
        // 드래그 시작
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x > screenWidthHalf)
        {
            isDrag = true;
            currentPos = Input.mousePosition;
        }
        // 드래그 종료
        else if (Input.GetMouseButtonUp(0))
        {
            isDrag = false;
            cineFreeLook.m_YAxis.m_InputAxisValue = 0;
            cineFreeLook.m_XAxis.m_InputAxisValue = 0;
        }

        // 드래그 중
        if (isDrag)
        {
            // 드래그 방향대로 카메라 회전
            Vector2 dragDir = (Vector2)Input.mousePosition - currentPos;
            cineFreeLook.m_YAxis.m_InputAxisValue = dragDir.y * turnSpeed;
            cineFreeLook.m_XAxis.m_InputAxisValue = dragDir.x * turnSpeed;
            currentPos = Input.mousePosition;
        }
    }
}
