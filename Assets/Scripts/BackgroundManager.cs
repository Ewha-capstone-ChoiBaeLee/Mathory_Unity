using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class BackgroundManager : MonoBehaviour
{
    string _baseUrl = "https://localhost:7039/api";
    public SpriteRenderer backgroundRenderer;  // 배경 이미지를 표시할 SpriteRenderer
    private List<Sprite> backgroundImages = new List<Sprite>();
    private List<Sprite> usedImages = new List<Sprite>();
    private Sprite currentBackground;

    private int SceneNum = 1;

    void Awake()
    {
        CheckBackground();
    }

    private void CheckBackground()
    {
        Story res = new Story();

        SendGetRequest("Story", res, SceneNum, (uwr) =>
        {
        });
    }

    public void SetNextBackground()
    {
        if (backgroundImages.Count == 0)
        {
            Debug.LogError("No background images loaded.");
            return;
        }

        List<Sprite> availableImages = new List<Sprite>(backgroundImages);

        // 사용된 이미지를 제외한 리스트 생성
        foreach (Sprite usedImage in usedImages)
        {
            availableImages.Remove(usedImage);
        }

        if (availableImages.Count == 0)
        {
            // 모든 이미지가 사용된 경우 사용된 이미지를 초기화
            availableImages = new List<Sprite>(backgroundImages);
            usedImages.Clear();
        }

        // 랜덤으로 새로운 배경 선택
        int randomIndex = Random.Range(0, availableImages.Count);
        currentBackground = availableImages[randomIndex];

        // 사용된 이미지 목록 업데이트
        if (usedImages.Count >= 4) // 씬이 4개라면 최대 4개의 이미지를 추적
        {
            usedImages.RemoveAt(0); // 가장 오래된 이미지를 제거
        }
        usedImages.Add(currentBackground);

        // 배경 이미지 설정
        if (backgroundRenderer != null)
        {
            backgroundRenderer.sprite = currentBackground;
        }
    }

    public void SendGetRequest(string url, object obj, int sceneNum, Action<UnityWebRequest> callback)
    {

        StartCoroutine(CoSendWebRequest(url, "GET", obj, (uwr) =>
        {

            string responseJson = uwr.downloadHandler.text;

            List<Story> storyList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Story>>(responseJson);

            foreach (Story story in storyList)
            {
                if (story.Part == sceneNum)
                {
                    switch (story.Location)
                    {
                        case "기숙사":
                            if (story.Time == "낮")
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Dormitory_a");
                                backgroundImages.AddRange(loadedImages);
                            }
                            else
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Dormitory_p");
                                backgroundImages.AddRange(loadedImages);
                            }
                            break;
                        case "마법 교실":
                            if (story.Time == "낮")
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Classroom_a");
                                backgroundImages.AddRange(loadedImages);
                            }
                            else
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Classroom_p");
                                backgroundImages.AddRange(loadedImages);
                            }
                            break;
                        case "학교 도서관":
                            if (story.Time == "낮")
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Library_a");
                                backgroundImages.AddRange(loadedImages);
                            }
                            else
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Library_p");
                                backgroundImages.AddRange(loadedImages);
                            }
                            break;
                        case "학교 보건실":
                            if (story.Time == "낮")
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Healthcenter_a");
                                backgroundImages.AddRange(loadedImages);
                            }
                            else
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Healthcenter_p");
                                backgroundImages.AddRange(loadedImages);
                            }
                            break;
                        case "학교 식당":
                            if (story.Time == "낮")
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Cafeteria_a");
                                backgroundImages.AddRange(loadedImages);
                            }
                            else
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Cafeteria_p");
                                backgroundImages.AddRange(loadedImages);
                            }
                            break;
                        case "동아리 방":
                            if (story.Time == "낮")
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Clubroom_a");
                                backgroundImages.AddRange(loadedImages);
                            }
                            else
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Clubroom_p");
                                backgroundImages.AddRange(loadedImages);
                            }
                            break;
                        case "학교 운동장":
                            if (story.Time == "낮")
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Schoolyard_a");
                                backgroundImages.AddRange(loadedImages);
                            }
                            else
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Schoolyard_p");
                                backgroundImages.AddRange(loadedImages);
                            }
                            break;
                        case "마법의 숲":
                            if (story.Time == "낮")
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Forest_a");
                                backgroundImages.AddRange(loadedImages);
                            }
                            else
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Forest_p");
                                backgroundImages.AddRange(loadedImages);
                            }
                            break;
                        case "마법 마을":
                            if (story.Time == "낮")
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Village_a");
                                backgroundImages.AddRange(loadedImages);
                            }
                            else
                            {
                                Sprite[] loadedImages = Resources.LoadAll<Sprite>("Backgrounds/Village_p");
                                backgroundImages.AddRange(loadedImages);
                            }
                            break;
                    }
                }
            }
            SetNextBackground();
            // 콜백 함수 호출
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
}
