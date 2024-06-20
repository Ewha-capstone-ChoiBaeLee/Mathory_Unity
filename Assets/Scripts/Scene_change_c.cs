using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_change_c : MonoBehaviour
{
    public void SceneChange()
    {
        LoadingSceneController.LoadScene("Story1");
    }
}