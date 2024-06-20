using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class WebManager : MonoBehaviour
{
    string _baseUrl = "https://localhost:7039/api";

    public InputField playerIdInput;
    public Button btn_start;
        
    // Start is called before the first frame update
    void Start()
    {
        btn_start.onClick.AddListener(GetPlayerId);
    }

	public void SendGetRequest(string url, object obj, string playerId, Action<UnityWebRequest> callback)
	{

		StartCoroutine(CoSendWebRequest(url, "GET", obj, (uwr) =>
		{

			string responseJson = uwr.downloadHandler.text;

			List<UserInformation> userList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserInformation>>(responseJson);

			foreach (UserInformation user in userList)
			{
				if (user.UserId == playerId)
				{
					Debug.Log($"UserId: {user.UserId}, UserLevel: {user.UserLevel}, UserName: {user.UserName} ");
					PlayerPrefs.SetInt("PlayerLevel", user.UserLevel);
                    PlayerPrefs.SetInt("PlayerYear", user.UserYear);
					PlayerPrefs.SetString("PlayerName", user.UserName);
				}
			}

			// 콜백 함수 호출
			callback.Invoke(uwr);
		}));
	}

	// Update is called once per frame
	void Update()
    {

    }

    void GetPlayerId()
    {
        string playerId = playerIdInput.GetComponent<InputField>().text;

        //PlayerPrefs.SetString("PlayerId", playerIdInput.text);

        UserInformation res = new UserInformation();
            
        SendGetRequest("user", res, playerId, (uwr) =>
        {
		});
    }

    IEnumerator CoSendWebRequest(string url, string method, object obj, Action<UnityWebRequest> callback)
    {
        string sendUrl = $"{_baseUrl}/{url}/";

        byte[] jsonBytes = null;

        if(obj != null)
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
}
