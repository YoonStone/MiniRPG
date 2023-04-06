using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public Transform playerPos;

    CharacterController playerCC;
    Transform player;
    PlayUIManager uiMng;

    void Start()
    {
        playerCC = FindObjectOfType<CharacterController>();

        //player = playerCC.transform;
        playerCC.enabled = false;
        playerCC.transform.position = playerPos.position;
        playerCC.transform.rotation = playerPos.rotation;
        playerCC.enabled = true;
      
        StartCoroutine(PlayUIManager.instance.Fade(Vector3.one, Vector3.zero));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
