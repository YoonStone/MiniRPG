using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    private Item item;
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

            // 수량이 1개 이하면 안 보이게
            if (count <= 1) countImg.SetActive(false);
            // 수량이 2개가 되면 보이게
            else if(count == 2) countImg.SetActive(true);
            countTxt.text = count.ToString();
        }
    }

    Image slotImage;
    GameObject countImg;
    TextMeshProUGUI countTxt;

    private void Awake()
    {
        slotImage = transform.GetChild(0).GetComponent<Image>();
        countImg = transform.GetChild(1).gameObject;
        countTxt = countImg.GetComponentInChildren<TextMeshProUGUI>();
    }

    // 슬롯에서 아이템 사라지는 경우
    void ItemOut()
    {
        item = null;
        slotImage.sprite = null;
        slotImage.color = new Vector4(1, 1, 1, 0);
        Count = 0;
    }

    // 클릭했을 때
    // 1. 상점이 열려있으면 판매
    // 2. 상점이 안 열려 있으면 사용

    // 드래그했을 때
    // 1. 다른 슬롯 위에서 드랍 : 위치 이동
    // 2. 인벤토리 UI 밖에서 드랍 : 아이템 버리기
}
