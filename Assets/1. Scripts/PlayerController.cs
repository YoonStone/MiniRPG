using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //왼손가락과 오른손가락에 입력값 저장
    int leftFingerId, rightfFingerId;
    //화면을 반으로 나눠서 오른쪽은 회전을, 왼쪽은 캐릭터 이동을 
    float halfScreenWidth;


    public Transform cameraTransform;

    //Rigidbody가 캐릭터에 적용되어 있다면 제거해 주세요
    public CharacterController charactercontroller;

    public float cameraSensitivity;
    public float moveSpeed;
    public float moveInputDeadZone;

    Vector2 lookInput;
    //pitch는 상하
    float cameraPitch;

    Vector2 moveTouchStartPosition;
    Vector2 moveInput;




    // Start is called before the first frame update
    void Start()
    {
        leftFingerId = -1;
        rightfFingerId = -1;
        halfScreenWidth = Screen.width / 2;
        moveInputDeadZone = Mathf.Pow(Screen.height / moveInputDeadZone, 2);
    }

    // Update is called once per frame
    void Update()
    {
        GetTouchInput();
        if (rightfFingerId != -1)
        {
            LookAround();
        }
        if (leftFingerId != -1)
        {
            Move();
        }

    }

    private void Move()
    {
        if (moveInput.sqrMagnitude <= moveInputDeadZone) return;
        Vector2 movementDirection = moveInput.normalized * moveSpeed * Time.deltaTime;
        charactercontroller.Move(transform.right * movementDirection.x + transform.forward * movementDirection.y);
    }

    void LookAround()
    {
        cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        transform.Rotate(transform.up, lookInput.x);
    }

    private void GetTouchInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            //Touch정보를 받아올수 있는 Touch
            Touch t = Input.GetTouch(i);
            switch (t.phase)
            {


                case TouchPhase.Began:
                    if (t.position.x < halfScreenWidth && leftFingerId == -1)
                    {
                        leftFingerId = t.fingerId;
                        moveTouchStartPosition = t.position;
                    }
                    else if (t.position.x > halfScreenWidth && rightfFingerId == -1)
                    {
                        rightfFingerId = t.fingerId;
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (t.fingerId == leftFingerId)
                    {
                        leftFingerId = -1;
                        Debug.Log("Stopped tracking left finger");
                    }
                    else if (t.fingerId == rightfFingerId)
                    {
                        rightfFingerId = -1;
                        Debug.Log("Stopped tracking right finger");
                    }
                    break;
                case TouchPhase.Moved:
                    if (t.fingerId == rightfFingerId)
                    {
                        lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                    }
                    else if (t.fingerId == leftFingerId)
                    {
                        moveInput = t.position - moveTouchStartPosition;
                    }
                    break;
                case TouchPhase.Stationary:
                    if (t.fingerId == rightfFingerId)
                    {
                        lookInput = Vector2.zero;
                    }
                    break;
            }
        }

    }
}