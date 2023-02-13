using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTurn : MonoBehaviour
{
    CinemachineFreeLook cineFreeLook;

    Vector2 currentPos;
    float screenWidthHalf;
    bool isDrag;

    void Start()
    {
        cineFreeLook = GetComponent<CinemachineFreeLook>();
        screenWidthHalf = Screen.width * 0.5f;
    }

    void Update()
    {
        // �巡�� ����
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x > screenWidthHalf)
        {
            isDrag = true;
            currentPos = Input.mousePosition;
        }
        // �巡�� ����
        else if (Input.GetMouseButtonUp(0))
        {
            isDrag = false;
            cineFreeLook.m_YAxis.m_InputAxisValue = 0;
            cineFreeLook.m_XAxis.m_InputAxisValue = 0;
        }

        // �巡�� ��
        if (isDrag)
        {
            // �巡�� ������ ī�޶� ȸ��
            Vector2 dragDir = (Vector2)Input.mousePosition - currentPos;
            cineFreeLook.m_YAxis.m_InputAxisValue = dragDir.y;
            cineFreeLook.m_XAxis.m_InputAxisValue = dragDir.x;
            currentPos = Input.mousePosition;
        }
    }
}
