﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour
{
    // 퀵슬롯인지
    public bool isQuickSlot;

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

            if (!isQuickSlot)
            {
                if (foodBtn) foodBtn.Count = count;
                if (count == 0) ItemOut();
                else if (count == 1) countImg.SetActive(false);
                else if (count >= 2 && !countImg.activeSelf) countImg.SetActive(true);
            }
        }
    }

    Image slotImage;
    GameObject countImg;
    TextMeshProUGUI countTxt;
    InventoryManager inventory;
    GameManager gm;
    Drag drag;
    Image dragImg;
    CameraTurn cameraTurn;
    //[HideInInspector] 
    public Slot foodBtn;

    private void Awake()
    {
        slotImage = transform.GetChild(0).GetComponent<Image>();
        countImg = transform.GetChild(1).gameObject;
        countTxt = countImg.GetComponentInChildren<TextMeshProUGUI>();
        cameraTurn = FindObjectOfType<CameraTurn>();
    }

    private void Start()
    {
        inventory = InventoryManager.instance;
        gm = GameManager.instance;
        drag = inventory.drag;
        dragImg = drag.GetComponent<Image>();
    }

    // 슬롯 클릭하면 아이템 사용
    public void OnClick()
    {
        if (item == null || Count == 0) return;

        // 상점이 안 열려있다면 사용
        if (!inventory.isOpenShop) inventory.UseItem(this);

        // 상점이 열려있다면 판매
        else StartCoroutine(SellItem());
    }

    // 슬롯 위에 손이 올라오면
    public void OnEnter()
    {
        drag.isOnSlot = true;
        drag.pointEnterSlot = this;
    }

    // 슬롯 위에서 손이 떨어지면 
    public void OnExit()
    {
        drag.isOnSlot = false;
    }

    // 슬롯을 누르면
    public void OnDown()
    {
        if (item == null || gm.isPopup) return;
        StartCoroutine(DownCheck());
    }

    // 계속 누르고 있는지 확인
    IEnumerator DownCheck()
    {
        cameraTurn.enabled = false;

        // 0.5초 후에도 누르고 있다면
        yield return new WaitForSeconds(0.5f);
        if (drag.pointEnterSlot != this) yield break;

        dragImg.color = Color.clear;
        drag.gameObject.SetActive(true);
        drag.DragStart(this, item, count);
    }

    // 슬롯에서 손을 떼면
    public void OnUp()
    {
        cameraTurn.enabled = true;
    }

    // 슬롯에서 아이템 사라지는 경우
    void ItemOut()
    {
        countImg.SetActive(false);
        slotImage.sprite = null;
        slotImage.color = new Vector4(1, 1, 1, 0);
        item = null;
    }

    // 팝업창 사용 (아이템 판매)
    IEnumerator SellItem()
    {
        // 팔 수 없는 아이템이라면 실행 불가
        if (!item.isCanSell) yield break;

        bool isCanPopup = GameManager.instance.PopupOpen("이 아이템을 판매하시겠습니까?", "예", "아니오");

        // 이미 팝업창이 열려있었다면 실행 금지
        if (!isCanPopup) yield break;

        // 예/아니오를 누를 때까지 기다리기
        AudioManager.instance.AudioCtrl_SFX(SFX.EffectDown);
        GameManager.instance.popupState = PopupState.None;
        yield return new WaitUntil(() => GameManager.instance.popupState != PopupState.None);

        AudioManager.instance.AudioCtrl_SFX(SFX.EffectUp);

        // 예를 눌렀다면 아이템 판매
        if (GameManager.instance.popupState == PopupState.Left)
        {
            AudioManager.instance.AudioCtrl_SFX(ItemAudio.BuySell);
            gm.Gold += item.itemPrice;
            Count--;
        }

        GameManager.instance.popupState = PopupState.None;
    }
}
