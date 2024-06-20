using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;


public class ToggleStatus : MonoBehaviour
{
    public GameObject toggle1;
    public GameObject toggle2;
    public GameObject toggle3;
    public GameObject toggle4;
    public GameObject toggle5;

    public Button btn;

    string _baseUrl = "https://localhost:7039/api";

    void Start()
    {
        btn.onClick.AddListener(SendToggleStatusToServer);
        btn.onClick.AddListener(ExecuteController);
        Debug.Log(PlayerPrefs.GetInt("GameLevel"));
    }

    void Update()
    {

    }

    // 토글 상태를 서버에 전송하는 메소드
    public void SendToggleStatusToServer()
    {
        // 토글 상태를 리스트에 저장
        List<int> toggleStatuses = new List<int>();

        if (toggle1.GetComponent<Toggle>().isOn)
            toggleStatuses.Add(0);
        if (toggle2.GetComponent<Toggle>().isOn)
            toggleStatuses.Add(1);
        if (toggle3.GetComponent<Toggle>().isOn)
            toggleStatuses.Add(2);
        if (toggle4.GetComponent<Toggle>().isOn)
            toggleStatuses.Add(3);
        if (toggle5.GetComponent<Toggle>().isOn)
            toggleStatuses.Add(4);

        // JSON 형태로 변환
        string jsonData = JsonConvert.SerializeObject(toggleStatuses);

        // 서버에 전송
        StartCoroutine(PostRequest("ToggleStatus", jsonData));
    }

    public void ExecuteController()
    {
        // GPT_StoryController 실행
        StartCoroutine(SendRequest("GPT_Story"));
    }

	// 서버에 POST 요청을 보내는 코루틴
	IEnumerator PostRequest(string url, string json)
	{
		string sendUrl = $"{_baseUrl}/{url}/";
		Debug.Log($"Sending request to: {sendUrl}"); // 요청 URL 디버깅

		var uwr = new UnityWebRequest(sendUrl, "POST");

		byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
		Debug.Log($"Request payload: {json}"); // 요청 데이터 디버깅

		uwr.SetRequestHeader("Content-Type", "application/json");
		uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
		uwr.downloadHandler = new DownloadHandlerBuffer();

		// 요청 보내기
		yield return uwr.SendWebRequest();

		// 요청이 성공했는지 여부를 확인
		if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
		{
			Debug.LogError($"Error sending request: {uwr.error}"); // 오류 디버깅
			Debug.LogError($"Response Code: {uwr.responseCode}"); // 응답 코드 디버깅
		}
		else
		{
			Debug.Log($"Status Code: {uwr.responseCode}"); // 응답 코드 디버깅
			Debug.Log($"Response: {uwr.downloadHandler.text}"); // 응답 내용 디버깅
		}
	}


	IEnumerator SendRequest(string url)
	{
		string sendUrl = $"{_baseUrl}/{url}/";

		Debug.Log($"Sending request to: {sendUrl}"); // 요청 URL 디버깅

		var www = new UnityWebRequest(sendUrl, "POST");
		www.downloadHandler = new DownloadHandlerBuffer();
		www.SetRequestHeader("Content-Type", "application/json");

		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError($"Error: {www.error}"); // 오류 디버깅
		}
		else
		{
			Debug.Log("Controller executed successfully");
			Debug.Log($"Response Code: {www.responseCode}"); // 응답 코드 디버깅
			Debug.Log($"Response: {www.downloadHandler.text}"); // 응답 내용 디버깅
		}
	}
}
