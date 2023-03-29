using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DropItem
{
    public GameObject itemPref; // 드랍 아이템
    public int dropCount_min;   // 최소 드랍 개수
    public int dropCount_max;   // 최대 드랍 개수
    public float percent;       // 드랍 확률
}

[CreateAssetMenu]
public class MonsterInfo : ScriptableObject
{
    [Header("체력")]
    public float hp_max;

    [Header("기본 공격력")]
    public float atk;

    [Header("처치 시 제공 경험치")]
    public float exp_min;
    public float exp_max;

    [Header("처치 시 제공 골드")]
    public int gold_min;
    public int gold_max;

    [Header("처치 시 제공 아이템")]
    public DropItem[] dropItems;

    [Header("플레이어와의 거리")]
    public float dist_Follow, dist_Attack;
}
