using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject nickName;

    void Start()
    {
        
    }

    void PlayerHI()
    {
        nickName.SetActive(true);
    }

    void PlayerBye()
    {
        nickName.SetActive(false);
    }

}
