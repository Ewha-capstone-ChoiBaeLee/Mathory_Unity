using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class MyPage
{
    public string UserId;
    public int Year;
    public int SubjectId;
    public int Solved_Questions;
    public int Correct_Questions;

    public UserInformation UserInformation;
    public Subject Subject;
}

public class Subject
{
    public string SubjectId;
    public string Subject_Name;
}

public class MyPageManager : MonoBehaviour
{
    string _baseUrl = "https://localhost:7039/api";
    public Text text1, text2;
    public Text text1_1_1, text1_1_2, text1_2_1, text1_2_2;
    public Text text2_1_1, text2_1_2, text2_2_1, text2_2_2;
    public Text text3_1_1, text3_1_2, text3_2_1, text3_2_2;
    public Text text4_1_1, text4_1_2, text4_2_1, text4_2_2;
    public Text text5_1_1, text5_1_2, text5_3_1, text5_3_2, text5_4_1, text5_4_2;
    public Text text6_1_1, text6_1_2;
    public TextMeshProUGUI text1_1, text1_2;
    public TextMeshProUGUI text2_1, text2_2;
    public TextMeshProUGUI text3_1, text3_2;
    public TextMeshProUGUI text4_1, text4_2;
    public TextMeshProUGUI text5_1, text5_3, text5_4;
    public TextMeshProUGUI text6_1;

    public RectTransform barRectTransform1_1, barRectTransform1_2;
    public RectTransform barRectTransform2_1, barRectTransform2_2;
    public RectTransform barRectTransform3_1, barRectTransform3_2;
    public RectTransform barRectTransform4_1, barRectTransform4_2;
    public RectTransform barRectTransform5_1, barRectTransform5_3, barRectTransform5_4;
    public RectTransform barRectTransform6_1;
    public int count1, count2, count3, count4, count5, count6;
    public int sum1_1, sum1_2, average1_1, average1_2;
    public int sum2_1, sum2_2, average2_1, average2_2;
    public int sum3_1, sum3_2, average3_1, average3_2;
    public int sum4_1, sum4_2, average4_1, average4_2;
    public int sum5_1, sum5_3, sum5_4, average5_1, average5_3, average5_4;
    public int sum6_1, average6_1;
    public int answer1_1, answer1_2, answer2_1, answer2_2, answer3_1, answer3_2, answer4_1, answer4_2, answer5_1, answer5_3, answer5_4, answer6_1;
    public string PlayerId;

    void Start()
    {
        PlayerId = PlayerPrefs.GetString("PlayerId");
        int PlayerYear = PlayerPrefs.GetInt("PlayerYear");
        string PlayerName = PlayerPrefs.GetString("PlayerName");
        text1.text = PlayerYear.ToString();
        text2.text = PlayerName;
    }

    public void SceneChange()
    {
        SceneManager.LoadScene("Home");
    }

    public void Initialize()
    {
        count1 = 0; count2 = 0; count3 = 0; count4 = 0; count5 = 0; count6 = 0;
        sum1_1 = 0; sum1_2 = 0; average1_1 = 0; average1_2 = 0;
        sum2_1 = 0; sum2_2 = 0; average2_1 = 0; average2_2 = 0;
        sum3_1 = 0; sum3_2 = 0; average3_1 = 0; average3_2 = 0;
        sum4_1 = 0; sum4_2 = 0; average4_1 = 0; average4_2 = 0;
        sum5_1 = 0; sum5_3 = 0; sum5_4 = 0; average5_1 = 0; average5_3 = 0; average5_4 = 0;
        sum6_1 = 0; average6_1 = 0;
        answer1_1 = 0; answer1_2 = 0;
        answer2_1 = 0; answer2_2 = 0;
        answer3_1 = 0; answer3_2 = 0;
        answer4_1 = 0; answer4_2 = 0;
        answer5_1 = 0; answer5_3 = 0; answer5_4 = 0;
        answer6_1 = 0;

        GameObject.Find("Canvas").transform.Find("stat1_1").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("stat1_2").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("stat2_1").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("stat2_2").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("stat3_1").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("stat3_2").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("stat4_1").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("stat4_2").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("stat5_1").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("stat5_2").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("stat5_3").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("stat6").gameObject.SetActive(false);
    }

    public void Year1()
    {
        Initialize();
        GameObject.Find("Canvas").transform.Find("stat1_1").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("stat1_2").gameObject.SetActive(true);
        MyPage res1 = new MyPage();
        SendGetRequest("mypage", res1, PlayerId, 1, (uwr) =>
        {
        });
    }

    public void Year2()
    {
        Initialize();
        GameObject.Find("Canvas").transform.Find("stat2_1").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("stat2_2").gameObject.SetActive(true);
        MyPage res2 = new MyPage();
        SendGetRequest("mypage", res2, PlayerId, 2, (uwr) =>
        {
        });
    }

    public void Year3()
    {
        Initialize();
        GameObject.Find("Canvas").transform.Find("stat3_1").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("stat3_2").gameObject.SetActive(true);
        MyPage res3 = new MyPage();
        SendGetRequest("mypage", res3, PlayerId, 3, (uwr) =>
        {
        });
    }

    public void Year4()
    {
        Initialize();
        GameObject.Find("Canvas").transform.Find("stat4_1").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("stat4_2").gameObject.SetActive(true);
        MyPage res4 = new MyPage();
        SendGetRequest("mypage", res4, PlayerId, 4, (uwr) =>
        {
        });
    }

    public void Year5()
    {
        Initialize();
        GameObject.Find("Canvas").transform.Find("stat5_1").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("stat5_2").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("stat5_3").gameObject.SetActive(true);

        MyPage res5 = new MyPage();
        SendGetRequest("mypage", res5, PlayerId, 5, (uwr) =>
        {
        });
    }

    public void Year6()
    {
        Initialize();
        GameObject.Find("Canvas").transform.Find("stat6").gameObject.SetActive(true);
        MyPage res6 = new MyPage();
        SendGetRequest("mypage", res6, PlayerId, 6, (uwr) =>
        {
        });
    }

    public void SendGetRequest(string url, object obj, string playerId, int playerYear, Action<UnityWebRequest> callback)
    {

        StartCoroutine(CoSendWebRequest(url, "GET", obj, (uwr) =>
        {

            string responseJson = uwr.downloadHandler.text;
            List<MyPage> pageList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MyPage>>(responseJson);
            foreach (MyPage page in pageList)
            {
                switch (page.Year)
                {
                    case 1:
                        if(page.SubjectId == 1)
                        {
                            sum1_1 += page.Correct_Questions;
                            count1 += 1;
                            if (page.UserId == playerId)
                            {
                                answer1_1 = page.Correct_Questions;
                                text1_1_1.text = answer1_1.ToString();
                                text1_1.text = page.Solved_Questions.ToString();
                            }
                        }
                        else
                        {
                            sum1_2 += page.Correct_Questions;
                            if (page.UserId == playerId)
                            {
                                answer1_2 = page.Correct_Questions;
                                text1_2_1.text = answer1_2.ToString();
                                text1_2.text = page.Solved_Questions.ToString();
                            }
                        }
                    break;
                    case 2:
                        if (page.SubjectId == 1)
                        {
                            sum2_1 += page.Correct_Questions;
                            count2 += 1;
                            if (page.UserId == playerId)
                            {
                                answer2_1 = page.Correct_Questions;
                                text2_1_1.text = answer2_1.ToString();
                                text2_1.text = page.Solved_Questions.ToString();
                            }
                        }
                        else
                        {
                            sum2_2 += page.Correct_Questions;
                            if (page.UserId == playerId)
                            {
                                answer2_2 = page.Correct_Questions;
                                text2_2_1.text = answer2_2.ToString();
                                text2_2.text = page.Solved_Questions.ToString();
                            }
                        }
                    break;
                    case 3:
                        if (page.SubjectId == 1)
                        {
                            sum3_1 += page.Correct_Questions;
                            count3 += 1;
                            if (page.UserId == playerId)
                            {
                                answer3_1 = page.Correct_Questions;
                                text3_1_1.text = answer3_1.ToString();
                                text3_1.text = page.Solved_Questions.ToString();
                            }
                        }
                        else
                        {
                            sum3_2 += page.Correct_Questions;
                            if (page.UserId == playerId)
                            {
                                answer3_2 = page.Correct_Questions;
                                text3_2_1.text = answer3_2.ToString();
                                text3_2.text = page.Solved_Questions.ToString();
                            }
                        }
                    break;
                    case 4:
                        if (page.SubjectId == 1)
                        {
                            sum4_1 += page.Correct_Questions;
                            count4 += 1;
                            if (page.UserId == playerId)
                            {
                                answer4_1 = page.Correct_Questions;
                                text4_1_1.text = answer4_1.ToString();
                                text4_1.text = page.Solved_Questions.ToString();
                            }
                        }
                        else
                        {
                            sum4_2 += page.Correct_Questions;
                            if (page.UserId == playerId)
                            {
                                answer4_2 = page.Correct_Questions;
                                text4_2_1.text = answer4_2.ToString();
                                text4_2.text = page.Solved_Questions.ToString();
                            }
                        }
                    break;
                    case 5:
                        if (page.SubjectId == 1)
                        {
                            sum5_1 += page.Correct_Questions;
                            count5 += 1;
                            if(page.UserId == playerId)
                            {
                                answer5_1 = page.Correct_Questions;
                                text5_1_1.text = answer5_1.ToString();
                                text5_1.text = page  .Solved_Questions.ToString();
                            }
                        }
                        else if (page.SubjectId == 3)
                        {
                            sum5_3 += page.Correct_Questions;
                            if (page.UserId == playerId)
                            {
                                answer5_3 = page.Correct_Questions;
                                text5_3_1.text = answer5_3.ToString();
                                text5_3.text = page.Solved_Questions.ToString();
                            }
                        }
                        else
                        {
                            sum5_4 += page.Correct_Questions;
                            if (page.UserId == playerId)
                            {
                                answer5_4 = page.Correct_Questions;
                                text5_4_1.text = answer5_4.ToString();
                                text5_4.text = page.Solved_Questions.ToString();
                            }
                        }
                    break;
                    case 6:
                        sum6_1 += page.Correct_Questions;
                        count6 += 1;
                        if (page.UserId == playerId)
                        {
                            answer6_1 = page.Correct_Questions;
                            text6_1_1.text = answer6_1.ToString();
                            text6_1.text = page.Solved_Questions.ToString();
                        }
                    break;
                }
            }

            switch (playerYear)
            {
                case 1:
                    average1_1 = sum1_1 / count1;
                    text1_1_2.text = average1_1.ToString();
                    Vector2 size1_1 = barRectTransform1_1.sizeDelta;
                    size1_1.y = answer1_1 * 60 / average1_1;
                    if (size1_1.y > 120)
                    {
                        size1_1.y = 120;
                    }
                    barRectTransform1_1.sizeDelta = size1_1;

                    average1_2 = sum1_2 / count1;
                    text1_2_2.text = average1_2.ToString();
                    Vector2 size1_2 = barRectTransform1_2.sizeDelta;
                    size1_2.y = answer1_2 * 60 / average1_2;
                    if (size1_2.y > 120)
                    {
                        size1_2.y = 120;
                    }
                    barRectTransform1_2.sizeDelta = size1_2;
                break;
                case 2:
                    average2_1 = sum2_1 / count2;
                    text2_1_2.text = average2_1.ToString();
                    Vector2 size2_1 = barRectTransform2_1.sizeDelta;
                    size2_1.y = answer2_1 * 60 / average2_1;
                    if (size2_1.y > 120)
                    {
                        size2_1.y = 120;
                    }
                    barRectTransform2_1.sizeDelta = size2_1;

                    average2_2 = sum2_2 / count2;
                    text2_2_2.text = average2_2.ToString();
                    Vector2 size2_2 = barRectTransform2_2.sizeDelta;
                    size2_2.y = answer2_2 * 60 / average2_2;
                    if (size2_2.y > 120)
                    {
                        size2_2.y = 120;
                    }
                    barRectTransform2_2.sizeDelta = size2_2;
                break;
                case 3:
                    average3_1 = sum3_1 / count3;
                    text3_1_2.text = average3_1.ToString();
                    Vector2 size3_1 = barRectTransform3_1.sizeDelta;
                    size3_1.y = answer3_1 * 60 / average3_1;
                    if (size3_1.y > 120)
                    {
                        size3_1.y = 120;
                    }
                    barRectTransform3_1.sizeDelta = size3_1;

                    average3_2 = sum3_2 / count3;
                    text3_2_2.text = average3_2.ToString();
                    Vector2 size3_2 = barRectTransform3_2.sizeDelta;
                    size3_2.y = answer3_2 * 60 / average3_2;
                    if (size3_2.y > 120)
                    {
                        size3_2.y = 120;
                    }
                    barRectTransform3_2.sizeDelta = size3_2;
                break;
                case 4:
                    average4_1 = sum4_1 / count4;
                    text4_1_2.text = average4_1.ToString();
                    Vector2 size4_1 = barRectTransform4_1.sizeDelta;
                    size4_1.y = answer4_1 * 60 / average4_1;
                    if (size4_1.y > 120)
                    {
                        size4_1.y = 120;
                    }
                    barRectTransform4_1.sizeDelta = size4_1;

                    average4_2 = sum4_2 / count4;
                    text4_2_2.text = average4_2.ToString();
                    Vector2 size4_2 = barRectTransform4_2.sizeDelta;
                    size4_2.y = answer4_2 * 60 / average4_2;
                    if (size4_2.y > 120)
                    {
                        size4_2.y = 120;
                    }
                    barRectTransform4_2.sizeDelta = size4_2;
                break;
                case 5:
                    average5_1 = sum5_1 / count5;
                    text5_1_2.text = average5_1.ToString();
                    Vector2 size5_1 = barRectTransform5_1.sizeDelta;
                    size5_1.y = answer5_1 * 60 / average5_1;
                    if (size5_1.y > 120)
                    {
                        size5_1.y = 120;
                    }
                    barRectTransform5_1.sizeDelta = size5_1;

                    average5_3 = sum5_3 / count5;
                    text5_3_2.text = average5_3.ToString();
                    Vector2 size5_3 = barRectTransform5_3.sizeDelta;
                    size5_3.y = answer5_3 * 60 / average5_3;
                    if (size5_3.y > 120)
                    {
                        size5_3.y = 120;
                    }
                    barRectTransform5_3.sizeDelta = size5_3;

                    average5_4 = sum5_4 / count5;
                    text5_4_2.text = average5_4.ToString();
                    Vector2 size5_4 = barRectTransform5_4.sizeDelta;
                    size5_4.y = answer5_4 * 60 / average5_4;
                    if (size5_4.y > 120)
                    {
                        size5_4.y = 120;
                    }
                    barRectTransform5_4.sizeDelta = size5_4;
                break;
                case 6:
                    average6_1 = sum6_1 / count6;
                    text6_1_2.text = average6_1.ToString();
                    Vector2 size6_1 = barRectTransform6_1.sizeDelta;
                    size6_1.y = answer6_1 * 60 / average6_1;
                    if (size6_1.y > 120)
                    {
                        size6_1.y = 120;
                    }
                    barRectTransform6_1.sizeDelta = size6_1;
                break;
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
