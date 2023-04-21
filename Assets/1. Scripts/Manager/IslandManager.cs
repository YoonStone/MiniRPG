using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    private void Start()
    {
        // NPC 정보 넘겨주기
        GameManager.instance.npcs = FindObjectsOfType<NPC>();
    }
}
