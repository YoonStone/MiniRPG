using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
      
        StartCoroutine(PlayUIManager.instance.SceneFade(Vector3.one, Vector3.zero, -1));
    }

    // 마을로 돌아가기 버튼
    public void OnClickBackBtn()
    {
        StartCoroutine(PlayUIManager.instance.SceneFade(Vector3.zero, Vector3.one, 1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
