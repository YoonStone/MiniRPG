using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drag : MonoBehaviour
{
    [HideInInspector]
    public Slot pointEnterSlot;

    CameraTurn cameraTurn;
    Image img;
    Slot dragStartSlot;
    Item dragItem;

    int drgaItemCount;
    bool isDrag;

    private void Awake()
    {
        cameraTurn = FindObjectOfType<CameraTurn>();
        img = GetComponent<Image>();
    }

    private void Update()
    {
        // 드래그
        if (isDrag)
        {
            transform.position = Input.mousePosition;

            // 드래그 종료 (나중에 터치아이디 사용해야 함**)
            if (Input.GetMouseButtonUp(0))
            {
                // 다른 '비어있는' 슬롯 위에서 드롭한 경우
                if (pointEnterSlot && pointEnterSlot != dragStartSlot && pointEnterSlot.Item == null)
                {
                    pointEnterSlot.Item = dragItem;
                    pointEnterSlot.Count = drgaItemCount;
                    dragStartSlot.ItemOut();
                }

                // 슬롯이 아닌 곳에 드롭한 경우
                else if(!pointEnterSlot)
                {
                    PlayUIManager.instance.PopupOpen(gameObject, "이 아이템을 버리시겠습니까?", "예", "아니오");
                }

                DragEnd();
            }
        }
    }

    // 드래그 시작
    public void DragStart(Slot _dragStartSlot, Item _dragItem, int _drgaItemCount)
    {
        dragStartSlot = _dragStartSlot;
        dragItem = _dragItem;
        drgaItemCount = _drgaItemCount;

        img.sprite = dragItem.itemSprite;
        img.color = Color.white;
        isDrag = true;
    }

    // 드래그 종료 시 공통적으로 할 일
    void DragEnd()
    {
        isDrag = false;
        cameraTurn.enabled = true;

        img.sprite = null;
        img.color = new Vector4(1, 1, 1, 0);
    }

    // 드래그 초기화
    void DragReset()
    {
        dragStartSlot = null;
        dragItem = null;
        drgaItemCount = 0;
    }

    void PopupCallback(bool isChoice1)
    {
        if(isChoice1) dragStartSlot.ItemOut();
        DragReset();
    }
}
