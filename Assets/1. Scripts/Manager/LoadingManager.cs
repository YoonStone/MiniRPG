using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    // 로딩씬 거쳐서 씬 로드
    public static void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
