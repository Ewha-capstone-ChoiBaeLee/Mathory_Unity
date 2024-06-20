using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using TexDrawLib;
using System.Linq;
using System.Text.RegularExpressions;

public class StoryManager3 : MonoBehaviour
{
	string _baseUrl = "https://localhost:7039/api";
	public Text Storytxt1;
    public Text Storytxt2;
    public Text Storytxt2_Name;
    public Sprite[] backgrounds;
    public string playerName, playerId;
    public int playerLevel, playerYear, gameLevel;
    public int Num, star;
    public Image backgroundImage;
    private int SceneNum = 3;

    public Text Quiz;
    public TEXDraw answer1, answer2, answer3, answer4, answer5, answer6, answer7;
    private int Num2 = 3;

    public Button btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn_c, btn_ic;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Canvas").transform.Find("chat1").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("chat2").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("btn_chat").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("btn_next").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("problem").gameObject.SetActive(false);

        Num = PlayerPrefs.GetInt("StoryLineNum2")-1;
		star = PlayerPrefs.GetInt("star");
        Debug.Log(star);
        GetStory();
        RemoveCharacterImg();
        playerName = PlayerPrefs.GetString("PlayerName");
        playerId = PlayerPrefs.GetString("PlayerId");
        playerLevel = PlayerPrefs.GetInt("PlayerLevel");
        gameLevel = PlayerPrefs.GetInt("GameLevel");
        CheckPlayerYear();
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
                    if (SceneNum == storyline.Part)
                    {
                        GameObject.Find("Canvas").transform.Find("btn_chat").gameObject.SetActive(true);
                    }
                    else
                    {
                        GameObject.Find("Canvas").transform.Find("btn_chat").gameObject.SetActive(false);
                        GameObject.Find("Canvas").transform.Find("btn_next").gameObject.SetActive(true);
                        PlayerPrefs.SetInt("StoryLineNum3", storyline.Num);
                    }
                }
            }
            callback.Invoke(uwr);
        }));
    }


    public void SendGetRequest3(string url, object obj, int Part, Action<UnityWebRequest> callback)
    {

        StartCoroutine(CoSendWebRequest(url, "GET", obj, (uwr) =>
        {

            string responseJson = uwr.downloadHandler.text;

            List<Quiz> QuizList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Quiz>>(responseJson);

            foreach (Quiz qu in QuizList)
            {
                int n = 0;
                string font = "\\medium \\color{black}";
                if (qu.Part == Part)
                {
                    Quiz.text = qu.Problem;

                    if (qu.Equation.Contains("Min") || qu.Equation.Contains("Max"))// min max problem 
                    {
                        if (qu.Equation.Contains("Fraction")) // 2 fraction min max
                        {
                            GameObject.Find("Canvas/problem").transform.Find("btn_answer1").gameObject.SetActive(false);
                            GameObject.Find("Canvas/problem").transform.Find("btn_answer2").gameObject.SetActive(false);
                            GameObject.Find("Canvas/problem").transform.Find("btn_answer3").gameObject.SetActive(false);
                            GameObject.Find("Canvas/problem").transform.Find("btn_answer6").gameObject.SetActive(false);
                            GameObject.Find("Canvas/problem").transform.Find("btn_answer7").gameObject.SetActive(false);

                            //extract choices from equation
                            string fracPattern = @"Fraction\((\d+),\s*(\d+)\)";
                            MatchCollection matches = Regex.Matches(qu.Equation, fracPattern);
                            int[] numerators = new int[2];
                            int[] denominators = new int[2];
                            int index = 0;
                            foreach (Match match in matches)
                            {
                                if (match.Success && index < 2)
                                {
                                    // The first capture group is the numerator
                                    numerators[index] = int.Parse(match.Groups[1].Value);
                                    // The second capture group is the denominator
                                    denominators[index] = int.Parse(match.Groups[2].Value);
                                    index++;
                                }
                            }

                            //extract answer
                            Match matchy = Regex.Match(qu.Answer, fracPattern);
                            int ansNum = 0; int ansDen = 0;
                            if (matchy.Success)
                            {
                                ansNum = int.Parse(matchy.Groups[1].Value);
                                ansDen = int.Parse(matchy.Groups[2].Value);
                            }

                            //select answer and other choice
                            int choiceNum = 0; int choiceDen = 0;
                            if ((numerators[0] == ansNum) && (denominators[0] == ansDen))
                            {
                                choiceNum = numerators[1]; choiceDen = denominators[1];
                            }
                            else if ((numerators[1] == ansNum) && (denominators[1] == ansDen))

                            {
                                choiceNum = numerators[0]; choiceDen = denominators[0];
                            }

                            //answer button
                            n = Random.Range(1, 2);
                            if (n == 1)
                            {
                                answer4.text = font + "\\frac{" + ansNum.ToString() + "}{" + ansDen.ToString() + "}"; //answer
                                answer5.text = font + "\\frac{" + choiceNum.ToString() + "}{" + choiceDen.ToString() + "}";

                                btn4.onClick.AddListener(AddStar);
                                btn4.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                btn5.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn4.onClick.AddListener(Correct);
                                btn5.onClick.AddListener(InCorrect);
                            }
                            else
                            {
                                answer4.text = font + "\\frac{" + choiceNum.ToString() + "}{" + choiceDen.ToString() + "}";
                                answer5.text = font + "\\frac{" + ansNum.ToString() + "}{" + ansDen.ToString() + "}"; //answer

                                btn5.onClick.AddListener(AddStar);
                                btn5.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                btn4.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn5.onClick.AddListener(Correct);
                                btn4.onClick.AddListener(InCorrect);
                            }
                        }
                        else if (qu.Equation.Contains(".")) // 2 decimal min max
                        {
                            GameObject.Find("Canvas/problem").transform.Find("btn_answer1").gameObject.SetActive(false);
                            GameObject.Find("Canvas/problem").transform.Find("btn_answer2").gameObject.SetActive(false);
                            GameObject.Find("Canvas/problem").transform.Find("btn_answer3").gameObject.SetActive(false);
                            GameObject.Find("Canvas/problem").transform.Find("btn_answer6").gameObject.SetActive(false);
                            GameObject.Find("Canvas/problem").transform.Find("btn_answer7").gameObject.SetActive(false);

                            //extract choices from equation
                            string decimalPattern = @"(?:Min|Max)\((\d+\.\d+),\s*(\d+\.\d+)\)";
                            Match match = Regex.Match(qu.Equation, decimalPattern);
                            double num1 = 0; double num2 = 0;
                            if (match.Success)
                            {
                                num1 = double.Parse(match.Groups[1].Value);
                                num2 = double.Parse(match.Groups[2].Value);
                            }

                            //select answer and other choice
                            double choice = 0; double answer = double.Parse(qu.Answer);
                            if (answer == num1)
                            {
                                choice = num2;
                            }
                            else if (answer == num2)
                            {
                                choice = num1;
                            }

                            //answer button
                            n = Random.Range(1, 2);
                            if (n == 1)
                            {
                                answer4.text = font + answer;
                                answer5.text = font + choice;

                                btn4.onClick.AddListener(AddStar);
                                btn4.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                btn5.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn4.onClick.AddListener(Correct);
                                btn5.onClick.AddListener(InCorrect);
                            }
                            else
                            {
                                answer4.text = font + choice;
                                answer5.text = font + answer;

                                btn5.onClick.AddListener(AddStar);
                                btn5.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                btn4.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn5.onClick.AddListener(Correct);
                                btn4.onClick.AddListener(InCorrect);
                            }

                        }
                        else // integer min max
                        {
                            string eq = "";
                            if (qu.Equation.Contains("Min"))
                            {
                                eq = qu.Equation.Replace("Min(", "").Replace(")", "");
                            }
                            else
                            {
                                eq = qu.Equation.Replace("Max(", "").Replace(")", "");
                            }
                            int[] numbers = eq.Split(',').Select(int.Parse).ToArray();

                            switch (numbers.Length)
                            {
                                case 2: //2 integers
                                    GameObject.Find("Canvas/problem").transform.Find("btn_answer1").gameObject.SetActive(false);
                                    GameObject.Find("Canvas/problem").transform.Find("btn_answer2").gameObject.SetActive(false);
                                    GameObject.Find("Canvas/problem").transform.Find("btn_answer3").gameObject.SetActive(false);
                                    GameObject.Find("Canvas/problem").transform.Find("btn_answer6").gameObject.SetActive(false);
                                    GameObject.Find("Canvas/problem").transform.Find("btn_answer7").gameObject.SetActive(false);

                                    //select answer and other choice
                                    int choice = 0; int answer = int.Parse(qu.Answer);

                                    if (numbers[0] == answer)
                                    {
                                        choice = numbers[1];
                                    }
                                    else
                                    {
                                        choice = numbers[0];
                                    }

                                    //answer button
                                    n = Random.Range(1, 2);
                                    if (n == 1)
                                    {
                                        answer4.text = font + answer;
                                        answer5.text = font + choice;

                                        btn4.onClick.AddListener(AddStar);
                                        btn4.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                        btn5.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn4.onClick.AddListener(Correct);
                                        btn5.onClick.AddListener(InCorrect);
                                    }
                                    else
                                    {
                                        answer4.text = font + choice;
                                        answer5.text = font + answer;

                                        btn5.onClick.AddListener(AddStar);
                                        btn5.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                        btn4.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn5.onClick.AddListener(Correct);
                                        btn4.onClick.AddListener(InCorrect);
                                    }
                                    break;

                                case 3: // 3 integers
                                    GameObject.Find("Canvas/problem").transform.Find("btn_answer4").gameObject.SetActive(false);
                                    GameObject.Find("Canvas/problem").transform.Find("btn_answer5").gameObject.SetActive(false);
                                    GameObject.Find("Canvas/problem").transform.Find("btn_answer6").gameObject.SetActive(false);
                                    GameObject.Find("Canvas/problem").transform.Find("btn_answer7").gameObject.SetActive(false);

                                    //select answer and other choice
                                    int choice1 = 0; int choice2 = 0; int ans = int.Parse(qu.Answer);
                                    if (numbers[0] == ans)
                                    {
                                        choice1 = numbers[1];
                                        choice2 = numbers[2];
                                    }
                                    else if (numbers[1] == ans)
                                    {
                                        choice1 = numbers[0];
                                        choice2 = numbers[2];
                                    }
                                    else if (numbers[2] == ans)
                                    {
                                        choice1 = numbers[0];
                                        choice2 = numbers[1];
                                    }

                                    //answer button
                                    n = Random.Range(1, 3);
                                    if (n == 1)
                                    {
                                        answer1.text = font + ans;
                                        answer2.text = font + choice1;
                                        answer3.text = font + choice2;

                                        btn1.onClick.AddListener(AddStar);
                                        btn1.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                        btn2.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn3.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn1.onClick.AddListener(Correct);
                                        btn2.onClick.AddListener(InCorrect);
                                        btn3.onClick.AddListener(InCorrect);
                                    }
                                    else if (n == 2)
                                    {
                                        answer1.text = font + choice1;
                                        answer2.text = font + ans;
                                        answer3.text = font + choice2;

                                        btn2.onClick.AddListener(AddStar);
                                        btn2.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                        btn1.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn3.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn2.onClick.AddListener(Correct);
                                        btn1.onClick.AddListener(InCorrect);
                                        btn3.onClick.AddListener(InCorrect);
                                    }
                                    else
                                    {
                                        answer1.text = font + choice1;
                                        answer2.text = font + choice2;
                                        answer3.text = font + ans;

                                        btn3.onClick.AddListener(AddStar);
                                        btn3.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                        btn1.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn2.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn3.onClick.AddListener(Correct);
                                        btn1.onClick.AddListener(InCorrect);
                                        btn2.onClick.AddListener(InCorrect);
                                    }
                                    break;

                                case 4: //4 integers
                                    GameObject.Find("Canvas/problem").transform.Find("btn_answer1").gameObject.SetActive(false);
                                    GameObject.Find("Canvas/problem").transform.Find("btn_answer2").gameObject.SetActive(false);
                                    GameObject.Find("Canvas/problem").transform.Find("btn_answer3").gameObject.SetActive(false);

                                    //select answer and other choice
                                    int cho1 = 0; int cho2 = 0; int cho3 = 0; int anz = int.Parse(qu.Answer);
                                    if (numbers[0] == anz)
                                    {
                                        cho1 = numbers[1];
                                        cho2 = numbers[2];
                                        cho3 = numbers[3];
                                    }
                                    else if (numbers[1] == anz)
                                    {
                                        cho1 = numbers[0];
                                        cho2 = numbers[2];
                                        cho3 = numbers[3];
                                    }
                                    else if (numbers[2] == anz)
                                    {
                                        cho1 = numbers[0];
                                        cho2 = numbers[1];
                                        cho3 = numbers[3];
                                    }
                                    else if (numbers[3] == anz)
                                    {
                                        cho1 = numbers[0];
                                        cho2 = numbers[1];
                                        cho3 = numbers[2];
                                    }

                                    //answer button
                                    n = Random.Range(1, 4);
                                    if (n == 1)
                                    {
                                        answer4.text = font + anz;
                                        answer5.text = font + cho1;
                                        answer6.text = font + cho2;
                                        answer7.text = font + cho3;

                                        btn4.onClick.AddListener(AddStar);
                                        btn4.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                        btn5.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn6.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn7.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn4.onClick.AddListener(Correct);
                                        btn5.onClick.AddListener(InCorrect);
                                        btn6.onClick.AddListener(InCorrect);
                                        btn7.onClick.AddListener(InCorrect);
                                    }
                                    else if (n == 2)
                                    {
                                        answer4.text = font + cho1;
                                        answer5.text = font + anz;
                                        answer6.text = font + cho2;
                                        answer7.text = font + cho3;

                                        btn5.onClick.AddListener(AddStar);
                                        btn5.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                        btn4.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn6.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn7.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn5.onClick.AddListener(Correct);
                                        btn4.onClick.AddListener(InCorrect);
                                        btn6.onClick.AddListener(InCorrect);
                                        btn7.onClick.AddListener(InCorrect);
                                    }
                                    else if (n == 3)
                                    {
                                        answer4.text = font + cho1;
                                        answer5.text = font + cho2;
                                        answer6.text = font + anz;
                                        answer7.text = font + cho3; ;

                                        btn6.onClick.AddListener(AddStar);
                                        btn6.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                        btn4.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn5.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn7.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn6.onClick.AddListener(Correct);
                                        btn4.onClick.AddListener(InCorrect);
                                        btn5.onClick.AddListener(InCorrect);
                                        btn7.onClick.AddListener(InCorrect);
                                    }
                                    else
                                    {
                                        answer4.text = font + cho1;
                                        answer5.text = font + cho2;
                                        answer6.text = font + cho3;
                                        answer7.text = font + anz;

                                        btn7.onClick.AddListener(AddStar);
                                        btn7.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                        btn4.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn5.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn6.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                        btn7.onClick.AddListener(Correct);
                                        btn4.onClick.AddListener(InCorrect);
                                        btn5.onClick.AddListener(InCorrect);
                                        btn6.onClick.AddListener(InCorrect);
                                    }

                                    break;
                            }
                        }
                    }

                    else // non min max problem
                    {
                        GameObject.Find("Canvas/problem").transform.Find("btn_answer4").gameObject.SetActive(false);
                        GameObject.Find("Canvas/problem").transform.Find("btn_answer5").gameObject.SetActive(false);
                        GameObject.Find("Canvas/problem").transform.Find("btn_answer6").gameObject.SetActive(false);
                        GameObject.Find("Canvas/problem").transform.Find("btn_answer7").gameObject.SetActive(false);

                        if (qu.Equation.Contains("Fraction")) // fraction equation
                        {
                            int numerator = 0; int denominator = 0;
                            n = Random.Range(1, 3);
                            string fracPattern = @"Fraction\((\w+),\s*(\w+)\)";
                            string fracReplacement = @"\frac{$1}{$2}";
                            string answer = Regex.Replace(qu.Answer, fracPattern, fracReplacement);

                            //extract numerator, denominator --> qu.Answer = frac(2, 5)
                            string pattern = @"Fraction\((\d+),\s*(\d+)\)";
                            Regex regex = new Regex(pattern);

                            Match match = regex.Match(qu.Answer);
                            if (match.Success)
                            {
                                numerator = int.Parse(match.Groups[1].Value);
                                denominator = int.Parse(match.Groups[2].Value);
                            }

                            if (n == 1)
                            {
                                answer1.text = font + answer;
                                answer2.text = font + "\\frac{" + (numerator + 1).ToString() + "}{" + denominator.ToString() + "}";
                                answer3.text = font + "\\frac{" + (numerator + 2).ToString() + "}{" + denominator.ToString() + "}";

                                btn1.onClick.AddListener(AddStar);
                                btn1.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                btn2.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn3.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn1.onClick.AddListener(Correct);
                                btn2.onClick.AddListener(InCorrect);
                                btn3.onClick.AddListener(InCorrect);
                            }
                            else if (n == 2)
                            {
                                answer1.text = font + "\\frac{" + (numerator - 1).ToString() + "}{" + denominator.ToString() + "}";
                                answer2.text = font + answer;
                                answer3.text = font + "\\frac{" + (numerator + 1).ToString() + "}{" + denominator.ToString() + "}";

                                btn2.onClick.AddListener(AddStar);
                                btn2.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                btn1.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn3.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn2.onClick.AddListener(Correct);
                                btn1.onClick.AddListener(InCorrect);
                                btn3.onClick.AddListener(InCorrect);
                            }
                            else
                            {
                                answer1.text = font + "\\frac{" + (numerator - 2).ToString() + "}{" + denominator.ToString() + "}";
                                answer2.text = font + "\\frac{" + (numerator - 1).ToString() + "}{" + denominator.ToString() + "}";
                                answer3.text = font + answer;

                                btn3.onClick.AddListener(AddStar);
                                btn3.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                btn1.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn2.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn3.onClick.AddListener(Correct);
                                btn1.onClick.AddListener(InCorrect);
                                btn2.onClick.AddListener(InCorrect);
                            }
                        }
                        else if (qu.Equation.Contains("."))
                        {
                            if (qu.Equation.Contains("mean"))  // average equation
                            {
                                n = Random.Range(1, 3);

                                if (n == 1)
                                {
                                    answer1.text = font + qu.Answer;
                                    answer2.text = font + (int.Parse(qu.Answer) + 1).ToString();
                                    answer3.text = font + (int.Parse(qu.Answer) + 2).ToString();

                                    btn1.onClick.AddListener(AddStar);
                                    btn1.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                    btn2.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                    btn3.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                    btn1.onClick.AddListener(Correct);
                                    btn2.onClick.AddListener(InCorrect);
                                    btn3.onClick.AddListener(InCorrect);
                                }
                                else if (n == 2)
                                {
                                    answer1.text = font + (int.Parse(qu.Answer) - 1).ToString();
                                    answer2.text = font + qu.Answer;
                                    answer3.text = font + (int.Parse(qu.Answer) + 1).ToString();

                                    btn2.onClick.AddListener(AddStar);
                                    btn2.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                    btn1.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                    btn3.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                    btn2.onClick.AddListener(Correct);
                                    btn1.onClick.AddListener(InCorrect);
                                    btn3.onClick.AddListener(InCorrect);
                                }
                                else
                                {
                                    answer1.text = font + (int.Parse(qu.Answer) - 2).ToString();
                                    answer2.text = font + (int.Parse(qu.Answer) - 1).ToString();
                                    answer3.text = font + qu.Answer;

                                    btn3.onClick.AddListener(AddStar);
                                    btn3.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                    btn1.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                    btn2.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                    btn3.onClick.AddListener(Correct);
                                    btn1.onClick.AddListener(InCorrect);
                                    btn2.onClick.AddListener(InCorrect);
                                }
                            }
                            else  // decimal equation
                            {
                                n = Random.Range(1, 3);

                                if (n == 1)
                                {
                                    answer1.text = font + qu.Answer;
                                    answer2.text = font + (double.Parse(qu.Answer) + 0.1).ToString();
                                    answer3.text = font + (double.Parse(qu.Answer) + 0.2).ToString();

                                    btn1.onClick.AddListener(AddStar);
                                    btn1.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                    btn2.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                    btn3.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                    btn1.onClick.AddListener(Correct);
                                    btn2.onClick.AddListener(InCorrect);
                                    btn3.onClick.AddListener(InCorrect);
                                }
                                else if (n == 2)
                                {
                                    answer1.text = font + (double.Parse(qu.Answer) - 0.1).ToString();
                                    answer2.text = font + qu.Answer;
                                    answer3.text = font + (double.Parse(qu.Answer) + 0.1).ToString();

                                    btn2.onClick.AddListener(AddStar);
                                    btn2.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                    btn1.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                    btn3.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                    btn2.onClick.AddListener(Correct);
                                    btn1.onClick.AddListener(InCorrect);
                                    btn3.onClick.AddListener(InCorrect);
                                }
                                else
                                {
                                    answer1.text = font + (double.Parse(qu.Answer) - 0.2).ToString();
                                    answer2.text = font + (double.Parse(qu.Answer) - 0.1).ToString();
                                    answer3.text = font + qu.Answer;

                                    btn3.onClick.AddListener(AddStar);
                                    btn3.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                    btn1.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                    btn2.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                    btn3.onClick.AddListener(Correct);
                                    btn1.onClick.AddListener(InCorrect);
                                    btn2.onClick.AddListener(InCorrect);
                                }
                            }

                        }
                        else if (qu.Equation.Contains("divmod"))
                        {
                            string divAnswerPattern = @"\((\w+),\s*(\w+)\)";
                            string divAnswerReplacement = @"{{}... {2}";
                            //answer = (2, 54)
                            string solution = Regex.Replace(qu.Answer, divAnswerPattern, divAnswerReplacement);

                            string[] parts = qu.Answer.Trim('(', ')').Split(',');

                            int quotient = int.Parse(parts[0].Trim());
                            int remainder = int.Parse(parts[1].Trim());

                            n = Random.Range(1, 3);
                            if (n == 1)
                            {
                                answer1.text = font + quotient + "..." + remainder;
                                answer2.text = font + (quotient + 1) + "..." + remainder;
                                answer3.text = font + (quotient + 2) + "..." + remainder;

                                btn1.onClick.AddListener(AddStar);
                                btn1.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                btn2.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn3.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn1.onClick.AddListener(Correct);
                                btn2.onClick.AddListener(InCorrect);
                                btn3.onClick.AddListener(InCorrect);

                            }
                            else if (n == 2)
                            {
                                answer1.text = font + (quotient - 1) + "..." + remainder;
                                answer2.text = font + quotient + "..." + remainder;
                                answer3.text = font + (quotient + 1) + "..." + remainder;

                                btn2.onClick.AddListener(AddStar);
                                btn2.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                btn1.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn3.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn2.onClick.AddListener(Correct);
                                btn1.onClick.AddListener(InCorrect);
                                btn3.onClick.AddListener(InCorrect);
                            }
                            else
                            {
                                answer1.text = font + (quotient - 2) + "..." + remainder;
                                answer2.text = font + (quotient - 1) + "..." + remainder;
                                answer3.text = font + quotient + "..." + remainder;

                                btn3.onClick.AddListener(AddStar);
                                btn3.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                btn1.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn2.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn3.onClick.AddListener(Correct);
                                btn1.onClick.AddListener(InCorrect);
                                btn2.onClick.AddListener(InCorrect);
                            }
                        }
                        else // integer equation
                        {
                            n = Random.Range(1, 3);

                            if (n == 1)
                            {
                                answer1.text = font + qu.Answer;
                                answer2.text = font + (int.Parse(qu.Answer) + 1).ToString();
                                answer3.text = font + (int.Parse(qu.Answer) + 2).ToString();

                                btn1.onClick.AddListener(AddStar);
                                btn1.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                btn2.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn3.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn1.onClick.AddListener(Correct);
                                btn2.onClick.AddListener(InCorrect);
                                btn3.onClick.AddListener(InCorrect);
                            }
                            else if (n == 2)
                            {
                                answer1.text = font + (int.Parse(qu.Answer) - 1).ToString();
                                answer2.text = font + qu.Answer;
                                answer3.text = font + (int.Parse(qu.Answer) + 1).ToString();

                                btn2.onClick.AddListener(AddStar);
                                btn2.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                btn1.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn3.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn2.onClick.AddListener(Correct);
                                btn1.onClick.AddListener(InCorrect);
                                btn3.onClick.AddListener(InCorrect);
                            }
                            else
                            {
                                answer1.text = font + (int.Parse(qu.Answer) - 2).ToString();
                                answer2.text = font + (int.Parse(qu.Answer) - 1).ToString();
                                answer3.text = font + qu.Answer;

                                btn3.onClick.AddListener(AddStar);
                                btn3.onClick.AddListener(() => UpdateQuiz1(qu.SubjectId));
                                btn1.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn2.onClick.AddListener(() => UpdateQuiz2(qu.SubjectId));
                                btn3.onClick.AddListener(Correct);
                                btn1.onClick.AddListener(InCorrect);
                                btn2.onClick.AddListener(InCorrect);
                            }
                        }
                    }
                }
            }
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

    public void GetQuiz()
    {
        GameObject.Find("Canvas").transform.Find("problem").gameObject.SetActive(true);

        Quiz res2 = new Quiz();

        SendGetRequest3("Quiz", res2, Num2, (uwr) =>
        {
            //Debug.Log("");
        });
    }

    public void Correct()
    {
        GameObject.Find("Canvas").transform.Find("Correct").gameObject.SetActive(true);
        btn_c.onClick.AddListener(SceneChange);
    }

    public void InCorrect()
    {
        GameObject.Find("Canvas").transform.Find("InCorrect").gameObject.SetActive(true);
        btn_ic.onClick.AddListener(SceneChange);
    }

    public void AddStar()
    {
        star += 1;
        PlayerPrefs.SetInt("star", star);
    }

    public void CheckPlayerYear()
    {
        if (gameLevel == 1)
        {
            playerYear = 1;
        }
        else if (gameLevel == 3)
        {
            playerYear = 2;
        }
        else if (gameLevel == 5)
        {
            playerYear = 3;
        }
        else if (gameLevel == 7)
        {
            playerYear = 4;
        }
        else if (gameLevel == 9)
        {
            playerYear = 5;
        }
        else
        {
            playerYear = 6;
        }
    }

    public void UpdateQuiz1(int subjectId)
    {
        StartCoroutine(UpdateQuestions(playerId, playerYear, subjectId, 1, 1));
    }

    public void UpdateQuiz2(int subjectId)
    {
        StartCoroutine(UpdateQuestions(playerId, playerYear, subjectId, 1, 0));
    }

    private IEnumerator UpdateQuestions(string userId, int playerYear, int subjectId, int solved_Num, int corrected_Num)
    {
        var data = new QuizData
        {
            UserId = userId,
            Year = playerYear,
            SubjectId = subjectId,
            Solved_Num = solved_Num,
            Corrected_Num = corrected_Num
        };
        yield return CoSendWebRequest("MyPage/update", "PUT", data, (uwr) =>
        {
        });
    }

    public void SceneChange()
    {
        SceneManager.LoadScene("Story4");
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
}