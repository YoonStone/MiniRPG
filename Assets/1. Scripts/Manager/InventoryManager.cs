﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // 슬롯 부모들
    public Transform itemSlot, equipSlot;
    public Slot foodBtn;
    public Drag drag;
    public Item[] items;
    

    [HideInInspector]
    public Slot[] itemSlots;
    public Slot[] equipSlots;

    [HideInInspector]
    public int questItemCount;

    [HideInInspector]
    public bool isOpenShop; // 상점이 열려있는지

    // 싱글톤
    public static InventoryManager instance;
    private void Awake()
    {
        if(!instance) instance = this;
    }

    private void Start()
    {
        itemSlots = itemSlot.GetComponentsInChildren<Slot>();
        equipSlots = equipSlot.GetComponentsInChildren<Slot>();

        AddItem(0); // 칼 지급
        //AddItem(2); // 칼 지급
        //AddItem(3); // 칼 지급
    }

    // 아이템 추가 (아이템)
    public void AddItem(Item item)
    {
        // 퀘스트용 아이템
        if (item.itemType == ItemType.Quest)
            questItemCount++;

        foreach (var itemSlot in itemSlots)
        {
            // 같은 아이템을 가진 슬롯이 있다면 개수 변경
            if(itemSlot.Item == item)
            {
                itemSlot.Count += 1;
                return;
            }
        }

        foreach (var itemSlot in equipSlots)
        {
            // 같은 아이템을 가진 슬롯이 있다면 개수 변경
            if (itemSlot.Item == item)
            {
                itemSlot.Count += 1;
                return;
            }
        }

        foreach (var itemSlot in itemSlots)
        {
            // 내용물이 없는 슬롯에 추가
            if (itemSlot.Item == null)
            {
                itemSlot.Item = item;
                itemSlot.Count = 1;
                return;
            }
        }
    }

    // 아이템 추가 (번호)
    public void AddItem(int itemIdx)
    {
        foreach (var itemSlot in itemSlots)
        {
            // 같은 아이템을 가진 슬롯이 있다면 개수 변경
            if (itemSlot.Item == items[itemIdx])
            {
                itemSlot.Count += 1;
                return;
            }
        }

        foreach (var itemSlot in equipSlots)
        {
            // 같은 아이템을 가진 슬롯이 있다면 개수 변경
            if (itemSlot.Item == items[itemIdx])
            {
                itemSlot.Count += 1;
                return;
            }
        }

        foreach (var itemSlot in itemSlots)
        {
            // 내용물이 없는 슬롯에 추가
            if (itemSlot.Item == null)
            {
                itemSlot.Item = items[itemIdx];
                itemSlot.Count = 1;
                return;
            }
        }
    }

    // 아이템 찾기 (번호)
    public Slot FindItemSlot(int itemIdx, bool isEquip = false)
    {
        //// 장비 아이템인지 일반 아이템인지
        //Slot[] slots = isEquip ? equipSlots : itemSlots;

        foreach (var itemSlot in itemSlots)
        {
            // 같은 아이템을 가진 슬롯이 있다면 개수 변경
            if (itemSlot.Item == items[itemIdx])
            {
                return itemSlot;
            }
        }

        foreach (var itemSlot in equipSlots)
        {
            // 같은 아이템을 가진 슬롯이 있다면 개수 변경
            if (itemSlot.Item == items[itemIdx])
            {
                return itemSlot;
            }
        }

        //foreach (var itemSlot in slots)
        //{
        //    // 같은 아이템을 가진 슬롯이 있다면 슬롯 반환
        //    if (itemSlot.Item == items[itemIdx])
        //    {
        //        return itemSlot;
        //    }
        //}

        return null;
    }

    // 장비 착용/해제
    public void EquipItemMove(int itemIdx, bool isPutOn)
    {
        // 장비를 착용하는건지 해제하는건지
        Slot[] fromSlots = isPutOn ? itemSlots : equipSlots;
        Slot[] toSlots = isPutOn ? equipSlots : itemSlots;

        Slot toSlot = null, fromSlot = null;

        foreach (var slot in fromSlots)
        {
            // 같은 아이템을 가진 슬롯이 있다면
            if (slot.Item == items[itemIdx])
            {
                fromSlot = slot;
                break;
            }
        }

        foreach (var slot in toSlots)
        {
            // 내용물이 없는 슬롯이 있다면
            if (slot.Item == null)
            {
                toSlot = slot;
                break;
            }
        }

        // 아이템 옮기기
        toSlot.Item = fromSlot.Item;
        toSlot.Count = fromSlot.Count;
        fromSlot.Count = 0;
    }

    // 아이템 사용
    public void UseItem(Slot slot)
    {
        switch (slot.Item.itemType)
        {
            case ItemType.Equipment:
                PlayUIManager.instance.player.Equip(slot.Item.itemName);
                break;

            // 음식은 체력이 100보다 작을 때만 섭취 가능
            case ItemType.Food:
                if (PlayUIManager.instance.Hp >= 100) return;
                Eat(slot.Item.itemName);
                if (slot.isQuickSlot) UseQuickSlot(slot);

                if (slot.Count == 1 && !slot.isQuickSlot) slot.Count = 0;
                else slot.Count--;
                break;
        }

    }

    // 아이템 판매
    public void SellItem(Slot slot)
    {
        Debug.Log("아이템 판매");
    }

    // 음식 섭취
    void Eat(string itemName)
    {
        float changeHp = 0;
        switch (itemName)
        {
            case "Bread": changeHp = 10; break;
        }

        PlayUIManager.instance.Hp += changeHp;
        StartCoroutine(PlayUIManager.instance.HpImgColor(Color.green));
    }

    //  퀵슬롯 사용 시 인벤토리 슬롯 개수 맞추기
    void UseQuickSlot(Slot quitSlot)
    {
        foreach (var itemSlot in itemSlots)
        {
            // 같은 아이템을 가진 슬롯이 있다면 개수 변경
            if (itemSlot.Item == quitSlot.Item)
            {
                itemSlot.Count = quitSlot.Count;
                return;
            }
        }
    }
}
