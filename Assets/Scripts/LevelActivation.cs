using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class LevelData
{
    public int Level;
}

public class StarCount
{
    public string UserId;
    public int LevelOne;
    public int LevelTwo;
    public int LevelThree;
    public int LevelFour;
    public int LevelFive;
    public int LevelSix;
    public int LevelSeven;
    public int LevelEight;
    public int LevelNine;
    public int LevelTen;
    public int LevelEleven;
    public int LevelTwelve;
}

public class LevelActivation : MonoBehaviour
{

	string _baseUrl = "https://localhost:7039/api";

	public Button btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8, btn9, btn10, btn11, btn12;
    public int StarCount, gameLevel;
    public string PlayerId;

	// Start is called before the first frame update
	void Start()
	{
		int PlayerLevel = PlayerPrefs.GetInt("PlayerLevel");
        PlayerId = PlayerPrefs.GetString("PlayerId");

		UserInformation res = new UserInformation();
		SendGetRequest("User", res, PlayerLevel, (uwr) =>
		{
		});

        /*StarCount res2 = new StarCount();
        SendGetRequest("StarCount", res2, PlayerLevel, (uwr) =>
        {
        });*/
    }

	public void LevelOne()
	{
        int Level = 1;
        LevelData data = new LevelData { Level = Level };
        StartCoroutine(CoSendWebRequest("Quiz", "POST", data, OnServerResponse));
        PlayerPrefs.SetInt("GameLevel", 1);
    }

    public void LevelThree()
    {
        int Level = 3;
        LevelData data = new LevelData { Level = Level };
        StartCoroutine(CoSendWebRequest("Quiz", "POST", data, OnServerResponse));
        PlayerPrefs.SetInt("GameLevel", 2);
    }

    public void LevelFive()
    {
        int Level = 5;
        LevelData data = new LevelData { Level = Level };
        StartCoroutine(CoSendWebRequest("Quiz", "POST", data, OnServerResponse));
        PlayerPrefs.SetInt("GameLevel", 3);
    }

    public void LeveSeven()
    {
        int Level = 7;
        LevelData data = new LevelData { Level = Level };
        StartCoroutine(CoSendWebRequest("Quiz", "POST", data, OnServerResponse));
        PlayerPrefs.SetInt("GameLevel", 4);
    }

    public void LevelNine()
    {
        int Level = 9;
        LevelData data = new LevelData { Level = Level };
        StartCoroutine(CoSendWebRequest("Quiz", "POST", data, OnServerResponse));
        PlayerPrefs.SetInt("GameLevel", 5);
    }

    public void LevelEleven()
    {
        int Level = 11;
        LevelData data = new LevelData { Level = Level };
        StartCoroutine(CoSendWebRequest("Quiz", "POST", data, OnServerResponse));
        PlayerPrefs.SetInt("GameLevel", 6);
    }

    public void SceneChange()
    {
        SceneManager.LoadScene("CharacterList");
    }

    public void GetStarCount(string playerId, int level)
    {
        StarCount res2 = new StarCount();
        SendGetRequest2("StarCount", res2, playerId, level, (uwr) =>
        {
        });
    }

    public void SendGetRequest(string url, object obj, int playerLevel, Action<UnityWebRequest> callback)
	{
		StartCoroutine(CoSendWebRequest(url, "GET", obj, (uwr) =>
		{
			string responseJson = uwr.downloadHandler.text;

			List<UserInformation> userList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserInformation>>(responseJson);

			switch (playerLevel)
			{
				case 1:
					btn2.GetComponent<Button>().interactable = false;
					btn3.GetComponent<Button>().interactable = false;
					btn4.GetComponent<Button>().interactable = false;
					btn5.GetComponent<Button>().interactable = false;
					btn6.GetComponent<Button>().interactable = false;
					btn7.GetComponent<Button>().interactable = false;
					btn8.GetComponent<Button>().interactable = false;
					btn9.GetComponent<Button>().interactable = false;
					btn10.GetComponent<Button>().interactable = false;
                    btn11.GetComponent<Button>().interactable = false;
                    btn12.GetComponent<Button>().interactable = false;
                    GetStarCount(PlayerId, 1);
                    break;
				case 2:
					btn3.GetComponent<Button>().interactable = false;
					btn4.GetComponent<Button>().interactable = false;
					btn5.GetComponent<Button>().interactable = false;
					btn6.GetComponent<Button>().interactable = false;
					btn7.GetComponent<Button>().interactable = false;
					btn8.GetComponent<Button>().interactable = false;
					btn9.GetComponent<Button>().interactable = false;
					btn10.GetComponent<Button>().interactable = false;
                    btn11.GetComponent<Button>().interactable = false;
                    btn12.GetComponent<Button>().interactable = false;
                    GetStarCount(PlayerId, 1);
                    GetStarCount(PlayerId, 2);
                    break;
				case 3:
					btn4.GetComponent<Button>().interactable = false;
					btn5.GetComponent<Button>().interactable = false;
					btn6.GetComponent<Button>().interactable = false;
					btn7.GetComponent<Button>().interactable = false;
					btn8.GetComponent<Button>().interactable = false;
					btn9.GetComponent<Button>().interactable = false;
					btn10.GetComponent<Button>().interactable = false;
                    btn11.GetComponent<Button>().interactable = false;
                    btn12.GetComponent<Button>().interactable = false;
                    GetStarCount(PlayerId, 1);
                    GetStarCount(PlayerId, 2);
                    GetStarCount(PlayerId, 3);
                    break;
				case 4:
					btn5.GetComponent<Button>().interactable = false;
					btn6.GetComponent<Button>().interactable = false;
					btn7.GetComponent<Button>().interactable = false;
					btn8.GetComponent<Button>().interactable = false;
					btn9.GetComponent<Button>().interactable = false;
					btn10.GetComponent<Button>().interactable = false;
                    btn11.GetComponent<Button>().interactable = false;
                    btn12.GetComponent<Button>().interactable = false;
                    GetStarCount(PlayerId, 1);
                    GetStarCount(PlayerId, 2);
                    GetStarCount(PlayerId, 3);
                    GetStarCount(PlayerId, 4);
                    break;
                case 5:
					btn6.GetComponent<Button>().interactable = false;
					btn7.GetComponent<Button>().interactable = false;
					btn8.GetComponent<Button>().interactable = false;
					btn9.GetComponent<Button>().interactable = false;
					btn10.GetComponent<Button>().interactable = false;
                    btn11.GetComponent<Button>().interactable = false;
                    btn12.GetComponent<Button>().interactable = false;
                    GetStarCount(PlayerId, 1);
                    GetStarCount(PlayerId, 2);
                    GetStarCount(PlayerId, 3);
                    GetStarCount(PlayerId, 4);
                    GetStarCount(PlayerId, 5);
                    break;
				case 6:
					btn7.GetComponent<Button>().interactable = false;
					btn8.GetComponent<Button>().interactable = false;
					btn9.GetComponent<Button>().interactable = false;
					btn10.GetComponent<Button>().interactable = false;
                    btn11.GetComponent<Button>().interactable = false;
                    btn12.GetComponent<Button>().interactable = false;
                    GetStarCount(PlayerId, 1);
                    GetStarCount(PlayerId, 2);
                    GetStarCount(PlayerId, 3);
                    GetStarCount(PlayerId, 4);
                    GetStarCount(PlayerId, 5);
                    GetStarCount(PlayerId, 6);
                    break;
				case 7:
					btn8.GetComponent<Button>().interactable = false;
					btn9.GetComponent<Button>().interactable = false;
					btn10.GetComponent<Button>().interactable = false;
                    btn11.GetComponent<Button>().interactable = false;
                    btn12.GetComponent<Button>().interactable = false;
                    GetStarCount(PlayerId, 1);
                    GetStarCount(PlayerId, 2);
                    GetStarCount(PlayerId, 3);
                    GetStarCount(PlayerId, 4);
                    GetStarCount(PlayerId, 5);
                    GetStarCount(PlayerId, 6);
                    GetStarCount(PlayerId, 7);
                    break;
				case 8:
					btn9.GetComponent<Button>().interactable = false;
					btn10.GetComponent<Button>().interactable = false;
                    btn11.GetComponent<Button>().interactable = false;
                    btn12.GetComponent<Button>().interactable = false;
                    GetStarCount(PlayerId, 1);
                    GetStarCount(PlayerId, 2);
                    GetStarCount(PlayerId, 3);
                    GetStarCount(PlayerId, 4);
                    GetStarCount(PlayerId, 5);
                    GetStarCount(PlayerId, 6);
                    GetStarCount(PlayerId, 7);
                    GetStarCount(PlayerId, 8);
                    break;
                case 9:
					btn10.GetComponent<Button>().interactable = false;
                    btn11.GetComponent<Button>().interactable = false;
                    btn12.GetComponent<Button>().interactable = false;
                    GetStarCount(PlayerId, 1);
                    GetStarCount(PlayerId, 2);
                    GetStarCount(PlayerId, 3);
                    GetStarCount(PlayerId, 4);
                    GetStarCount(PlayerId, 5);
                    GetStarCount(PlayerId, 6);
                    GetStarCount(PlayerId, 7);
                    GetStarCount(PlayerId, 8);
                    GetStarCount(PlayerId, 9);
                    break;
				case 10:
                    btn11.GetComponent<Button>().interactable = false;
                    btn12.GetComponent<Button>().interactable = false;
                    GetStarCount(PlayerId, 1);
                    GetStarCount(PlayerId, 2);
                    GetStarCount(PlayerId, 3);
                    GetStarCount(PlayerId, 4);
                    GetStarCount(PlayerId, 5);
                    GetStarCount(PlayerId, 6);
                    GetStarCount(PlayerId, 7);
                    GetStarCount(PlayerId, 8);
                    GetStarCount(PlayerId, 9);
                    GetStarCount(PlayerId, 10);
                    break;
				case 11:
                    btn12.GetComponent<Button>().interactable = false;
                    GetStarCount(PlayerId, 1);
                    GetStarCount(PlayerId, 2);
                    GetStarCount(PlayerId, 3);
                    GetStarCount(PlayerId, 4);
                    GetStarCount(PlayerId, 5);
                    GetStarCount(PlayerId, 6);
                    GetStarCount(PlayerId, 7);
                    GetStarCount(PlayerId, 8);
                    GetStarCount(PlayerId, 9);
                    GetStarCount(PlayerId, 10);
                    GetStarCount(PlayerId, 11);
                    break;
				case 12:
                    GetStarCount(PlayerId, 1);
                    GetStarCount(PlayerId, 2);
                    GetStarCount(PlayerId, 3);
                    GetStarCount(PlayerId, 4);
                    GetStarCount(PlayerId, 5);
                    GetStarCount(PlayerId, 6);
                    GetStarCount(PlayerId, 7);
                    GetStarCount(PlayerId, 8);
                    GetStarCount(PlayerId, 9);
                    GetStarCount(PlayerId, 10);
                    GetStarCount(PlayerId, 11);
                    GetStarCount(PlayerId, 12);
                    break;
            }

			callback.Invoke(uwr);
		}));
	}

    public void SendGetRequest2(string url, object obj, string playerId, int level, Action<UnityWebRequest> callback)
    {
        StartCoroutine(CoSendWebRequest(url, "GET", obj, (uwr) =>
        {
            string responseJson = uwr.downloadHandler.text;

            List<StarCount> starList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StarCount>>(responseJson);

            foreach(StarCount star in starList)
            {
                if (star.UserId == playerId)
                {
                    switch (level)
                    {
                        case 1:
                            if (star.LevelOne == 1)
                            {
                                GameObject.Find("Canvas/levelOneButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                            }
                            else if (star.LevelOne == 2)
                            {
                                GameObject.Find("Canvas/levelOneButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelOneButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                            }
                            else if (star.LevelOne == 3)
                            {
                                GameObject.Find("Canvas/levelOneButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelOneButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelOneButton/starGrade").transform.Find("star3").gameObject.SetActive(true);
                            }
                            break;
                        case 2:
                            if (star.LevelTwo == 1)
                            {
                                GameObject.Find("Canvas/levelTwoButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                            }
                            else if (star.LevelTwo == 2)
                            {
                                GameObject.Find("Canvas/levelTwoButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelTwoButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                            }
                            else if (star.LevelTwo == 3)
                            {
                                GameObject.Find("Canvas/levelTwoButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelTwoButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelTwoButton/starGrade").transform.Find("star3").gameObject.SetActive(true);
                            }
                            break;
                        case 3:
                            if (star.LevelThree == 1)
                            {
                                GameObject.Find("Canvas/levelThreeButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                            }
                            else if (star.LevelThree == 2)
                            {
                                GameObject.Find("Canvas/levelThreeButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelThreeButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                            }
                            else if (star.LevelThree == 3)
                            {
                                GameObject.Find("Canvas/levelThreeButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelThreeButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelThreeButton/starGrade").transform.Find("star3").gameObject.SetActive(true);
                            }
                            break;
                        case 4:
                            if (star.LevelFour == 1)
                            {
                                GameObject.Find("Canvas/levelFourButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                            }
                            else if (star.LevelFour == 2)
                            {
                                GameObject.Find("Canvas/levelFourButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelFourButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                            }
                            else if (star.LevelFour == 3)
                            {
                                GameObject.Find("Canvas/levelFourButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelFourButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelFourButton/starGrade").transform.Find("star3").gameObject.SetActive(true);
                            }
                            break;
                        case 5:
                            if (star.LevelFive == 1)
                            {
                                GameObject.Find("Canvas/levelFiveButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                            }
                            else if (star.LevelFive == 2)
                            {
                                GameObject.Find("Canvas/levelFiveButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelFiveButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                            }
                            else if (star.LevelFive == 3)
                            {
                                GameObject.Find("Canvas/levelFiveButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelFiveButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelFiveButton/starGrade").transform.Find("star3").gameObject.SetActive(true);
                            }
                            break;
                        case 6:
                            if (star.LevelSix == 1)
                            {
                                GameObject.Find("Canvas/levelSixButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                            }
                            else if (star.LevelSix == 2)
                            {
                                GameObject.Find("Canvas/levelSixButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelSixButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                            }
                            else if (star.LevelSix == 3)
                            {
                                GameObject.Find("Canvas/levelSixButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelSixButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelSixButton/starGrade").transform.Find("star3").gameObject.SetActive(true);
                            }
                            break;
                        case 7:
                            if (star.LevelSeven == 1)
                            {
                                GameObject.Find("Canvas/levelSevenButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                            }
                            else if (star.LevelSeven == 2)
                            {
                                GameObject.Find("Canvas/levelSevenButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelSevenButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                            }
                            else if (star.LevelSeven == 3)
                            {
                                GameObject.Find("Canvas/levelSevenButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelSevenButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelSevenButton/starGrade").transform.Find("star3").gameObject.SetActive(true);
                            }
                            break;
                        case 8:
                            if (star.LevelEight == 1)
                            {
                                GameObject.Find("Canvas/levelEightButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                            }
                            else if (star.LevelEight == 2)
                            {
                                GameObject.Find("Canvas/levelEightButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelEightButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                            }
                            else if (star.LevelEight == 3)
                            {
                                GameObject.Find("Canvas/levelEightButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelEightButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelEightButton/starGrade").transform.Find("star3").gameObject.SetActive(true);
                            }
                            break;
                        case 9:
                            if (star.LevelNine == 1)
                            {
                                GameObject.Find("Canvas/levelNineButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                            }
                            else if (star.LevelNine == 2)
                            {
                                GameObject.Find("Canvas/levelNineButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelNineButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                            }
                            else if (star.LevelNine == 3)
                            {
                                GameObject.Find("Canvas/levelNineButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelNineButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelNineButton/starGrade").transform.Find("star3").gameObject.SetActive(true);
                            }
                            break;
                        case 10:
                            if (star.LevelTen == 1)
                            {
                                GameObject.Find("Canvas/levelTenButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                            }
                            else if (star.LevelTen == 2)
                            {
                                GameObject.Find("Canvas/levelTenButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelTenButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                            }
                            else if (star.LevelTen == 3)
                            {
                                GameObject.Find("Canvas/levelTenButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelTenButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelTenButton/starGrade").transform.Find("star3").gameObject.SetActive(true);
                            }
                            break;
                        case 11:
                            if (star.LevelEleven == 1)
                            {
                                GameObject.Find("Canvas/levelElevenButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                            }
                            else if (star.LevelEleven == 2)
                            {
                                GameObject.Find("Canvas/levelElevenButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelElevenButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                            }
                            else if (star.LevelEleven == 3)
                            {
                                GameObject.Find("Canvas/levelElevenButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelElevenButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelElevenButton/starGrade").transform.Find("star3").gameObject.SetActive(true);
                            }
                            break;
                        case 12:
                            if (star.LevelTwelve == 1)
                            {
                                GameObject.Find("Canvas/levelTwelveButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                            }
                            else if (star.LevelTwelve == 2)
                            {
                                GameObject.Find("Canvas/levelTwelveButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelTwelveButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                            }
                            else if (star.LevelTwelve == 3)
                            {
                                GameObject.Find("Canvas/levelTwelveButton/starGrade").transform.Find("star1").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelTwelveButton/starGrade").transform.Find("star2").gameObject.SetActive(true);
                                GameObject.Find("Canvas/levelTwelveButton/starGrade").transform.Find("star3").gameObject.SetActive(true);
                            }
                            break;

                    }
                }
            }

            callback.Invoke(uwr);
        }));
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

    void OnServerResponse(UnityWebRequest uwr)
    {
        Debug.Log("Server response: " + uwr.downloadHandler.text);
    }
}