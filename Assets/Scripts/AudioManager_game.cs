using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager_game : MonoBehaviour
{
    private static AudioManager_game instance;

    void Awake()
    {
        // AudioManager_main이 Hierarchy 창에 존재한다면 destroy
        AudioManager_main mainManager = FindObjectOfType<AudioManager_main>();
        if (mainManager != null)
        {
            Destroy(mainManager.gameObject);
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