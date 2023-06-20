using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTurn : MonoBehaviour
{
    [Header("카메라 회전 속도")]
    public float turnSpeed;


    [HideInInspector] public CinemachineFreeLook cineFreeLook;
    //[HideInInspector] public Vector2 currentPos;

    private void Awake()
    {
        cineFreeLook = GetComponent<CinemachineFreeLook>();
    }

    private void OnDisable()
    {
        StopRotate();
    }

    void Update()
    {
        //// 드래그 중
        //if (isDrag)
        //{
        //    // 드래그 방향대로 카메라 회전
        //    //Vector2 dragDir = (Vector2)Input.GetTouch(touchId).position - currentPos;
        //    cineFreeLook.m_YAxis.m_InputAxisValue = rotateDir.y * turnSpeed;
        //    cineFreeLook.m_XAxis.m_InputAxisValue = rotateDir.x * turnSpeed;
        //    //currentPos = Input.GetTouch(touchId).position;
        //}
    }

    public void DoRotate(Vector3 rotateDir)
    {
        cineFreeLook.m_YAxis.m_InputAxisValue = rotateDir.y * turnSpeed;
        cineFreeLook.m_XAxis.m_InputAxisValue = rotateDir.x * turnSpeed;
    }

    public void StopRotate()
    {
        cineFreeLook.m_YAxis.m_InputAxisValue = 0;
        cineFreeLook.m_XAxis.m_InputAxisValue = 0;
    }
}
