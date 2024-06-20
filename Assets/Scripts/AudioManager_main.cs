using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager_main : MonoBehaviour
{
    private static AudioManager_main instance;

    void Awake()
    {
        // AudioManager_game이 Hierarchy 창에 존재한다면 destroy
        AudioManager_game gameManager = FindObjectOfType<AudioManager_game>();
        if (gameManager != null)
        {
            Destroy(gameManager.gameObject);
        }

        // 인스턴스가 이미 존재하면 새로 생성된 오브젝트를 파괴
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 이 오브젝트를 파괴하지 않고 유지
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
}