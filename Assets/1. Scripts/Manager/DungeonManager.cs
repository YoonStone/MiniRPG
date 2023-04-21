using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    CharacterController playerCC;
    Transform player;

    // 마을로 돌아가기 버튼
    public void OnClickBackBtn()
    {
        StartCoroutine(GameManager.instance.SceneFade(Vector3.zero, Vector3.one, 1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
