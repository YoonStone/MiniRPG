using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    // 슬롯 부모들
    public Transform itemSlot, protectSlot;
    public Slot foodBtn;
    public Drag drag;
    public Item[] items;

    [HideInInspector]
    public Slot[] itemSlots;
    Slot[] protectSlots;

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
        protectSlots = protectSlot.GetComponentsInChildren<Slot>();

        AddItem(0); // 칼 지급
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

    // 아이템 사용
    public void UseItem(Slot slot)
    {
        switch (slot.Item.itemType)
        {
            case ItemType.Equipment: PlayUIManager.instance.player.
                    Equip(slot.Item.itemName);  break;

            // 음식은 체력이 100보다 작을 때만 섭취 가능
            case ItemType.Food: if (PlayUIManager.instance.Hp >= 100) return;
                Eat(slot.Item.itemName); break;

            default: return;
        }

        if (slot.Count == 1 && !slot.isQuickSlot) slot.Count = 0;
        else slot.Count--;

        if (slot.isQuickSlot && slot.Item.itemType == ItemType.Food) UseQuitSlot(slot);
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
    void UseQuitSlot(Slot quitSlot)
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
