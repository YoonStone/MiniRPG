using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Equipment,
    Food,
    Etc,
    Quest
}

[CreateAssetMenu]
public class Item : ScriptableObject
{
    [Header("아이템 고유 번호")]
    public int itemIdx;

    [Header("아이템 이름")]
    public string itemName;

    [Header("아이템 종류")]
    public ItemType itemType;

    [Header("아이템 설명")] [TextArea]
    public string itemInfo;

    [Header("아이템 이미지")]
    public Sprite itemSprite;

    [Header("아이템 가격")]
    public int itemPrice;

    [Header("버릴 수 있는지")]
    public bool isCanDrop;

    [Header("팔 수 있는지")]
    public bool isCanSell;
}
