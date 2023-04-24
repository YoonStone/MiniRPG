using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class NPCInfo : ScriptableObject
{
    public int npcIndex;
    public string npcName;
    public string npcKoreanName;
    public int interactableNumber; // 상호작용이 가능해지는 퀘스트 번호
}
