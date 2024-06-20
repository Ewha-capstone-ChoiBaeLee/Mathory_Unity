using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class StarData
{
    public int Star;
    public string UserId;
    public int UserLevel;
}

[Serializable]
public class LevelData2
{
    public string UserId;
    public int UserLevel;
    public int Increment;
}

public class StoryManager4 : MonoBehaviour
{
    string _baseUrl = "https://localhost:7039/api";
    public Text Storytxt1;
    public Text Storytxt2;
    public Text Storytxt2_Name;
    public Sprite[] backgrounds;
    public string playerName, playerId;
    public int playerLevel, gameLevel;
    public int Num, star;
    public Image backgroundImage;
    //private int SceneNum = 4;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Canvas").transform.Find("chat1").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("chat2").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("btn_chat").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("btn_next").gameObject.SetActive(false);

        GameObject.Find("Canvas").transform.Find("result").gameObject.SetActive(false);

        Num = PlayerPrefs.GetInt("StoryLineNum3") - 1;
        star = PlayerPrefs.GetInt("star");
        Debug.Log(star);
        GetStory();
        RemoveCharacterImg();
        playerName = PlayerPrefs.GetString("PlayerName");
        playerId = PlayerPrefs.GetString("PlayerId");
        playerLevel = PlayerPrefs.GetInt("PlayerLevel");
        gameLevel = PlayerPrefs.GetInt("GameLevel");
    }

    public void SendGetRequest(string url, object obj, int Num, Action<UnityWebRequest> callback)
    {

        StartCoroutine(CoSendWebRequest(url, "GET", obj, (uwr) =>
        {

            string responseJson = uwr.downloadHandler.text;

            List<StoryLine> storylineList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StoryLine>>(responseJson);

            foreach (StoryLine storyline in storylineList)
            {
                if (storyline.Num == Num)
                {
                    if (storyline.Name == "설명")
                    {
                        CheckSceneNum(obj, storyline.Num, storyline.Part);
                        GameObject.Find("Canvas").transform.Find("chat1").gameObject.SetActive(true);
                        GameObject.Find("Canvas").transform.Find("chat2").gameObject.SetActive(false);
                        Debug.Log($"Story: {storyline.Story}");
                        string replacedString1 = storyline.Story.Replace("주인공", playerName);
                        Storytxt1.text = replacedString1;
                    }
                    else
                    {
                        CheckSceneNum(obj, storyline.Num, storyline.Part);
                        GameObject.Find("Canvas").transform.Find("chat1").gameObject.SetActive(false);
                        GameObject.Find("Canvas").transform.Find("chat2").gameObject.SetActive(true);
                        GetCharacterImg(storyline.Name);
                        Debug.Log($"Story: {storyline.Story} Name: {storyline.Name}");
                        string replacedString2 = storyline.Story.Replace("주인공", playerName);
                        Storytxt2.text = replacedString2;
                        if (storyline.Name == "주인공")
                        {
                            Storytxt2_Name.text = playerName;
                        }
                        else
                        {
                            Storytxt2_Name.text = storyline.Name;
                        }
                    }
                }
            }
            // 콜백 함수 호출
            callback.Invoke(uwr);
        }));
    }

    public void SendGetRequest2(string url, object obj, int Num, Action<UnityWebRequest> callback)
    {

        StartCoroutine(CoSendWebRequest(url, "GET", obj, (uwr) =>
        {

            string responseJson = uwr.downloadHandler.text;

            List<StoryLine> storylineList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StoryLine>>(responseJson);

            foreach (StoryLine storyline in storylineList)
            {
                if (storyline.Num == Num)
                {
                    GameObject.Find("Canvas").transform.Find("btn_chat").gameObject.SetActive(true);
                }

                if (storylineList.Count < Num)
                {
                    GameObject.Find("Canvas").transform.Find("btn_chat").gameObject.SetActive(false);
                    GameObject.Find("Canvas").transform.Find("btn_next").gameObject.SetActive(true);
                }
            }


            // 콜백 함수 호출
            callback.Invoke(uwr);
        }));
    }

    public void GetStory()
    {
        StoryLine res = new StoryLine();

        SendGetRequest("StoryLine", res, ++Num, (uwr) =>
        {
            //Debug.Log("");
        });

        RemoveCharacterImg();
    }

    public void GetResult()
    {
        GameObject.Find("Canvas").transform.Find("result").gameObject.SetActive(true);

        if (star == 0)
        {
            GameObject.Find("Canvas/result").transform.Find("Complete").gameObject.SetActive(true);
        }
        else if (star == 1)
        {
            GameObject.Find("Canvas/result/star").transform.Find("star1").gameObject.SetActive(true);
            GameObject.Find("Canvas/result").transform.Find("Complete").gameObject.SetActive(true);
        }
        else if (star == 2)
        {
            GameObject.Find("Canvas/result/star").transform.Find("star2").gameObject.SetActive(true);
            GameObject.Find("Canvas/result").transform.Find("Complete").gameObject.SetActive(true);
        }
        else
        {
            GameObject.Find("Canvas/result/star").transform.Find("star3").gameObject.SetActive(true);
            GameObject.Find("Canvas/result").transform.Find("LevelUp").gameObject.SetActive(true);
        }
        SendGameResults(playerId, playerLevel, gameLevel, 3, star);
    }

    public void SendGameResults(string userId, int playerLevel, int gameLevel, int solved_Num, int corrected_Num)
    {
        StartCoroutine(UpdateLevel(userId, playerLevel, gameLevel, solved_Num, corrected_Num));
        StartCoroutine(UpdateStar(userId, gameLevel, corrected_Num));
    }

    private IEnumerator UpdateLevel(string userId, int userLevel, int gameLevel, int solved_Num, int corrected_Num)
    {
        if (solved_Num == corrected_Num && userLevel == gameLevel)
        {
            var data = new LevelData2 { UserId = userId, UserLevel = userLevel, Increment = 1 };
            yield return CoSendWebRequest("User/update", "PUT", data, (uwr) =>
            {
            });

            if(userLevel != 12)
            {
                PlayerPrefs.SetInt("PlayerLevel", playerLevel + 1);
            }
        }
    }

    private IEnumerator UpdateStar(string userId, int gameLevel, int star)
    {
        var data = new StarData { UserId = userId, UserLevel = gameLevel, Star = star };
        yield return CoSendWebRequest("StarCount/update", "PUT", data, (uwr) =>
        {
        });
    }

    public void SceneChange()
    {
        SceneManager.LoadScene("Home");
        StartCoroutine(CoSendWebRequest("Delete", "DELETE", null, OnDeleteRequestCompleted));
    }

    public void CheckSceneNum(object obj, int num, int part)
    {
        SendGetRequest2("StoryLine", obj, ++num, (uwr) =>
        {
            //Debug.Log("");
        });
    }

    public void RemoveCharacterImg()
    {
        GameObject.Find("Canvas").transform.Find("Lucas").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Mark").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Sophia").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Lina").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Jessica").gameObject.SetActive(false);
    }

    public void GetCharacterImg(string name)
    {
        if (name == "루카스")
        {
            GameObject.Find("Canvas").transform.Find("Lucas").gameObject.SetActive(true);
        }
        else if (name == "마크")
        {
            GameObject.Find("Canvas").transform.Find("Mark").gameObject.SetActive(true);
        }
        else if (name == "소피아")
        {
            GameObject.Find("Canvas").transform.Find("Sophia").gameObject.SetActive(true);
        }
        else if (name == "리나")
        {
            GameObject.Find("Canvas").transform.Find("Lina").gameObject.SetActive(true);
        }
        else if (name == "제시카")
        {
            GameObject.Find("Canvas").transform.Find("Jessica").gameObject.SetActive(true);
        }
    }

    IEnumerator CoSendWebRequest(string url, string method, object obj, Action<UnityWebRequest> callback)
    {
        string sendUrl = $"{_baseUrl}/{url}/";

        byte[] jsonBytes = null;

        if (obj != null)
        {
            string jsonStr = JsonUtility.ToJson(obj);
            jsonBytes = Encoding.UTF8.GetBytes(jsonStr);
        }

        var uwr = new UnityWebRequest(sendUrl, method);
        uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            //Debug.Log("Recv " + uwr.downloadHandler.text);
            callback.Invoke(uwr);
        }
    }

    void OnDeleteRequestCompleted(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Delete request succeeded: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Delete request failed: " + request.error);
        }
    }
}