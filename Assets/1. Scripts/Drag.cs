using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    CameraTurn cameraTurn;
    bool isDrag;
    Vector3 originPos;

    private void Start()
    {
        cameraTurn = FindObjectOfType<CameraTurn>();
        originPos = transform.position;
    }

    // 슬롯의 아이템 클릭
    public void OnPointDown()
    {
        isDrag = true;
        cameraTurn.isCantTurn = true;
    }

    // 슬롯 드래그 종료
    public void OnPointUp()
    {
        if (!isDrag) return;
        isDrag = false;
        cameraTurn.isCantTurn = false;
        
        // 원래 자리로 이동
        transform.position = originPos;
    }

    private void Update()
    {
        // 드래그
        if (isDrag) transform.position = Input.mousePosition;
    }
}
