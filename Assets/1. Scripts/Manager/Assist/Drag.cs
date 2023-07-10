using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drag : MonoBehaviour
{
    [HideInInspector] public Slot pointEnterSlot;
    [HideInInspector] public bool isOnSlot; // 슬롯 위에 손가락이 있는지

    CameraTurn cameraTurn;
    Image img;
    GameManager gm;
    Slot dragStartSlot;
    Item dragItem;

    int drgaItemCount;
    bool isDrag;

    private void Awake()
    {
        cameraTurn = FindObjectOfType<CameraTurn>();
        img = GetComponent<Image>();
    }

    private void Start()
    {
        gm = GameManager.instance;
    }

    private void OnEnable()
    {
        img.color = Color.clear;
        transform.position = Input.mousePosition;
    }

    private void Update()
    {
        // 드래그
        if (isDrag)
        {
            transform.position = Input.mousePosition;

            // 드래그 종료
            if (Input.GetMouseButtonUp(0))
            {
                DragEnd();

                // 다른 '비어있는' 슬롯 위에서 드롭한 경우
                if (pointEnterSlot && pointEnterSlot != dragStartSlot && pointEnterSlot.Item == null)
                {
                    pointEnterSlot.Item = dragItem;
                    pointEnterSlot.Count = drgaItemCount;
                    dragStartSlot.Count = 0;
                }

                // 슬롯이 아닌 곳에 드롭한 경우 + 버릴 수 있는 아이템인 경우
                else if(!pointEnterSlot && dragItem.isCanDrop)
                {
                    StartCoroutine(PopupCall_ItemDrop());
                }
            }
        }
    }

    // 드래그 시작
    public void DragStart(Slot _dragStartSlot, Item _dragItem, int _drgaItemCount)
    {
        transform.position = Input.mousePosition;

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
        pointEnterSlot = null;
        dragStartSlot = null;
        dragItem = null;
        drgaItemCount = 0;
    }

    // 팝업창 사용 (아이템 버리기)
    IEnumerator PopupCall_ItemDrop()
    {
        bool isCanPopup = gm.PopupOpen("이 아이템을 버리시겠습니까?", "예", "아니오");

        // 이미 팝업창이 열려있었다면 실행 금지
        if (!isCanPopup) yield break;

        // 예/아니오를 누를 때까지 기다리기
        AudioManager.instance.AudioCtrl_SFX(SFX.EffectDown);
        gm.popupState = PopupState.None;
        yield return new WaitUntil(() => gm.popupState != PopupState.None);

        AudioManager.instance.AudioCtrl_SFX(SFX.EffectUp);

        // 예를 눌렀다면 버리기
        if (gm.popupState == PopupState.Left)
            dragStartSlot.Count = 0;

        DragReset();
        gm.popupState = PopupState.None;
    }
}
