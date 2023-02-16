using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Item[] items;

    // 싱글톤
    public static ItemManager instance;
    private void Awake()
    {
        if (!instance) instance = this;
    }
}
