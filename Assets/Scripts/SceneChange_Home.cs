using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange_Home : MonoBehaviour
{
    void Start()
    {
        GameObject.Find("Canvas").transform.Find("Popup_Ranking").gameObject.SetActive(false);
    }

    public void SceneChange1()
    {
        SceneManager.LoadScene("MyPage");
    }

    public void SceneChange2()
    {
        SceneManager.LoadScene("LevelList");
    }

    public void SceneChange3()
    {
        GameObject.Find("Canvas").transform.Find("Popup_Ranking").gameObject.SetActive(true);
    }

    public void CloseRanking()
    {
        GameObject.Find("Canvas").transform.Find("Popup_Ranking").gameObject.SetActive(false);
    }
}
