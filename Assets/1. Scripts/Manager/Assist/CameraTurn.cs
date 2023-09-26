using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UIElements;

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

#if UNITY_EDITOR
    Vector2 position;
    Vector2 currentPos;
    void Update()
    {
        // 드래그 중
        if (Input.GetMouseButton(0))
        {
            // 드래그 방향 구해서 전달
            position = Input.mousePosition;
            Vector3 dir = position - currentPos;
            DoRotate(dir.normalized * 3f);

            currentPos = Input.mousePosition;

            // 드래그 방향대로 카메라 회전
            //Vector2 dragDir = (Vector2)Input.GetTouch(touchId).position - currentPos;
            //cineFreeLook.m_YAxis.m_InputAxisValue = rotateDir.y * turnSpeed;
            //cineFreeLook.m_XAxis.m_InputAxisValue = rotateDir.x * turnSpeed;
            //currentPos = Input.GetTouch(touchId).position;
        }
    }
#endif

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

    public float GetYAxisValue()
    {
        return cineFreeLook.m_YAxis.m_InputAxisValue;
    }


    public float GetXAxisValue()
    {
        return cineFreeLook.m_XAxis.m_InputAxisValue;
    }
}
