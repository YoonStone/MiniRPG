using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTurn : MonoBehaviour
{
    [Header("카메라 회전 속도")]
    public float turnSpeed;


    [HideInInspector] public CinemachineFreeLook cineFreeLook;
    [HideInInspector] public Vector2 currentPos;
    [HideInInspector] public bool isDrag;
    [HideInInspector] public int touchId;

    private void Awake()
    {
        cineFreeLook = GetComponent<CinemachineFreeLook>();
    }

    private void OnDisable()
    {
        isDrag = false;
        cineFreeLook.m_YAxis.m_InputAxisValue = 0;
        cineFreeLook.m_XAxis.m_InputAxisValue = 0;
    }

    void Update()
    {
        // 드래그 중
        if (isDrag)
        {
            // 드래그 방향대로 카메라 회전
            Vector2 dragDir = (Vector2)Input.GetTouch(touchId).position - currentPos;
            cineFreeLook.m_YAxis.m_InputAxisValue = dragDir.y * turnSpeed;
            cineFreeLook.m_XAxis.m_InputAxisValue = dragDir.x * turnSpeed;
            currentPos = Input.GetTouch(touchId).position;
        }
        else
        {
            cineFreeLook.m_YAxis.m_InputAxisValue = 0;
            cineFreeLook.m_XAxis.m_InputAxisValue = 0;
        }
    }
}
