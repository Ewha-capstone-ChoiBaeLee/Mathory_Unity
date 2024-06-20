using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_SignIn : MonoBehaviour
{
	[SerializeField] Text errorText;
	[SerializeField] Canvas canvas;
	void OnEnable()
	{
		AccountManager.OnSignInFailed.AddListener(OnSignInFailed);
		AccountManager.OnSignInSuccess.AddListener(OnSignInSuccess);
	}
	void OnDisable()
	{
		AccountManager.OnSignInFailed.RemoveListener(OnSignInFailed);
		AccountManager.OnSignInSuccess.RemoveListener(OnSignInSuccess);
	}
	void OnSignInFailed(string error)
	{
		errorText.gameObject.SetActive(true);
		//errorText.text = error;
	}
	void OnSignInSuccess()
	{
		SceneManager.LoadScene("Home");
	}

	string userID,password;

	public void UpdateUserID(string _userID)
	{
		userID = _userID;
	}
	public void UpdatePassword(string _password)
	{
		password = _password;
	}
	public void SignIn()
	{
		AccountManager.Instance.SignIn(userID, password);
	}
}
