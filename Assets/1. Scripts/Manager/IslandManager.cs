using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    // 처음 플레이할 때만 빵 아이템 생성
    public GameObject firstItems;
    public void CreateItem()
    {
        firstItems.SetActive(true);
    }
}
