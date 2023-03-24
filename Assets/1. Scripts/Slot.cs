using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour
{
    // 퀵슬롯인지
    public bool isQuitSlot;

    public Item item;
    public Item Item
    {
        get { return item; }

        // 슬롯에 아이템이 들어오면 할 일
        set
        { 
            item = value;
            slotImage.sprite = item.itemSprite;
            slotImage.color = Color.white;
        }
    }

    private int count; // 해당 슬롯의 아이템 개수
    public int Count
    {
        get { return count; }

        // 개수가 달라지면 할 일
        set
        {
            count = value;
            countTxt.text = count.ToString();

            if (!isQuitSlot)
            {
                if (Item == foodBtn.Item) foodBtn.Count = count;
                if (count == 0) ItemOut();
                else if (count == 1) countImg.SetActive(false);
                else if (count >= 2 && !countImg.activeSelf) countImg.SetActive(true);
            }
        }
    }

    Image slotImage;
    GameObject countImg;
    TextMeshProUGUI countTxt;
    Drag drag;
    Slot foodBtn;
    CameraTurn cameraTurn;

    private void Start()
    {
        slotImage = transform.GetChild(0).GetComponent<Image>();
        countImg = transform.GetChild(1).gameObject;
        countTxt = countImg.GetComponentInChildren<TextMeshProUGUI>();
        drag = InventoryManager.instance.drag;
        foodBtn = InventoryManager.instance.foodBtn;
        cameraTurn = FindObjectOfType<CameraTurn>();
    }

    // 슬롯 클릭하면 아이템 사용
    public void OnClick()
    {
        if (item == null || Count == 0) return;

        // 상점이 안 열려있다면 사용
        InventoryManager.instance.UseItem(this);

        // 상점이 열려있다면 판매
    }

    // 슬롯 위에 손이 올라오면
    public void OnEnter()
    {
        drag.pointEnterSlot = this;
    }

    // 슬롯 위에서 손이 떨어지면 
    public void OnExit()
    {
        drag.pointEnterSlot = null;
    }

    // 슬롯 드래그 시작
    public bool isDown;
    public void OnDown()
    {
        if (item == null || !item.isCanDrop || PlayUIManager.instance.isPopup) return;
        StartCoroutine(DownCheck());
    }

    IEnumerator DownCheck()
    {
        cameraTurn.enabled = false;
        isDown = true;

        // 0.5초 후에도 누르고 있다면
        yield return new WaitForSeconds(0.5f);
        if (!isDown) yield break;

        drag.gameObject.SetActive(true);
        drag.DragStart(this, item, count);
    }

    public void OnUp()
    {
        isDown = false;
        cameraTurn.enabled = true;
    }

    // 슬롯에서 아이템 사라지는 경우
    public void ItemOut()
    {
        count = 0;
        countImg.SetActive(false);
        slotImage.sprite = null;
        slotImage.color = new Vector4(1, 1, 1, 0);
        item = null;
    }
}
