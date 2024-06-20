using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_CreateAccount : MonoBehaviour
{
	[SerializeField] Text errorText;
	[SerializeField] Canvas canvas;
	void OnEnable()
	{
		AccountManager.OnCreateAccountFailed.AddListener(OnCreateAccountFailed);
		AccountManager.OnSignInSuccess.AddListener(OnSignInSuccess);
	}
	void OnDisable()
	{
		AccountManager.OnCreateAccountFailed.RemoveListener(OnCreateAccountFailed);
		AccountManager.OnSignInSuccess.RemoveListener(OnSignInSuccess);
	}
	void OnCreateAccountFailed(string error)
	{
		errorText.gameObject.SetActive(true);
		//errorText.text = error;
	}
	void OnSignInSuccess()
	{
		canvas.enabled = false;
    }

	string userID, username, password, year;

    public void UpdateUserID (string _userID)
    {
        userID = _userID;
    }
	public void UpdateUsername(string _username)
	{
		username = _username;
	}
	public void UpdatePassword(string _password)
	{
		password = _password;
	}
	public void UpdateYear(string _year)
	{
		year = _year;
	}

	public void CreateAccount()
	{
		int yr = int.Parse(year);
		AccountManager.Instance.CreateAccount(userID,username,password,yr);
	}
}
