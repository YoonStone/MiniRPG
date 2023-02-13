using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // 슬롯 부모들
    public Transform itemSlot, protectSlot;

    Slot[] itemSlots;
    Slot[] protectSlots;

    private void Start()
    {
        itemSlots = itemSlot.GetComponentsInChildren<Slot>();
        protectSlots = protectSlot.GetComponentsInChildren<Slot>();
    }

    // 아이템 추가
    public void AddItem(Item item)
    {
        foreach (var itemSlot in itemSlots)
        {
            // 같은 아이템을 가진 슬롯이 있다면 개수 변경
            if(itemSlot.Item == item)
            {
                CountChange(itemSlot, 1);
                return;
            }
        }

        foreach (var itemSlot in itemSlots)
        {
            // 내용물이 없는 슬롯에 추가
            if (itemSlot.Item == null)
            {
                itemSlot.Item = item;
                CountChange(itemSlot, 1);
                return;
            }
        }
    }

    // 슬롯의 개수 변경
    void CountChange(Slot slot, int change)
    {
        slot.Count += change;
    }
}
