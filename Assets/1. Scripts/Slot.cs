using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
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

            if (!GetComponent<FoodUse>())
            {
                if (Item == foodBtn.Item) foodBtn.Count = count;
                if (count <= 1) countImg.SetActive(false);
                else if (count >= 2 && !countImg.activeSelf) countImg.SetActive(true);
            }
        }
    }

    Image slotImage;
    GameObject countImg;
    TextMeshProUGUI countTxt;
    Drag drag;
    Slot foodBtn;

    private void Start()
    {
        slotImage = transform.GetChild(0).GetComponent<Image>();
        countImg = transform.GetChild(1).gameObject;
        countTxt = countImg.GetComponentInChildren<TextMeshProUGUI>();
        drag = InventoryManager.instance.drag;
        foodBtn = InventoryManager.instance.foodBtn;
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

    // 슬롯 클릭
    public void OnDown()
    {
        if (item == null) return;
        drag.gameObject.SetActive(true);
        drag.DragStart(this, item, count);
    }

    // 슬롯에서 아이템 사라지는 경우
    public void ItemOut()
    {
        Count = 0;
        slotImage.sprite = null;
        slotImage.color = new Vector4(1, 1, 1, 0);
        item = null;
    }

    // 클릭했을 때
    // 1. 상점이 열려있으면 판매
    // 2. 상점이 안 열려 있으면 사용
}
