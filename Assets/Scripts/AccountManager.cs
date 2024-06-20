using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Text;

public class UserInformation
{
	public int Id;
	public string UserId;
	public string UserPW;
	public string UserName;
	public int UserYear;
	public int UserLevel;
}

public class AccountManager : MonoBehaviour
{
	public static AccountManager Instance;
	public static UnityEvent OnSignInSuccess = new UnityEvent();
	public static UnityEvent<string> OnSignInFailed = new UnityEvent<string>();
	public static UnityEvent<string> OnCreateAccountFailed = new UnityEvent<string>();

    [SerializeField] Canvas canvas;

    private void Awake()
	{
		Instance = this;
	}

	string _baseUrl = "https://localhost:7039/api";
	UserInformation newUser = new UserInformation();

	public void CreateAccount(string userID, string userName, string userPW, int userYear)
	{
		newUser.UserId = userID;
		newUser.UserName = userName;
		newUser.UserPW = userPW;
		newUser.UserLevel = (userYear*2)-1;
		newUser.UserYear = userYear;
		Debug.Log("create account button pressed");
		
		SendPostRequest("user", newUser, (uwr) =>
		{
			if (uwr.result == UnityWebRequest.Result.Success)
			{
				Debug.Log("Data added successfully!");
			}
			else
			{
				Debug.LogError("Failed to add data: " + uwr.error);
				OnCreateAccountFailed.Invoke(uwr.error);
			}
		});
		
	}
	public void SendPostRequest(string url, object obj, Action<UnityWebRequest> callback)
	{
		StartCoroutine(CoSendWebRequest(url, "POST", obj, callback));
        SignUpSuccess();
    }

	public void SignUpSuccess()
	{
		GameObject.Find("Account").transform.Find("CreateAccount").gameObject.SetActive(false);
        GameObject.Find("Account").transform.Find("Popup").gameObject.SetActive(true);
    }

	public void ExitPopup()
	{
        GameObject.Find("Account").transform.Find("Popup").gameObject.SetActive(false);
        canvas.enabled = true;
    }

    public void SignIn(string userID, string userPW)
	{
		UserInformation res = new UserInformation();

		SendGetRequest("user", res, userID, userPW, (uwr) =>
		{
		});
	}

	public void SendGetRequest(string url, object obj, string playerId, string password, Action<UnityWebRequest> callback)
	{

		StartCoroutine(CoSendWebRequest(url, "GET", obj, (uwr) =>
		{

			string responseJson = uwr.downloadHandler.text;
			List<UserInformation> userList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserInformation>>(responseJson);
			bool valid = false;
			foreach (UserInformation user in userList)
			{
				if (user.UserId == playerId && user.UserPW == password)
				{
					Debug.Log("Sign in successful");
					Debug.Log($"UserId: {user.UserId}, UserName: {user.UserName}, UserLevel: {user.UserLevel}, UserYear: {user.UserYear}");
					PlayerPrefs.SetString("PlayerId", user.UserId);
					PlayerPrefs.SetInt("PlayerLevel", user.UserLevel);
					PlayerPrefs.SetString("PlayerName", user.UserName);
					PlayerPrefs.SetInt("PlayerYear", user.UserYear);
					valid = true;
					OnSignInSuccess.Invoke();
					break;
				}
				
			}
			if (valid == false)
			{			
				Debug.Log("Invalid password or username");
				OnSignInFailed.Invoke("Invalid password or username");
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
			callback.Invoke(uwr);
		}
	}



}
