using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_change_l : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("CharacterList");
    }
}
