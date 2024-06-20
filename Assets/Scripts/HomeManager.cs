using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;

public class Ranking
{
    public int Min_Score { get; set; }
    public int Max_Score { get; set; }
    public string Ranking_Name { get; set; }
}

public class HomeManager : MonoBehaviour
{
    private string apiUrl = "https://localhost:7039/api/mypage";
    private string rankingApiUrl = "https://localhost:7039/api/ranking";

    public Text[] rankingText1;
    public TextMeshProUGUI[] rankingText2;
    public TextMeshProUGUI[] rankingText3;
    public Text text1;
    public TextMeshProUGUI text2, ranking, ranking2;

    private List<KeyValuePair<string, int>> sortedList;

    private List<Ranking> rankingList = new List<Ranking>();

    void Start()
    {
        string PlayerId = PlayerPrefs.GetString("PlayerId");
        StartCoroutine(GetUserDatas(PlayerId));
    }

    IEnumerator GetUserDatas(string playerId)
    {
        yield return StartCoroutine(GetRankingData());

        UnityWebRequest www = UnityWebRequest.Get(apiUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            List<MyPage> MyPageList = JsonConvert.DeserializeObject<List<MyPage>>(jsonResponse);
            var userCorrectQuestionsSum = CalculateCorrectQuestionsSum(MyPageList);
            var sortedUserCorrectQuestionsSum = SortByCorrectQuestions(userCorrectQuestionsSum);
            DisplayRankings(sortedUserCorrectQuestionsSum, MyPageList);
            DisplayPlayerData(playerId, sortedUserCorrectQuestionsSum, MyPageList);
        }
    }

    IEnumerator GetRankingData()
    {
        UnityWebRequest www = UnityWebRequest.Get(rankingApiUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            List<Ranking> rankingList = JsonConvert.DeserializeObject<List<Ranking>>(jsonResponse);
            // 이후 사용할 수 있도록 rankingList 저장
            this.rankingList = rankingList;
        }
    }


    Dictionary<string, int> CalculateCorrectQuestionsSum(List<MyPage> data)
    {
        Dictionary<string, int> userCorrectQuestionsSum = new Dictionary<string, int>();

        foreach (var user in data)
        {
            if (userCorrectQuestionsSum.ContainsKey(user.UserId))
            {
                userCorrectQuestionsSum[user.UserId] += user.Correct_Questions;
            }
            else
            {
                userCorrectQuestionsSum[user.UserId] = user.Correct_Questions;
            }
        }

        return userCorrectQuestionsSum;
    }

    List<KeyValuePair<string, int>> SortByCorrectQuestions(Dictionary<string, int> userCorrectQuestionsSum)
    {
        var sortedList = new List<KeyValuePair<string, int>>(userCorrectQuestionsSum);

        sortedList.Sort((firstPair, nextPair) =>
        {
            return nextPair.Value.CompareTo(firstPair.Value);
        });

        return sortedList;
    }

    void DisplayRankings(List<KeyValuePair<string, int>> sortedList, List<MyPage> MyPageList)
    {
        foreach (var entry in sortedList)
        {
            for (int i = 0; i < rankingText1.Length && i < sortedList.Count; i++)
            {
                foreach (var user in MyPageList)
                {
                    if (user.UserId == sortedList[i].Key)
                    {
                        string userName = user.UserInformation.UserName;
                        rankingText1[i].text = userName;
                        break;
                    }
                }
                int score = sortedList[i].Value;
                rankingText2[i].text = sortedList[i].Value.ToString();
                rankingText3[i].text = GetRankingName(score);
                if (rankingText3[i].text == "MASTER")
                {
                    rankingText3[i].color = new Color32(138, 43, 226, 255);
                }
                else if (rankingText3[i].text == "DIAMOND")
                {
                    rankingText3[i].color = new Color32(0, 191, 255, 255);
                }
                else if (rankingText3[i].text == "GOLD")
                {
                    rankingText3[i].color = new Color32(255, 215, 0, 255);
                }
                else if (rankingText3[i].text == "SILVER")
                {
                    rankingText3[i].color = new Color32(192, 192, 192, 255);
                }
                else if (rankingText3[i].text == "BRONZE")
                {
                    rankingText3[i].color = new Color32(205, 127, 50, 255);
                }
                else if (rankingText3[i].text == "IRON")
                {
                    rankingText3[i].color = new Color32(105, 105, 105, 255);
                }
            }
        }
    }

    void DisplayPlayerData(string playerId, List<KeyValuePair<string, int>> sortedList, List<MyPage> MyPageList)
    {
        for (int i = 0; i < sortedList.Count; i++)
        {
            if (sortedList[i].Key == playerId)
            {
                foreach (var user in MyPageList)
                {
                    if (user.UserId == playerId)
                    {
                        int score = sortedList[i].Value;
                        text1.text = user.UserInformation.UserName;
                        text2.text = score.ToString();
                        ranking.text = (i + 1).ToString();
                        ranking2.text = GetRankingName(score);
                        if (ranking2.text == "MASTER")
                        {
                            ranking2.color = new Color32(138, 43, 226, 255);
                        }
                        else if (ranking2.text == "DIAMOND")
                        {
                            ranking2.color = new Color32(0, 191, 255, 255);
                        }
                        else if (ranking2.text == "GOLD")
                        {
                            ranking2.color = new Color32(255, 215, 0, 255);
                        }
                        else if (ranking2.text == "SILVER")
                        {
                            ranking2.color = new Color32(192, 192, 192, 255);
                        }
                        else if (ranking2.text == "BRONZE")
                        {
                            ranking2.color = new Color32(205, 127, 50, 255);
                        }
                        else if (ranking2.text == "IRON")
                        {
                            ranking2.color = new Color32(105, 105, 105, 255);
                        }
                        break;
                    }
                }
                break;
            }
        }
    }

    public void GetRankingColor(string rank)
    {

    }

    string GetRankingName(int score)
    {
        foreach (var ranking in rankingList)
        {
            if (score >= ranking.Min_Score && score <= ranking.Max_Score)
            {
                return ranking.Ranking_Name;
            }
        }
        return "Unranked"; // 랭킹이 없는 경우 기본값
    }

}
