using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TexDrawLib;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

public class SolutionManager : MonoBehaviour
{
    public static SolutionManager Instance;
    string _baseUrl = "https://localhost:7039/api";
    private void Awake()
    {
        Instance = this;
    }
    public void GetQuiz(int part, Action<string, string> callback)
    {
        SendGetRequest("quiz", part, callback);
    }
    public void SendGetRequest(string url, int part, Action<string, string> callback)
    {
        StartCoroutine(CoSendWebRequest(url, "GET", null, part, callback));
    }
    IEnumerator CoSendWebRequest(string url, string method, object obj, int part, Action<string, string> callback)
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
            callback.Invoke(null, null);
        }
        else
        {
            string responseJson = uwr.downloadHandler.text;
            List<Quiz> quizList = JsonConvert.DeserializeObject<List<Quiz>>(responseJson);

            foreach (Quiz quiz in quizList)
            {
                if (quiz.Part == part)
                {
                    string question = quiz.Problem;
                    string solution = TexChange(quiz.Equation, quiz.Answer);
                    callback.Invoke(question, solution);
                    yield break;
                }
            }
            callback.Invoke(null, null);
        }
    }
    static string fractionChange(string equation, string answer)
    {
        string fracPattern = @"Fraction\((\w+),\s*(\w+)\)";
        string fracReplacement = @"\frac{$1}{$2}";

        string solution = Regex.Replace(equation, fracPattern, fracReplacement) + " = " + Regex.Replace(answer, fracPattern, fracReplacement);
        return solution;
    }
    static string divmodChange(string equation, string answer)
    {
        string divEquationPattern = @"divmod\((\w+),\s*(\w+)\)";
        string divEquationReplacement = @"{$1} \div {$2}";
        string divAnswerPattern = @"\((\w+),\s*(\w+)\)";
        string divAnswerReplacement = @"{$1}... {$2}";

        string solution = Regex.Replace(equation, divEquationPattern, divEquationReplacement) + " = " + Regex.Replace(answer, divAnswerPattern, divAnswerReplacement);
        return solution;
    }
    static string MeanEquation(string input, string answer)
    {
        List<int> numbers = new List<int>();
        string pattern = @"\d+"; // 정규 표현식 패턴: 숫자에 매칭

        MatchCollection matches = Regex.Matches(input, pattern);

        foreach (Match match in matches)
        {
            numbers.Add(int.Parse(match.Value));
        }

        string mean_sum = "\\frac{" + String.Join(" + ", numbers) + "}{3} = " + answer + " \\\\ 평균은 자료의 값을 모두 더해 자료의 수로 나눈 값 입니다. 각 자료들을 모두 더하고 자료의 수인 3으로 나눠주면 답을 구할 수 있어요.";
        return mean_sum;
    }
    static (int, int) ExtractNumbers(string input) // gcd의 숫자 추출
    {
        string pattern1 = @"gcd\((\d+),\s*(\d+)\)";
        string pattern2 = @"lcm\((\d+),\s*(\d+)\)";
        Match match1 = Regex.Match(input, pattern1);
        Match match2 = Regex.Match(input, pattern2);

        if (match1.Success)
        {
            int num1 = int.Parse(match1.Groups[1].Value);
            int num2 = int.Parse(match1.Groups[2].Value);
            return (num1, num2);
        }
        else if (match2.Success)
        {
            int num1 = int.Parse(match2.Groups[1].Value);
            int num2 = int.Parse(match2.Groups[2].Value);
            return (num1, num2);
        }
        return default((int, int)); // 기본값 반환
    }
    public static int[] Factorization(int n) // 숫자 소인수분해
    {
        List<int> answer = new List<int>();
        for (int i = 2; i <= n; i++)
        {
            while (n % i == 0)
            {
                answer.Add(i);
                n /= i;
            }
        }
        return answer.ToArray();
    }
    //소인수분해한 결과로 gcd 구하기
    static (int[] result, int[] updatedArray1, int[] updatedArray2, List<List<int>> listOfLists) gcdDissolve(int[] array1, int[] array2)
    {
        List<int> resultList = new List<int>();
        List<int> array1List = new List<int>(array1);
        List<int> array2List = new List<int>(array2);
        List<List<int>> listOfLists = new List<List<int>>();

        int i = 0;
        while (i < array1List.Count)
        {
            int num = array1List[i];
            if (array2List.Contains(num))
            {
                // Remove from array1List and array2List
                array1List.RemoveAt(i);
                array2List.Remove(num);

                // Add to resultList
                resultList.Add(num);

                // 인수가 나누어진 이후의 값
                int array1sum = array1List.Aggregate(1, (acc, val) => acc * val);
                int array2sum = array2List.Aggregate(1, (acc, val) => acc * val);

                // listOfLists에 추가
                listOfLists.Add(new List<int> { num, array1sum, array2sum });

            }
            else
            {
                // Only move to next if no match was found, because otherwise we've already removed the current element
                i++;
            }
        }

        int[] result = resultList.ToArray();
        int[] updatedArray1 = array1List.ToArray();
        int[] updatedArray2 = array2List.ToArray();

        return (result, updatedArray1, updatedArray2, listOfLists);
    }
    public static string GCDVisualization(string equation, string answer) //TeX 수식 생성
    {
        (int num1, int num2) = ExtractNumbers(equation); //string에서 숫자 추출
        int[] number1 = Factorization(num1); //숫자 소인수분해
        int[] number2 = Factorization(num2); //숫자 소인수분해

        (int[] result, int[] array1result, int[] array2result, List<List<int>> listOfLists) = gcdDissolve(number1, number2);
        //result: 최대공약수
        //array1result: array 1 공통되지 않은 인수
        //array2result: array 2 공통되지 않은 인수
        if (listOfLists.Count == 0)
        {
            return "두 수는 서로소이기 떄문에 두 수의 최대공약수는 1이다.";
        }


        string sentence1 = "\\color{blue}{" + listOfLists[0][0] + "}\\color{black} \\underline{) " + num1.ToString() + " " + num2.ToString() + " }\\\\ ";
        string sentence2 = "";
        for (int i = 0; i < listOfLists.Count - 1; i++)
        {
            sentence2 += "\\color{blue}{" + listOfLists[i + 1][0] + "}\\color{black} \\underline{) " + listOfLists[i][1].ToString() + " " + listOfLists[i][2].ToString() + " }\\\\ ";
        }
        string sentence3 = " \\ \\ \\ \\ \\ " + listOfLists[listOfLists.Count - 1][1].ToString() + " " + listOfLists[listOfLists.Count - 1][2].ToString();
        string sentence4 = " \\\\ 위의 소인수분해에서 파란색으로 표시한 부분을 곱하면, 두 수의 최대공배수가 된다. 따라서 최대공배수는 \\color{blue}{" + answer + "}\\color{black}이다.";
        string sentence5 = sentence1 + sentence2 + sentence3 + sentence4;
        return sentence5;
    }
    public static string LCMVisualization(string equation, string answer) //TeX 수식 생성
    {
        (int num1, int num2) = ExtractNumbers(equation); //string에서 숫자 추출
        int[] number1 = Factorization(num1); //숫자 소인수분해
        int[] number2 = Factorization(num2); //숫자 소인수분해

        (int[] result, int[] array1result, int[] array2result, List<List<int>> listOfLists) = gcdDissolve(number1, number2);
        //result: 최소공배수
        //array1result: array 1 공통되지 않은 인수
        //array2result: array 2 공통되지 않은 인수
        if (listOfLists.Count == 0)
        {
            return "두 수는 서로소이기 때문에 두 수의 최소공배수는 " + num1.ToString() + " \\times" + num2.ToString() +" = " + (num1*num2).ToString() + " 이다." +
                "";
        }

        string sentence1 = "\\color{red}{" + listOfLists[0][0] + "}\\color{black} \\underline{) " + num1.ToString() + " " + num2.ToString() + " }\\\\ ";
        string sentence2 = "";
        for (int i = 0; i < listOfLists.Count - 1; i++)
        {
            sentence2 += "\\color{red}{" + listOfLists[i + 1][0] + "}\\color{black} \\underline{) " + listOfLists[i][1].ToString() + " " + listOfLists[i][2].ToString() + " }\\\\ ";
        }

        string sentence3 = " \\ \\ \\ \\ \\ \\color{red}{" + listOfLists[listOfLists.Count - 1][1].ToString() + " " + listOfLists[listOfLists.Count - 1][2].ToString() + "}\\color{black} ";


        string sentence4 = " \\\\ 위의 소인수분해에서 빨간색으로 표시한 부분을 곱하면, 두 수의 최소공약수가 된다. 따라서 최소공약수는 \\color{red}{" + answer + "}\\color{black}이다.";
        string sentence5 = sentence1 + sentence2 + sentence3 + sentence4;
        return sentence5;
    }
    static string twoNumberMin(int[] numbers)
    {
        int[] tens = { numbers[0] / 10, numbers[1] / 10 };
        int[] ones = { numbers[0] % 10, numbers[1] % 10 };
        string solution = "";
        if (numbers[0] == numbers[1])
        {
            solution += $"{numbers[0]} {numbers[1]} ";
            solution += " 두 숫자의 크기가 같습니다.";
            return solution;
        }

        solution += "\\color{red}{" + tens[0] + "}\\color{black}" + ones[0] +
                    ",  \\color{red}{" + tens[1] + "}\\color{black}" + ones[1];

        solution += " 십의 자리 숫자부터 비교합니다.\\\\ ";
        if (tens[0] == tens[1]) // same tens
        {
            solution += $"두 숫자의 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";
            solution += tens[0] + "\\color{red}{" + ones[0] + "},  \\color{black}{" + tens[1] + "}\\color{red}{" + ones[1] + "}\\color{black}";
            // compare ones 
            if (ones[0] < ones[1])
            {
                solution += $"  일의 자리 숫자에서 가장 작은 숫자는 {ones[0]}입니다.\\\\";
                solution += $"그래서, 가장 작은 숫자는 {numbers[0]}입니다";
            }
            else if (ones[1] < ones[0])
            {
                solution += $"  일의 자리 숫자에서 가장 작은 숫자는 {ones[1]}입니다.\\\\";
                solution += $"그래서, 가장 작은 숫자는 {numbers[1]}입니다";
            }
        }
        else // different tens
        {
            if (tens[0] < tens[1])
            {
                solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens[0]}입니다.\\\\";
                solution += $"그래서, 가장 작은 숫자는 {numbers[0]}입니다";
            }
            else
            {
                solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens[1]}입니다.\\\\";
                solution += $"그래서, 가장 작은 숫자는 {numbers[1]}입니다";
            }
        }

        return solution;
    }
    static List<int> FindSmallestWithIndexes(int[] arr)
    {
        List<int> indexes = new List<int>();
        if (arr.Length == 0)
        {
            return indexes;
        }

        int smallest = arr.Min();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == smallest)
            {
                indexes.Add(i);
            }
        }

        return indexes;
    }
    static string threeNumbersMin(int[] numbers)
    {

        int[] hundreds = { numbers[0] / 100, numbers[1] / 100, numbers[2] / 100 };
        int[] tens = { (numbers[0] / 10) % 10, (numbers[1] / 10) % 10, (numbers[2] / 10) % 10 };
        int[] ones = { numbers[0] % 10, numbers[1] % 10, numbers[2] % 10 };

        string solution = "";
        if ((numbers[0] == numbers[1]) && (numbers[1] == numbers[2])) // 3 same numbers
        {
            solution += $"{numbers[0]} {numbers[1]} {numbers[2]} ";
            solution += "세 숫자의 크기가 같습니다.";
            return solution;
        }

        solution += "\\color{red}{" + hundreds[0] + "}\\color{black}" + tens[0] + ones[0] +
                    ",  \\color{red}{" + hundreds[1] + "}\\color{black}" + tens[1] + ones[1] +
                    ",  \\color{red}{" + hundreds[2] + "}\\color{black}" + tens[2] + ones[2] + " 백의 자리 숫자부터 비교합니다.\\\\ ";

        if ((hundreds[0] == hundreds[1]) && (hundreds[1] == hundreds[2])) // 3 same hundreds
        {
            solution += $"세 숫자의 백의 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";
            solution += hundreds[0] + "\\color{red}{" + tens[0] + "}\\color{black}" + ones[0] +
                        ",  " + hundreds[1] + "\\color{red}{" + tens[1] + "}\\color{black}" + ones[1] +
                        ",  " + hundreds[2] + "\\color{red}{" + tens[2] + "}\\color{black}" + ones[2];
            List<int> ind = FindSmallestWithIndexes(tens);

            if (ind.Count == 2) // 3 same hundreds, 2 same tens 
            {
                int a = ind[0];
                int b = ind[1];
                if (tens[a] < tens[b])
                {
                    solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens[a]}입니다.\\\\";
                    solution += $"그래서, 가장 작은 숫자는 {numbers[a]}입니다.";
                }
                else if (tens[a] > tens[b])
                {
                    solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens[b]}입니다.\\\\";
                    solution += $"그래서, 가장 작은 숫자는 {numbers[b]}입니다.";
                }
                else
                { //same tens
                    solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens.Min()} 이고 ";
                    solution += $"{numbers[a]}, {numbers[b]}의 백과 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";
                    solution += "\\color{black}" + hundreds[a] + tens[a] + "\\color{red}{" + ones[a] + "}\\color{black}" + ",  " + hundreds[b] + tens[b] + "\\color{red}{" + ones[b] + "}\\color{black}";

                    solution += $"  일의 자리 숫자에서 가장 작은 숫자는 {ones.Min()} 입니다.\\\\";
                    solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
                }
            }
            else if (ind.Count == 1) // 3 same hundreds, 1 smallest tens
            {
                solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens.Min()} 입니다.\\\\";
                solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
            }
            else // 3 same hundreds, 3 same tens
            {
                solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens.Min()} 이고 ";
                solution += $"세 숫자의 백과 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";

                solution += "\\color{black}" + hundreds[0] + tens[0] + "\\color{red}{" + ones[0] + "}\\color{black}" + ",  " + hundreds[1] + tens[1] + "\\color{red}{" + ones[1] + "}\\color{black}" + ",  " + hundreds[2] + tens[2] + "\\color{red}{" + ones[2] + "}\\color{black}";
                solution += $"  일의 자리 숫자에서 가장 작은 숫자는 {ones.Min()} 입니다.\\\\";
                solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
            }
        }
        else if ((hundreds[0] != hundreds[1]) && (hundreds[0] != hundreds[2]) && (hundreds[1] != hundreds[2])) // 3 diff hundreds
        {
            solution += $"백의 자리 숫자에서 가장 작은 숫자는 {hundreds.Min()} 입니다.\\\\";
            solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
        }
        else // 2 same hundreds (both smallest)
        {
            List<int> indexes = FindSmallestWithIndexes(hundreds);
            int x = indexes[0];
            int y = indexes[1];

            solution += $"백의 자리 숫자에서 가장 작은 숫자는 {Math.Min(hundreds[x], hundreds[y])} 이고 ";
            solution += $"{numbers[x]}, {numbers[y]} 백의 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";
            solution += hundreds[x] + "\\color{red}{" + tens[x] + "}\\color{black}" + ones[x] +
                        ",  " + hundreds[y] + "\\color{red}{" + tens[y] + "}\\color{black}" + ones[y];

            if (tens[x] < tens[y])
            {
                solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens[x]}입니다.\\\\";
                solution += $"그래서, 가장 작은 숫자는 {numbers[x]}입니다.";
            }
            else if (tens[x] > tens[y])
            {
                solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens[y]}입니다.\\\\";
                solution += $"그래서, 가장 작은 숫자는 {numbers[y]}입니다.";
            }
            else
            {
                solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens[x]} 이고 ";
                solution += $"{numbers[x]}, {numbers[y]}의 백과 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";
                solution += "\\color{black}" + hundreds[x] + tens[x] + "\\color{red}{" + ones[x] + "}\\color{black}" + ",  " + hundreds[y] + tens[y] + "\\color{red}{" + ones[y] + "}\\color{black}";

                solution += $"  일의 자리 숫자에서 가장 작은 숫자는 {Math.Min(ones[x], ones[y])} 입니다.\\\\";
                solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
            }
        }

        return solution;
    }
    static string fourNumbersMin(int[] numbers)
    {
        int[] thousands = { numbers[0] / 1000, numbers[1] / 1000, numbers[2] / 1000, numbers[3] / 1000 };
        int[] hundreds = { (numbers[0] / 100) % 10, (numbers[1] / 100) % 10, (numbers[2] / 100) % 10, (numbers[3] / 100) % 10 };
        int[] tens = { (numbers[0] / 10) % 10, (numbers[1] / 10) % 10, (numbers[2] / 10) % 10, (numbers[3] / 10) % 10 };
        int[] ones = { numbers[0] % 10, numbers[1] % 10, numbers[2] % 10, numbers[3] % 10 };
        List<int> idx = new List<int>();
        int x = 0; int y = 0; int z = 0; int minOnes = 0;

        string solution = "";
        if ((numbers[0] == numbers[1]) && (numbers[1] == numbers[2]) && (numbers[2] == numbers[3])) // 4 same numbers
        {
            solution += $"{numbers[0]} {numbers[1]} {numbers[2]} {numbers[3]} ";
            solution += "네 숫자의 크기가 같습니다.";
            return solution;
        }

        solution += "\\color{red}{" + thousands[0] + "}\\color{black}" + hundreds[0] + tens[0] + ones[0] +
                    ",  \\color{red}{" + thousands[1] + "}\\color{black}" + hundreds[1] + tens[1] + ones[1] +
                    ",  \\color{red}{" + thousands[2] + "}\\color{black}" + hundreds[2] + tens[2] + ones[2] +
                    ",  \\color{red}{" + thousands[3] + "}\\color{black}" + hundreds[3] + tens[3] + ones[3] + " 천의 자리 숫자부터 비교합니다.\\\\ ";

        if ((thousands[0] == thousands[1]) && (thousands[1] == thousands[2]) && (thousands[2] == thousands[3])) //4 same thousands
        {
            solution += $"네 숫자의 천의 자리 숫자가 같아서 백 숫자를 비교합니다.\\\\ ";
            solution += thousands[0] + "\\color{red}{" + hundreds[0] + "}\\color{black}" + tens[0] + ones[0] +
                        ",  " + thousands[1] + "\\color{red}{" + hundreds[1] + "}\\color{black}" + tens[1] + ones[1] +
                        ",  " + thousands[2] + "\\color{red}{" + hundreds[2] + "}\\color{black}" + tens[2] + ones[2] +
                        ",  " + thousands[3] + "\\color{red}{" + hundreds[3] + "}\\color{black}" + tens[3] + ones[3];
            idx = FindSmallestWithIndexes(hundreds);

            if (idx.Count == 4)
            { //4 same thousands, 4 same hundreds
                solution += $"  네 숫자의 천과 백의 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";
                solution += $"{thousands[0]}" + $"{hundreds[0]}" + "\\color{red}{" + tens[0] + "}\\color{black}" + ones[0] +
                        ",  " + $"{thousands[1]}" + $"{hundreds[1]}" + "\\color{red}{" + tens[1] + "}\\color{black}" + ones[1] +
                        ",  " + $"{thousands[2]}" + $"{hundreds[2]}" + "\\color{red}{" + tens[2] + "}\\color{black}" + ones[2] +
                        ",  " + $"{thousands[3]}" + $"{hundreds[3]}" + "\\color{red}{" + tens[3] + "}\\color{black}" + ones[3];

                idx = FindSmallestWithIndexes(tens);
                if (idx.Count == 4)
                { //4 same thousands, 4 same hundreds, 4 same tens
                    solution += $"  네 숫자의 천,백,십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";
                    solution += $" {thousands[0]}" + $"{hundreds[0]}" + $"{tens[0]}" + "\\color{red}{" + ones[0] + "}\\color{black}" +
                        ",  " + $" {thousands[1]}" + $"{hundreds[1]}" + $"{tens[1]}" + "\\color{red}{" + ones[1] + "}\\color{black}" +
                        ",  " + $" {thousands[2]}" + $"{hundreds[2]}" + $"{tens[2]}" + "\\color{red}{" + ones[2] + "}\\color{black}" +
                        ",  " + $" {thousands[3]}" + $"{hundreds[3]}" + $"{tens[3]}" + "\\color{red}{" + ones[3] + "}\\color{black}";

                    solution += $"  일의 자리 숫자에서 가장 작은 숫자는 {ones.Min()} 입니다.\\\\";
                    solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
                }
                else if (idx.Count == 3)
                { //4 same thousands, 4 same hundreds, 3 same tens
                    x = idx[0];
                    y = idx[1];
                    z = idx[2];
                    solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens[x]} 이고";
                    solution += $" 세 숫자의 천, 백, 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";

                    solution += $"{thousands[x]}" + $"{hundreds[x]}" + $"{tens[x]}" + "\\color{red}{" + ones[x] + "}\\color{black}" +
                        ",  " + $"{thousands[y]}" + $"{hundreds[y]}" + $"{tens[y]}" + "\\color{red}{" + ones[y] + "}\\color{black}" +
                        ",  " + $"{thousands[z]}" + $"{hundreds[z]}" + $"{tens[z]}" + "\\color{red}{" + ones[z] + "}\\color{black}";

                    minOnes = new int[] { ones[x], ones[y], ones[z] }.Min();
                    solution += $"  일의 자리 숫자에서 가장 작은 숫자는 {minOnes} 입니다.\\\\";
                    solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
                }
                else if (idx.Count == 2)
                { //4 same thousands, 4 same hundreds, 2 same tens
                    x = idx[0];
                    y = idx[1];
                    solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens[x]} 이고";
                    solution += $" 두 숫자의 천,백,십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";
                    solution += $"{thousands[x]}" + $"{hundreds[x]}" + $"{tens[x]}" + "\\color{red}{" + ones[x] + "}\\color{black}" +
                        ",  " + $"{thousands[y]}" + $"{hundreds[y]}" + $"{tens[y]}" + "\\color{red}{" + ones[y] + "}\\color{black}";

                    solution += $"  일의 자리 숫자에서 가장 작은 숫자는 {Math.Min(ones[x], ones[y])} 입니다.\\\\";
                    solution += $"그래서, 가장 작은 숫자는 {numbers[Array.IndexOf(ones, Math.Min(ones[x], ones[y]))]}입니다.";
                }
                else
                { //4 same thousands, 4 same hundreds, 1 smallest tens
                    int smallestTensIdx = idx[0];
                    solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens[smallestTensIdx]} 입니다.\\\\";
                    solution += $"그래서, 가장 작은 숫자는 {numbers[smallestTensIdx]}입니다.";
                }
            }
            else if (idx.Count == 3) //4 same thousands, 3 same hundreds
            {
                x = idx[0];
                y = idx[1];
                z = idx[2];
                solution += $"  백의 자리 숫자에서 가장 작은 숫자는 {hundreds[x]} 이고";
                solution += $" 세 숫자의 천,백 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";
                solution += $"{thousands[x]}" + $"{hundreds[x]}" + "\\color{red}{" + tens[x] + "}\\color{black}" + ones[x] +
                        ",  " + $"{thousands[y]}" + $"{hundreds[y]}" + "\\color{red}{" + tens[y] + "}\\color{black}" + ones[y] +
                        ",  " + $"{thousands[z]}" + $"{hundreds[z]}" + "\\color{red}{" + tens[z] + "}\\color{black}" + ones[z];

                idx = FindSmallestWithIndexes(new int[] { tens[x], tens[y], tens[z] });
                if (idx.Count == 3) //4 same thousands, 3 same hundreds, 3 same tens
                {
                    solution += $"  세 숫자의 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";
                    minOnes = new int[] { ones[x], ones[y], ones[z] }.Min();
                    solution += $"  일의 자리 숫자에서 가장 작은 숫자는 {minOnes} 입니다.\\\\";
                    solution += $"그래서, 가장 작은 숫자는{numbers.Min()}입니다.";
                }
                else if (idx.Count == 2) //4 same thousands, 3 same hundreds, 2 same tens
                {
                    x = idx[0];
                    y = idx[1];
                    solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens[x]} 이고";
                    solution += $" 두 숫자의 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";
                    solution += $"{thousands[x]}" + $"{hundreds[x]}" + $"{tens[x]}" + "\\color{red}{" + ones[x] + "}\\color{black}" +
                        ",  " + $"{thousands[y]}" + $"{hundreds[y]}" + $"{tens[y]}" + "\\color{red}{" + ones[y] + "}\\color{black}";

                    solution += $"  일의 자리 숫자에서 가장 작은 숫자는 {Math.Min(ones[x], ones[y])} 입니다.\\\\";
                    solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
                }
                else if (idx.Count == 1)//4 same thousands, 3 same hundreds, 1 smallest tens
                {
                    x = idx[0];
                    solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {tens[x]} 입니다.\\\\";
                    solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
                }
            }
            else if (idx.Count == 2) //4 same thousands, 2 same hundreds
            {
                x = idx[0];
                y = idx[1];
                solution += $"  백의 자리 숫자에서 가장 작은 숫자는 {hundreds[x]} 이고";
                solution += $" 두 숫자의 천,백 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";
                solution += $"{thousands[x]}" + $"{hundreds[x]}" + "\\color{red}{" + tens[x] + "}\\color{black}" + ones[x] +
                        ",  " + $"{thousands[y]}" + $"{hundreds[y]}" + "\\color{red}{" + tens[y] + "}\\color{black}" + ones[y];

                solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {Math.Min(tens[x], tens[y])} 입니다.\\\\";
                solution += $"그래서, 가장 작은 숫자는 {numbers[Array.IndexOf(tens, Math.Min(tens[x], tens[y]))]}입니다.";
            }
            else//4 same thousands, 1 smallest hundreds
            {
                x = idx[0];
                solution += $"  백의 자리 숫자에서 가장 작은 숫자는 {hundreds[x]} 입니다.\\\\";
                solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
            }
        }


        else
        {
            idx = FindSmallestWithIndexes(thousands);
            if (idx.Count == 3) // 3 same thousands
            {
                x = idx[0];
                y = idx[1];
                z = idx[2];
                int[] three_STH_Numbers = { numbers[x], numbers[y], numbers[z] };
                int[] three_STH_thousands = { three_STH_Numbers[0] / 1000, three_STH_Numbers[1] / 1000, three_STH_Numbers[2] / 1000 };
                int[] three_STH_hundreds = { (three_STH_Numbers[0] / 100) % 10, (three_STH_Numbers[1] / 100) % 10, (three_STH_Numbers[2] / 100) % 10 };
                int[] three_STH_tens = { (three_STH_Numbers[0] / 10) % 10, (three_STH_Numbers[1] / 10) % 10, (three_STH_Numbers[2] / 10) % 10 };
                int[] three_STH_ones = { three_STH_Numbers[0] % 10, three_STH_Numbers[1] % 10, three_STH_Numbers[2] % 10 };


                solution += $"  천의 자리 숫자에서 가장 작은 숫자는 {thousands[x]} 이고";
                solution += $" 세 숫자의 천의 자리 숫자가 같아서 백 숫자를 비교합니다.\\\\ ";
                solution += three_STH_thousands[0] + "\\color{red}{" + three_STH_hundreds[0] + "}\\color{black}" + $"{three_STH_tens[0]}" + $"{three_STH_ones[0]}" +
                        ",  " + three_STH_thousands[1] + "\\color{red}{" + three_STH_hundreds[1] + "}\\color{black}" + $"{three_STH_tens[1]}" + $"{three_STH_ones[1]}" +
                        ",  " + three_STH_thousands[2] + "\\color{red}{" + three_STH_hundreds[2] + "}\\color{black}" + $"{three_STH_tens[2]}" + $"{three_STH_ones[2]}";

                idx = FindSmallestWithIndexes(three_STH_hundreds); //find smallest hundreds
                if (idx.Count == 3) // 3 same thousands, 3 same hundreds
                {
                    solution += $"  백의 자리 숫자에서 가장 작은 숫자는 {three_STH_hundreds[0]} 이고";
                    solution += $" 세 숫자의 백의 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";

                    solution += $"{three_STH_thousands[0]}" + $"{three_STH_hundreds[0]}" + "\\color{red}{" + three_STH_tens[0] + "}\\color{black}" + $"{three_STH_ones[0]}" +
                        ",  " + $"{three_STH_thousands[1]}" + $"{three_STH_hundreds[1]}" + "\\color{red}{" + three_STH_tens[1] + "}\\color{black}" + $"{three_STH_ones[1]}" +
                        ",  " + $"{three_STH_thousands[2]}" + $"{three_STH_hundreds[2]}" + "\\color{red}{" + three_STH_tens[2] + "}\\color{black}" + $"{three_STH_ones[2]}";

                    idx = FindSmallestWithIndexes(three_STH_tens); //find smallest tens
                    if (idx.Count == 3) //3 same thousands, 3 same hundreds, 3 same tens
                    {
                        solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {three_STH_tens[0]} 이고";
                        solution += $" 세 숫자의 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";

                        minOnes = three_STH_ones.Min();

                        solution += $"{three_STH_thousands[0]}" + $"{three_STH_hundreds[0]}" + $"{three_STH_tens[0]}" + "\\color{red}{" + three_STH_ones[0] + "}\\color{black}" +
                        ",  " + $"{three_STH_thousands[1]}" + $"{three_STH_hundreds[1]}" + $"{three_STH_tens[1]}" + "\\color{red}{" + three_STH_ones[1] + "}\\color{black}" +
                        ",  " + $"{three_STH_thousands[2]}" + $"{three_STH_hundreds[2]}" + $"{three_STH_tens[2]}" + "\\color{red}{" + three_STH_ones[2] + "}\\color{black}";


                        solution += $"  일의 자리 숫자에서 가장 작은 숫자는 {minOnes} 입니다.\\\\";
                        solution += $"그래서, 가장 작은 숫자는{numbers.Min()}입니다.";
                    }
                    else if (idx.Count == 2)
                    { //3 same thousands, 3 same hundreds, 2 same tens
                        x = idx[0];
                        y = idx[1];
                        solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {three_STH_tens[x]} 이고";
                        solution += $" 두 숫자의 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";

                        minOnes = new int[] { three_STH_ones[x], three_STH_ones[y] }.Min();
                        solution += $"{three_STH_thousands[x]}" + $"{three_STH_hundreds[x]}" + $"{three_STH_tens[x]}" + "\\color{red}{" + three_STH_ones[x] + "}\\color{black}" +
                        ",  " + $"{three_STH_thousands[y]}" + $"{three_STH_hundreds[y]}" + $"{three_STH_tens[y]}" + "\\color{red}{" + three_STH_ones[y] + "}\\color{black}";

                        solution += $"  일의 자리 숫자에서 가장 작은 숫자는 {minOnes} 입니다.\\\\";
                        solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
                    }
                    else
                    { //3 same thousands, 3 same hundreds, 1 smallest tens
                        x = idx[0];
                        solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {three_STH_tens[x]} 입니다.\\\\";
                        solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
                    }

                }
                else if (idx.Count == 2)// 3 same thousands, 2 same hundreds
                {
                    x = idx[0];
                    y = idx[1];
                    solution += $"  백의 자리 숫자에서 가장 작은 숫자는 {three_STH_hundreds[x]} 이고";
                    solution += $" 두 숫자의 백의 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";
                    solution += $"{three_STH_thousands[x]}" + $"{three_STH_hundreds[x]}" + "\\color{red}{" + three_STH_tens[x] + "}\\color{black}" + $"{three_STH_ones[x]}" +
                        ",  " + $"{three_STH_thousands[y]}" + $"{three_STH_hundreds[y]}" + "\\color{red}{" + three_STH_tens[y] + "}\\color{black}" + $"{three_STH_ones[y]}";

                    idx = FindSmallestWithIndexes(new int[] { three_STH_tens[x], three_STH_tens[y] });
                    if (idx.Count == 2)
                    {//3 same thousands, 2 same hundreds, 2 same tens
                        x = idx[0];
                        y = idx[1];
                        solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {three_STH_tens[x]} 이고";
                        solution += $" 두 숫자의 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";

                        minOnes = new int[] { three_STH_ones[x], three_STH_ones[y] }.Min();
                        solution += $"{three_STH_thousands[x]}" + $"{three_STH_hundreds[x]}" + $"{three_STH_tens[x]}" + "\\color{red}{" + three_STH_ones[x] + "}\\color{black}" +
                        ",  " + $"{three_STH_thousands[y]}" + $"{three_STH_hundreds[y]}" + $"{three_STH_tens[y]}" + "\\color{red}{" + three_STH_ones[y] + "}\\color{black}";

                        solution += $"  일의 자리 숫자에서 가장 작은 숫자는 {minOnes} 입니다.\\\\";
                        solution += $"그래서, 가장 작은 숫자는{numbers.Min()}입니다.";
                    }
                    else if (idx.Count == 1)
                    {//3 same thousands, 2 same hundreds, 1 smallest tens
                        x = idx[0];
                        solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {three_STH_tens[x]} 입니다.\\\\";
                        solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
                    }

                }
                else// 3 same thousands, 1 smallest hundreds
                {
                    x = idx[0];
                    solution += $"  백의 자리 숫자에서 가장 작은 숫자는 {three_STH_hundreds[x]} 입니다.\\\\";
                    solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";

                }
            }

            else if (idx.Count == 2) // 2 same thousands
            {
                x = idx[0];
                y = idx[1];
                int[] two_STH_Numbers = { numbers[x], numbers[y] };
                int[] two_STH_thousands = { two_STH_Numbers[0] / 1000, two_STH_Numbers[1] / 1000 };
                int[] two_STH_hundreds = { (two_STH_Numbers[0] / 100) % 10, (two_STH_Numbers[1] / 100) % 10 };
                int[] two_STH_tens = { (two_STH_Numbers[0] / 10) % 10, (two_STH_Numbers[1] / 10) % 10 };
                int[] two_STH_ones = { two_STH_Numbers[0] % 10, two_STH_Numbers[1] % 10 };
                solution += $"  천의 자리 숫자에서 가장 작은 숫자는 {thousands[x]} 이고";
                solution += $" 두 숫자의 천의 자리 숫자가 같아서 백 숫자를 비교합니다.\\\\ ";

                solution += two_STH_thousands[0] + "\\color{red}{" + two_STH_hundreds[0] + "}\\color{black}" + $"{two_STH_tens[0]}" + $"{two_STH_ones[0]}" +
                        ",  " + two_STH_thousands[1] + "\\color{red}{" + two_STH_hundreds[1] + "}\\color{black}" + $"{two_STH_tens[1]}" + $"{two_STH_ones[1]}";

                idx = FindSmallestWithIndexes(two_STH_hundreds);//find smallest hundreds
                if (idx.Count == 2) // 2 same thousands, 2 same hundreds
                {
                    solution += $"  백의 자리 숫자에서 가장 작은 숫자는 {two_STH_hundreds[x]} 이고";
                    solution += $" 두 숫자의 백의 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";
                    solution += $"{two_STH_thousands[0]}" + $"{two_STH_hundreds[0]}" + "\\color{red}{" + two_STH_tens[0] + "}\\color{black}" + $"{two_STH_ones[0]}" +
                        ",  " + $"{two_STH_thousands[1]}" + $"{two_STH_hundreds[1]}" + "\\color{red}{" + two_STH_tens[1] + "}\\color{black}" + $"{two_STH_ones[1]}";

                    idx = FindSmallestWithIndexes(two_STH_tens); //find smallest tens
                    if (idx.Count == 2) //2 same thousands, 2 same hundreds, 2 same tens
                    {
                        solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {two_STH_tens[0]} 이고";
                        solution += $" 두 숫자의 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";

                        minOnes = two_STH_ones.Min();
                        solution += $"{two_STH_thousands[0]}" + $"{two_STH_hundreds[0]}" + $"{two_STH_tens[0]}" + "\\color{red}{" + two_STH_ones[0] + "}\\color{black}" +
                        ",  " + $"{two_STH_thousands[1]}" + $"{two_STH_hundreds[1]}" + $"{two_STH_tens[1]}" + "\\color{red}{" + two_STH_ones[1] + "}\\color{black}";

                        solution += $"  일의 자리 숫자에서 가장 작은 숫자는 {minOnes} 입니다.\\\\";
                        solution += $"그래서, 가장 작은 숫자는{numbers.Min()}입니다.";
                    }
                    else if (idx.Count == 1)
                    {//2 same thousands, 2 same hundreds, 1 smallest tens
                        x = idx[0];
                        solution += $"  십의 자리 숫자에서 가장 작은 숫자는 {two_STH_tens[x]} 입니다.\\\\";
                        solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
                    }
                }
                else if (idx.Count == 1)
                {// 2 same thousands, 1 smallest hundreds
                    solution += $"  백의 자리 숫자에서 가장 작은 숫자는 {two_STH_hundreds.Min()} 입니다.\\\\";
                    solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
                }

            }
            else if (idx.Count == 1) //1 smallest thousands
            {
                x = idx[0];
                solution += $"  천의 자리 숫자에서 가장 작은 숫자는 {thousands[x]} 입니다.\\\\";
                solution += $"그래서, 가장 작은 숫자는 {numbers.Min()}입니다.";
            }
        }

        return solution;
    }
    static string twoNumberMax(int[] numbers)
    {
        int[] tens = { numbers[0] / 10, numbers[1] / 10 };
        int[] ones = { numbers[0] % 10, numbers[1] % 10 };
        string solution = "";
        if (numbers[0] == numbers[1])
        {
            solution += $"{numbers[0]} {numbers[1]} ";
            solution += " 두 숫자의 크기가 같습니다.";
            return solution;
        }

        solution += "\\color{red}{" + tens[0] + "}\\color{black}" + ones[0] +
                    ",  \\color{red}{" + tens[1] + "}\\color{black}" + ones[1];

        solution += " 십의 자리 숫자부터 비교합니다.\\\\ ";
        if (tens[0] == tens[1]) // same tens
        {
            solution += $"두 숫자의 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";
            solution += tens[0] + "\\color{red}{" + ones[0] + "},  \\color{black}{" + tens[1] + "}\\color{red}{" + ones[1] + "}\\color{black}";
            // compare ones 
            if (ones[0] > ones[1])
            {
                solution += $"  일의 자리 숫자에서 가장 큰 숫자는 {ones[0]}입니다.\\\\";
                solution += $"그래서, 가장 큰 숫자는 {numbers[0]}입니다";
            }
            else if (ones[1] > ones[0])
            {
                solution += $"  일의 자리 숫자에서 가장 큰 숫자는 {ones[1]}입니다.\\\\";
                solution += $"그래서, 가장 큰 숫자는 {numbers[1]}입니다";
            }
        }
        else // different tens
        {
            if (tens[0] > tens[1])
            {
                solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens[0]}입니다.\\\\";
                solution += $"그래서, 가장 큰 숫자는 {numbers[0]}입니다";
            }
            else
            {
                solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens[1]}입니다.\\\\";
                solution += $"그래서, 가장 큰 숫자는 {numbers[1]}입니다";
            }
        }

        return solution;
    }
    static List<int> FindLargestWithIndexes(int[] arr)
    {
        List<int> indexes = new List<int>();
        if (arr.Length == 0)
        {
            return indexes;
        }

        int largest = arr.Max();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == largest)
            {
                indexes.Add(i);
            }
        }

        return indexes;
    }
    static string threeNumbersMax(int[] numbers)
    {

        int[] hundreds = { numbers[0] / 100, numbers[1] / 100, numbers[2] / 100 };
        int[] tens = { (numbers[0] / 10) % 10, (numbers[1] / 10) % 10, (numbers[2] / 10) % 10 };
        int[] ones = { numbers[0] % 10, numbers[1] % 10, numbers[2] % 10 };

        string solution = "";
        if ((numbers[0] == numbers[1]) && (numbers[1] == numbers[2])) // 3 same numbers
        {
            solution += $"{numbers[0]} {numbers[1]} {numbers[2]} ";
            solution += "세 숫자의 크기가 같습니다.";
            return solution;
        }

        solution += "\\color{red}{" + hundreds[0] + "}\\color{black}" + tens[0] + ones[0] +
                    ",  \\color{red}{" + hundreds[1] + "}\\color{black}" + tens[1] + ones[1] +
                    ",  \\color{red}{" + hundreds[2] + "}\\color{black}" + tens[2] + ones[2] + " 백의 자리 숫자부터 비교합니다.\\\\ ";

        if ((hundreds[0] == hundreds[1]) && (hundreds[1] == hundreds[2])) // 3 same hundreds
        {
            solution += $"세 숫자의 백의 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";
            solution += hundreds[0] + "\\color{red}{" + tens[0] + "}\\color{black}" + ones[0] +
                        ",  " + hundreds[1] + "\\color{red}{" + tens[1] + "}\\color{black}" + ones[1] +
                        ",  " + hundreds[2] + "\\color{red}{" + tens[2] + "}\\color{black}" + ones[2];
            List<int> ind = FindLargestWithIndexes(tens);

            if (ind.Count == 2) // 3 same hundreds, 2 same tens 
            {
                int a = ind[0];
                int b = ind[1];
                if (tens[a] > tens[b])
                {
                    solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens[a]}입니다.\\\\";
                    solution += $"그래서, 가장 큰 숫자는 {numbers[a]}입니다.";
                }
                else if (tens[a] < tens[b])
                {
                    solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens[b]}입니다.\\\\";
                    solution += $"그래서, 가장 큰 숫자는 {numbers[b]}입니다.";
                }
                else
                { //same tens
                    solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens.Max()} 이고 ";
                    solution += $"{numbers[a]}, {numbers[b]}의 백과 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";
                    solution += "\\color{black}" + hundreds[a] + tens[a] + "\\color{red}{" + ones[a] + "}\\color{black}" + ",  " + hundreds[b] + tens[b] + "\\color{red}{" + ones[b] + "}\\color{black}";

                    solution += $"  일의 자리 숫자에서 가장 큰 숫자는 {Math.Max(ones[a], ones[b])} 입니다.\\\\";
                    solution += $"그래서, 가장 작은 숫자는 {numbers.Max()}입니다.";
                }
            }
            else if (ind.Count == 1) // 3 same hundreds, 1 smallest tens
            {
                solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens.Max()} 입니다.\\\\";
                solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
            }
            else // 3 same hundreds, 3 same tens
            {
                solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens.Max()} 이고 ";
                solution += $"세 숫자의 백과 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";

                solution += "\\color{black}" + hundreds[0] + tens[0] + "\\color{red}{" + ones[0] + "}\\color{black}" + ",  " + hundreds[1] + tens[1] + "\\color{red}{" + ones[1] + "}\\color{black}" + ",  " + hundreds[2] + tens[2] + "\\color{red}{" + ones[2] + "}\\color{black}";
                solution += $"  일의 자리 숫자에서 가장 큰 숫자는 {ones.Max()} 입니다.\\\\";
                solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
            }
        }
        else if ((hundreds[0] != hundreds[1]) && (hundreds[0] != hundreds[2]) && (hundreds[1] != hundreds[2])) // 3 diff hundreds
        {
            solution += $"백의 자리 숫자에서 가장 큰 숫자는 {hundreds.Max()} 입니다.\\\\";
            solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
        }
        else // 2 same hundreds (both smallest)
        {
            List<int> indexes = FindLargestWithIndexes(hundreds);
            int x = indexes[0];
            int y = indexes[1];

            solution += $"백의 자리 숫자에서 가장 큰 숫자는 {Math.Max(hundreds[x], hundreds[y])} 이고 ";
            solution += $"{numbers[x]}, {numbers[y]} 백의 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";
            solution += hundreds[x] + "\\color{red}{" + tens[x] + "}\\color{black}" + ones[x] +
                        ",  " + hundreds[y] + "\\color{red}{" + tens[y] + "}\\color{black}" + ones[y];

            if (tens[x] > tens[y])
            {
                solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens[x]}입니다.\\\\";
                solution += $"그래서, 가장 큰 숫자는 {numbers[x]}입니다.";
            }
            else if (tens[x] < tens[y])
            {
                solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens[y]}입니다.\\\\";
                solution += $"그래서, 가장 큰 숫자는 {numbers[y]}입니다.";
            }
            else
            {
                solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens[x]} 이고 ";
                solution += $"{numbers[x]}, {numbers[y]}의 백과 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";
                solution += "\\color{black}" + hundreds[x] + tens[x] + "\\color{red}{" + ones[x] + "}\\color{black}" + ",  " + hundreds[y] + tens[y] + "\\color{red}{" + ones[y] + "}\\color{black}";

                solution += $"  일의 자리 숫자에서 가장 큰 숫자는 {Math.Max(ones[x], ones[y])} 입니다.\\\\";
                solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
            }
        }

        return solution;
    }
    static string fourNumbersMax(int[] numbers)
    {
        int[] thousands = { numbers[0] / 1000, numbers[1] / 1000, numbers[2] / 1000, numbers[3] / 1000 };
        int[] hundreds = { (numbers[0] / 100) % 10, (numbers[1] / 100) % 10, (numbers[2] / 100) % 10, (numbers[3] / 100) % 10 };
        int[] tens = { (numbers[0] / 10) % 10, (numbers[1] / 10) % 10, (numbers[2] / 10) % 10, (numbers[3] / 10) % 10 };
        int[] ones = { numbers[0] % 10, numbers[1] % 10, numbers[2] % 10, numbers[3] % 10 };
        List<int> idx = new List<int>();
        int x = 0; int y = 0; int z = 0; int maxOnes = 0;

        string solution = "";
        if ((numbers[0] == numbers[1]) && (numbers[1] == numbers[2]) && (numbers[2] == numbers[3])) // 4 same numbers
        {
            solution += $"{numbers[0]} {numbers[1]} {numbers[2]} {numbers[3]} ";
            solution += "네 숫자의 크기가 같습니다.";
            return solution;
        }

        solution += "\\color{red}{" + thousands[0] + "}\\color{black}" + hundreds[0] + tens[0] + ones[0] +
                    ",  \\color{red}{" + thousands[1] + "}\\color{black}" + hundreds[1] + tens[1] + ones[1] +
                    ",  \\color{red}{" + thousands[2] + "}\\color{black}" + hundreds[2] + tens[2] + ones[2] +
                    ",  \\color{red}{" + thousands[3] + "}\\color{black}" + hundreds[3] + tens[3] + ones[3] + " 천의 자리 숫자부터 비교합니다.\\\\ ";

        if ((thousands[0] == thousands[1]) && (thousands[1] == thousands[2]) && (thousands[2] == thousands[3])) //4 same thousands
        {
            solution += $"네 숫자의 천의 자리 숫자가 같아서 백 숫자를 비교합니다.\\\\ ";
            solution += thousands[0] + "\\color{red}{" + hundreds[0] + "}\\color{black}" + tens[0] + ones[0] +
                        ",  " + thousands[1] + "\\color{red}{" + hundreds[1] + "}\\color{black}" + tens[1] + ones[1] +
                        ",  " + thousands[2] + "\\color{red}{" + hundreds[2] + "}\\color{black}" + tens[2] + ones[2] +
                        ",  " + thousands[3] + "\\color{red}{" + hundreds[3] + "}\\color{black}" + tens[3] + ones[3];
            idx = FindLargestWithIndexes(hundreds);

            if (idx.Count == 4)
            { //4 same thousands, 4 same hundreds
                solution += $"  네 숫자의 천과 백의 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";
                solution += $"{thousands[0]}" + $"{hundreds[0]}" + "\\color{red}{" + tens[0] + "}\\color{black}" + ones[0] +
                        ",  " + $"{thousands[1]}" + $"{hundreds[1]}" + "\\color{red}{" + tens[1] + "}\\color{black}" + ones[1] +
                        ",  " + $"{thousands[2]}" + $"{hundreds[2]}" + "\\color{red}{" + tens[2] + "}\\color{black}" + ones[2] +
                        ",  " + $"{thousands[3]}" + $"{hundreds[3]}" + "\\color{red}{" + tens[3] + "}\\color{black}" + ones[3];

                idx = FindLargestWithIndexes(tens);
                if (idx.Count == 4)
                { //4 same thousands, 4 same hundreds, 4 same tens
                    solution += $"  네 숫자의 천,백,십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";
                    solution += $" {thousands[0]}" + $"{hundreds[0]}" + $"{tens[0]}" + "\\color{red}{" + ones[0] + "}\\color{black}" +
                        ",  " + $" {thousands[1]}" + $"{hundreds[1]}" + $"{tens[1]}" + "\\color{red}{" + ones[1] + "}\\color{black}" +
                        ",  " + $" {thousands[2]}" + $"{hundreds[2]}" + $"{tens[2]}" + "\\color{red}{" + ones[2] + "}\\color{black}" +
                        ",  " + $" {thousands[3]}" + $"{hundreds[3]}" + $"{tens[3]}" + "\\color{red}{" + ones[3] + "}\\color{black}";

                    solution += $"  일의 자리 숫자에서 가장 큰 숫자는 {ones.Max()} 입니다.\\\\";
                    solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
                }
                else if (idx.Count == 3)
                { //4 same thousands, 4 same hundreds, 3 same tens
                    x = idx[0];
                    y = idx[1];
                    z = idx[2];
                    solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens[x]} 이고";
                    solution += $" 세 숫자의 천, 백, 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";

                    solution += $"{thousands[x]}" + $"{hundreds[x]}" + $"{tens[x]}" + "\\color{red}{" + ones[x] + "}\\color{black}" +
                        ",  " + $"{thousands[y]}" + $"{hundreds[y]}" + $"{tens[y]}" + "\\color{red}{" + ones[y] + "}\\color{black}" +
                        ",  " + $"{thousands[z]}" + $"{hundreds[z]}" + $"{tens[z]}" + "\\color{red}{" + ones[z] + "}\\color{black}";

                    maxOnes = new int[] { ones[x], ones[y], ones[z] }.Max();
                    solution += $"  일의 자리 숫자에서 가장 큰 숫자는 {maxOnes} 입니다.\\\\";
                    solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
                }
                else if (idx.Count == 2)
                { //4 same thousands, 4 same hundreds, 2 same tens
                    x = idx[0];
                    y = idx[1];
                    solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens[x]} 이고";
                    solution += $" 두 숫자의 천,백,십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";
                    solution += $"{thousands[x]}" + $"{hundreds[x]}" + $"{tens[x]}" + "\\color{red}{" + ones[x] + "}\\color{black}" +
                        ",  " + $"{thousands[y]}" + $"{hundreds[y]}" + $"{tens[y]}" + "\\color{red}{" + ones[y] + "}\\color{black}";

                    solution += $"  일의 자리 숫자에서 가장 큰 숫자는 {Math.Max(ones[x], ones[y])} 입니다.\\\\";
                    solution += $"그래서, 가장 큰 숫자는 {numbers[Array.IndexOf(ones, Math.Max(ones[x], ones[y]))]}입니다.";
                }
                else
                { //4 same thousands, 4 same hundreds, 1 smallest tens
                    int largestTensIdx = idx[0];
                    solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens[largestTensIdx]} 입니다.\\\\";
                    solution += $"그래서, 가장 큰 숫자는 {numbers[largestTensIdx]}입니다.";
                }
            }
            else if (idx.Count == 3) //4 same thousands, 3 same hundreds
            {
                x = idx[0];
                y = idx[1];
                z = idx[2];
                solution += $"  백의 자리 숫자에서 가장 큰 숫자는 {hundreds[x]} 이고";
                solution += $" 세 숫자의 천,백 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";
                solution += $"{thousands[x]}" + $"{hundreds[x]}" + "\\color{red}{" + tens[x] + "}\\color{black}" + ones[x] +
                        ",  " + $"{thousands[y]}" + $"{hundreds[y]}" + "\\color{red}{" + tens[y] + "}\\color{black}" + ones[y] +
                        ",  " + $"{thousands[z]}" + $"{hundreds[z]}" + "\\color{red}{" + tens[z] + "}\\color{black}" + ones[z];

                idx = FindLargestWithIndexes(new int[] { tens[x], tens[y], tens[z] });
                if (idx.Count == 3) //4 same thousands, 3 same hundreds, 3 same tens
                {
                    solution += $"  세 숫자의 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";
                    maxOnes = new int[] { ones[x], ones[y], ones[z] }.Max();
                    solution += $"  일의 자리 숫자에서 가장 큰 숫자는 {maxOnes} 입니다.\\\\";
                    solution += $"그래서, 가장 큰 숫자는{numbers.Max()}입니다.";
                }
                else if (idx.Count == 2) //4 same thousands, 3 same hundreds, 2 same tens
                {
                    x = idx[0];
                    y = idx[1];
                    solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens[x]} 이고";
                    solution += $" 두 숫자의 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";
                    solution += $"{thousands[x]}" + $"{hundreds[x]}" + $"{tens[x]}" + "\\color{red}{" + ones[x] + "}\\color{black}" +
                        ",  " + $"{thousands[y]}" + $"{hundreds[y]}" + $"{tens[y]}" + "\\color{red}{" + ones[y] + "}\\color{black}";

                    solution += $"  일의 자리 숫자에서 가장 큰 숫자는 {Math.Max(ones[x], ones[y])} 입니다.\\\\";
                    solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
                }
                else if (idx.Count == 1)//4 same thousands, 3 same hundreds, 1 smallest tens
                {
                    x = idx[0];
                    solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {tens[x]} 입니다.\\\\";
                    solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
                }
            }
            else if (idx.Count == 2) //4 same thousands, 2 same hundreds
            {
                x = idx[0];
                y = idx[1];
                solution += $"  백의 자리 숫자에서 가장 큰 숫자는 {hundreds[x]} 이고";
                solution += $" 두 숫자의 천,백 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";
                solution += $"{thousands[x]}" + $"{hundreds[x]}" + "\\color{red}{" + tens[x] + "}\\color{black}" + ones[x] +
                        ",  " + $"{thousands[y]}" + $"{hundreds[y]}" + "\\color{red}{" + tens[y] + "}\\color{black}" + ones[y];

                solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {Math.Max(tens[x], tens[y])} 입니다.\\\\";
                solution += $"그래서, 가장 큰 숫자는 {numbers[Array.IndexOf(tens, Math.Max(tens[x], tens[y]))]}입니다.";
            }
            else//4 same thousands, 1 smallest hundreds
            {
                x = idx[0];
                solution += $"  백의 자리 숫자에서 가장 큰 숫자는 {hundreds[x]} 입니다.\\\\";
                solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
            }
        }


        else
        {
            idx = FindLargestWithIndexes(thousands);
            if (idx.Count == 3) // 3 same thousands
            {
                x = idx[0];
                y = idx[1];
                z = idx[2];
                int[] three_STH_Numbers = { numbers[x], numbers[y], numbers[z] };
                int[] three_STH_thousands = { three_STH_Numbers[0] / 1000, three_STH_Numbers[1] / 1000, three_STH_Numbers[2] / 1000 };
                int[] three_STH_hundreds = { (three_STH_Numbers[0] / 100) % 10, (three_STH_Numbers[1] / 100) % 10, (three_STH_Numbers[2] / 100) % 10 };
                int[] three_STH_tens = { (three_STH_Numbers[0] / 10) % 10, (three_STH_Numbers[1] / 10) % 10, (three_STH_Numbers[2] / 10) % 10 };
                int[] three_STH_ones = { three_STH_Numbers[0] % 10, three_STH_Numbers[1] % 10, three_STH_Numbers[2] % 10 };


                solution += $"  천의 자리 숫자에서 가장 큰 숫자는 {thousands[x]} 이고";
                solution += $" 세 숫자의 천의 자리 숫자가 같아서 백 숫자를 비교합니다.\\\\ ";
                solution += three_STH_thousands[0] + "\\color{red}{" + three_STH_hundreds[0] + "}\\color{black}" + $"{three_STH_tens[0]}" + $"{three_STH_ones[0]}" +
                        ",  " + three_STH_thousands[1] + "\\color{red}{" + three_STH_hundreds[1] + "}\\color{black}" + $"{three_STH_tens[1]}" + $"{three_STH_ones[1]}" +
                        ",  " + three_STH_thousands[2] + "\\color{red}{" + three_STH_hundreds[2] + "}\\color{black}" + $"{three_STH_tens[2]}" + $"{three_STH_ones[2]}";

                idx = FindLargestWithIndexes(three_STH_hundreds); //find smallest hundreds
                if (idx.Count == 3) // 3 same thousands, 3 same hundreds
                {
                    solution += $"  백의 자리 숫자에서 가장 큰 숫자는 {three_STH_hundreds[0]} 이고";
                    solution += $" 세 숫자의 백의 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";

                    solution += $"{three_STH_thousands[0]}" + $"{three_STH_hundreds[0]}" + "\\color{red}{" + three_STH_tens[0] + "}\\color{black}" + $"{three_STH_ones[0]}" +
                        ",  " + $"{three_STH_thousands[1]}" + $"{three_STH_hundreds[1]}" + "\\color{red}{" + three_STH_tens[1] + "}\\color{black}" + $"{three_STH_ones[1]}" +
                        ",  " + $"{three_STH_thousands[2]}" + $"{three_STH_hundreds[2]}" + "\\color{red}{" + three_STH_tens[2] + "}\\color{black}" + $"{three_STH_ones[2]}";

                    idx = FindLargestWithIndexes(three_STH_tens); //find smallest tens
                    if (idx.Count == 3) //3 same thousands, 3 same hundreds, 3 same tens
                    {
                        solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {three_STH_tens[0]} 이고";
                        solution += $" 세 숫자의 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";

                        maxOnes = three_STH_ones.Max();

                        solution += $"{three_STH_thousands[0]}" + $"{three_STH_hundreds[0]}" + $"{three_STH_tens[0]}" + "\\color{red}{" + three_STH_ones[0] + "}\\color{black}" +
                        ",  " + $"{three_STH_thousands[1]}" + $"{three_STH_hundreds[1]}" + $"{three_STH_tens[1]}" + "\\color{red}{" + three_STH_ones[1] + "}\\color{black}" +
                        ",  " + $"{three_STH_thousands[2]}" + $"{three_STH_hundreds[2]}" + $"{three_STH_tens[2]}" + "\\color{red}{" + three_STH_ones[2] + "}\\color{black}";


                        solution += $"  일의 자리 숫자에서 가장 큰 숫자는 {maxOnes} 입니다.\\\\";
                        solution += $"그래서, 가장 큰 숫자는{numbers.Max()}입니다.";
                    }
                    else if (idx.Count == 2)
                    { //3 same thousands, 3 same hundreds, 2 same tens
                        x = idx[0];
                        y = idx[1];
                        solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {three_STH_tens[x]} 이고";
                        solution += $" 두 숫자의 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";

                        maxOnes = new int[] { three_STH_ones[x], three_STH_ones[y] }.Max();
                        solution += $"{three_STH_thousands[x]}" + $"{three_STH_hundreds[x]}" + $"{three_STH_tens[x]}" + "\\color{red}{" + three_STH_ones[x] + "}\\color{black}" +
                        ",  " + $"{three_STH_thousands[y]}" + $"{three_STH_hundreds[y]}" + $"{three_STH_tens[y]}" + "\\color{red}{" + three_STH_ones[y] + "}\\color{black}";

                        solution += $"  일의 자리 숫자에서 가장 큰 숫자는 {maxOnes} 입니다.\\\\";
                        solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
                    }
                    else
                    { //3 same thousands, 3 same hundreds, 1 smallest tens
                        x = idx[0];
                        solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {three_STH_tens[x]} 입니다.\\\\";
                        solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
                    }

                }
                else if (idx.Count == 2)// 3 same thousands, 2 same hundreds
                {
                    x = idx[0];
                    y = idx[1];
                    solution += $"  백의 자리 숫자에서 가장 큰 숫자는 {three_STH_hundreds[x]} 이고";
                    solution += $" 두 숫자의 백의 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";
                    solution += $"{three_STH_thousands[x]}" + $"{three_STH_hundreds[x]}" + "\\color{red}{" + three_STH_tens[x] + "}\\color{black}" + $"{three_STH_ones[x]}" +
                        ",  " + $"{three_STH_thousands[y]}" + $"{three_STH_hundreds[y]}" + "\\color{red}{" + three_STH_tens[y] + "}\\color{black}" + $"{three_STH_ones[y]}";

                    idx = FindLargestWithIndexes(new int[] { three_STH_tens[x], three_STH_tens[y] });
                    if (idx.Count == 2)
                    {//3 same thousands, 2 same hundreds, 2 same tens
                        x = idx[0];
                        y = idx[1];
                        solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {three_STH_tens[x]} 이고";
                        solution += $" 두 숫자의 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";

                        maxOnes = new int[] { three_STH_ones[x], three_STH_ones[y] }.Max();
                        solution += $"{three_STH_thousands[x]}" + $"{three_STH_hundreds[x]}" + $"{three_STH_tens[x]}" + "\\color{red}{" + three_STH_ones[x] + "}\\color{black}" +
                        ",  " + $"{three_STH_thousands[y]}" + $"{three_STH_hundreds[y]}" + $"{three_STH_tens[y]}" + "\\color{red}{" + three_STH_ones[y] + "}\\color{black}";

                        solution += $"  일의 자리 숫자에서 가장 큰 숫자는 {maxOnes} 입니다.\\\\";
                        solution += $"그래서, 가장 큰 숫자는{numbers.Max()}입니다.";
                    }
                    else if (idx.Count == 1)
                    {//3 same thousands, 2 same hundreds, 1 smallest tens
                        x = idx[0];
                        solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {three_STH_tens[x]} 입니다.\\\\";
                        solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
                    }

                }
                else// 3 same thousands, 1 smallest hundreds
                {
                    x = idx[0];
                    solution += $"  백의 자리 숫자에서 가장 큰 숫자는 {three_STH_hundreds[x]} 입니다.\\\\";
                    solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";

                }
            }

            else if (idx.Count == 2) // 2 same thousands
            {
                x = idx[0];
                y = idx[1];
                int[] two_STH_Numbers = { numbers[x], numbers[y] };
                int[] two_STH_thousands = { two_STH_Numbers[0] / 1000, two_STH_Numbers[1] / 1000 };
                int[] two_STH_hundreds = { (two_STH_Numbers[0] / 100) % 10, (two_STH_Numbers[1] / 100) % 10 };
                int[] two_STH_tens = { (two_STH_Numbers[0] / 10) % 10, (two_STH_Numbers[1] / 10) % 10 };
                int[] two_STH_ones = { two_STH_Numbers[0] % 10, two_STH_Numbers[1] % 10 };
                solution += $"  천의 자리 숫자에서 가장 큰 숫자는 {thousands[x]} 이고";
                solution += $" 두 숫자의 천의 자리 숫자가 같아서 백 숫자를 비교합니다.\\\\ ";

                solution += two_STH_thousands[0] + "\\color{red}{" + two_STH_hundreds[0] + "}\\color{black}" + $"{two_STH_tens[0]}" + $"{two_STH_ones[0]}" +
                        ",  " + two_STH_thousands[1] + "\\color{red}{" + two_STH_hundreds[1] + "}\\color{black}" + $"{two_STH_tens[1]}" + $"{two_STH_ones[1]}";

                idx = FindLargestWithIndexes(two_STH_hundreds);//find smallest hundreds
                if (idx.Count == 2) // 2 same thousands, 2 same hundreds
                {
                    solution += $"  백의 자리 숫자에서 가장 큰 숫자는 {two_STH_hundreds[x]} 이고";
                    solution += $" 두 숫자의 백의 자리 숫자가 같아서 십 숫자를 비교합니다.\\\\ ";
                    solution += $"{two_STH_thousands[0]}" + $"{two_STH_hundreds[0]}" + "\\color{red}{" + two_STH_tens[0] + "}\\color{black}" + $"{two_STH_ones[0]}" +
                        ",  " + $"{two_STH_thousands[1]}" + $"{two_STH_hundreds[1]}" + "\\color{red}{" + two_STH_tens[1] + "}\\color{black}" + $"{two_STH_ones[1]}";

                    idx = FindLargestWithIndexes(two_STH_tens); //find smallest tens
                    if (idx.Count == 2) //2 same thousands, 2 same hundreds, 2 same tens
                    {
                        solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {two_STH_tens[0]} 이고";
                        solution += $" 두 숫자의 십의 자리 숫자가 같아서 일 숫자를 비교합니다.\\\\ ";

                        maxOnes = two_STH_ones.Max();
                        solution += $"{two_STH_thousands[0]}" + $"{two_STH_hundreds[0]}" + $"{two_STH_tens[0]}" + "\\color{red}{" + two_STH_ones[0] + "}\\color{black}" +
                        ",  " + $"{two_STH_thousands[1]}" + $"{two_STH_hundreds[1]}" + $"{two_STH_tens[1]}" + "\\color{red}{" + two_STH_ones[1] + "}\\color{black}";

                        solution += $"  일의 자리 숫자에서 가장 큰 숫자는 {maxOnes} 입니다.\\\\";
                        solution += $"그래서, 가장 큰 숫자는{numbers.Max()}입니다.";
                    }
                    else if (idx.Count == 1)
                    {//2 same thousands, 2 same hundreds, 1 smallest tens
                        x = idx[0];
                        solution += $"  십의 자리 숫자에서 가장 큰 숫자는 {two_STH_tens[x]} 입니다.\\\\";
                        solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
                    }
                }
                else if (idx.Count == 1)
                {// 2 same thousands, 1 smallest hundreds
                    solution += $"  백의 자리 숫자에서 가장 큰 숫자는 {two_STH_hundreds.Max()} 입니다.\\\\";
                    solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
                }

            }
            else if (idx.Count == 1) //1 smallest thousands
            {
                x = idx[0];
                solution += $"  천의 자리 숫자에서 가장 큰 숫자는 {thousands[x]} 입니다.\\\\";
                solution += $"그래서, 가장 큰 숫자는 {numbers.Max()}입니다.";
            }
        }

        return solution;
    }
    static string twoDPMin(double[] numbers)
    {
        int[] ones = new int[numbers.Length];
        int[] first_decimal = new int[numbers.Length];
        int[] second_decimal = new int[numbers.Length];

        for (int i = 0; i < numbers.Length; i++)
        {
            string numberStr = numbers[i].ToString("F2");
            string[] parts = numberStr.Split('.');

            ones[i] = int.Parse(parts[0]);
            first_decimal[i] = int.Parse(parts[1][0].ToString());
            second_decimal[i] = int.Parse(parts[1][1].ToString());
        }
        string solution = "";
        if (numbers[0] == numbers[1])
        {
            solution += $"{numbers[0]} {numbers[1]} 두 숫자의 크기가 같습니다.";
            return solution;
        }

        if (ones[0] == ones[1])
        { // 2 same ones
            solution += "일의 자리를 비교하면 ";
            solution += "\\color{red}{" + ones[0] + "}.\\color{black}" + $"{first_decimal[0]}" + $"{second_decimal[0]}" + "는 " + ones[0] + ", ";
            solution += "\\color{red}{" + ones[1] + "}.\\color{black}" + $"{first_decimal[1]}" + $"{second_decimal[1]}" + "도 " + ones[1] + "로 서로 같습니다.\\\\ ";

            if (first_decimal[0] == first_decimal[1])
            { // 2 same ones, 2 same first decimal
                solution += "소수 첫째 자리를 비교하면 ";
                solution += ones[0] + ".\\color{red}{" + first_decimal[0] + "}\\color{black}" + second_decimal[0] + "는 " + first_decimal[0] + ", ";
                solution += ones[1] + ".\\color{red}{" + first_decimal[1] + "}\\color{black}" + second_decimal[1] + "도 " + first_decimal[1] + "로 서로 같습니다.\\\\ ";

                solution += $"{ones[0]}" + "." + $"{first_decimal[0]}" + "\\color{red}{" + second_decimal[0] + "}\\color{black}" + "는 " + second_decimal[0] + ", ";
                solution += $"{ones[1]}" + "." + $"{first_decimal[1]}" + "\\color{red}{" + second_decimal[1] + "}\\color{black}" + "는 " + second_decimal[1] + "로 ";
                if (second_decimal[0] < second_decimal[1])
                {
                    solution += $"{numbers[0]}가 더 작습니다.\\\\";
                    solution += $"따라서 {numbers[0]} < {numbers[1]} ";
                }
                else
                {
                    solution += $"{numbers[1]}가 더 작습니다.\\\\";
                    solution += $"따라서 {numbers[1]} < {numbers[0]} ";
                }
            }
            else
            { // 2 same ones, 1 smallest first decimal
                solution += "소수 첫째 자리를 비교하면 ";
                solution += ones[0] + ".\\color{red}{" + first_decimal[0] + "}\\color{black}" + second_decimal[0] + "는 " + first_decimal[0] + ", ";
                solution += ones[1] + ".\\color{red}{" + first_decimal[1] + "}\\color{black}" + second_decimal[1] + "는 " + first_decimal[1] + "로 ";
                if (first_decimal[0] < first_decimal[1])
                {
                    solution += $"{numbers[0]}가 더 작습니다.\\\\";
                    solution += $"따라서 {numbers[0]} < {numbers[1]} ";
                }
                else
                {
                    solution += $"{numbers[1]}가 더 작습니다.\\\\";
                    solution += $"따라서 {numbers[1]} < {numbers[0]} ";
                }
            }
        }
        else
        { // 1 smallest ones
            solution += "일의 자리를 비교하면 ";
            solution += "\\color{red}{" + ones[0] + "}.\\color{black}" + $"{first_decimal[0]}" + $"{second_decimal[0]}" + "는 " + ones[0] + ", ";
            solution += "\\color{red}{" + ones[1] + "}.\\color{black}" + $"{first_decimal[1]}" + $"{second_decimal[1]}" + "는 " + ones[1] + "로 ";
            if (ones[0] < ones[1])
            {
                solution += $"{numbers[0]}가 더 작습니다.\\\\";
                solution += $"따라서 {numbers[0]} < {numbers[1]} ";
            }
            else
            {
                solution += $"{numbers[1]}가 더 작습니다.\\\\";
                solution += $"따라서 {numbers[1]} < {numbers[0]} ";
            }
        }

        return solution;
    }
    static string threeDPMin(double[] numbers)
    {
        int[] ones = new int[numbers.Length];
        int[] first_decimal = new int[numbers.Length];
        int[] second_decimal = new int[numbers.Length];
        int[] third_decimal = new int[numbers.Length];

        for (int i = 0; i < numbers.Length; i++)
        {
            string numberStr = numbers[i].ToString("F3");  // Convert number to string with 3 decimal places
            string[] parts = numberStr.Split('.');  // Split the number into integer and decimal parts

            ones[i] = int.Parse(parts[0]);
            first_decimal[i] = int.Parse(parts[1][0].ToString());
            second_decimal[i] = int.Parse(parts[1][1].ToString());
            third_decimal[i] = int.Parse(parts[1][2].ToString());
        }

        string solution = "";
        if (numbers[0] == numbers[1])
        {
            solution += $"{numbers[0]} {numbers[1]} 두 숫자의 크기가 같습니다.";
            return solution;
        }

        if (ones[0] == ones[1])
        { // 2 same ones
            solution += "일의 자리를 비교하면 ";
            solution += "\\color{red}{" + ones[0] + "}.\\color{black}" + $"{first_decimal[0]}" + $"{second_decimal[0]}" + $"{third_decimal[0]}" + "는 " + ones[0] + ", ";
            solution += "\\color{red}{" + ones[1] + "}.\\color{black}" + $"{first_decimal[1]}" + $"{second_decimal[1]}" + $"{third_decimal[1]}" + "도 " + ones[1] + "로 서로 같습니다.\\\\ ";

            if (first_decimal[0] == first_decimal[1])
            { // 2 same ones, 2 same first decimal
                solution += "소수 첫째 자리를 비교하면 ";
                solution += ones[0] + ".\\color{red}{" + first_decimal[0] + "}\\color{black}" + $"{second_decimal[0]}" + $"{third_decimal[0]}" + "는 " + first_decimal[0] + ", ";
                solution += ones[1] + ".\\color{red}{" + first_decimal[1] + "}\\color{black}" + $"{second_decimal[1]}" + $"{third_decimal[1]}" + "도 " + first_decimal[1] + "로 서로 같습니다.\\\\ ";

                solution += "소수 둘째 자리를 비교하면 ";
                if (second_decimal[0] < second_decimal[1])
                {
                    solution += $"{ones[0]}" + "." + $"{first_decimal[0]}" + "\\color{red}{" + second_decimal[0] + "}\\color{black}" + third_decimal[0] + "는 " + second_decimal[0] + ", ";
                    solution += $"{ones[1]}" + "." + $"{first_decimal[1]}" + "\\color{red}{" + second_decimal[1] + "}\\color{black}" + third_decimal[1] + "는 " + second_decimal[1] + "로 ";
                    solution += $"{numbers[0]}가 더 작습니다.\\\\";
                    solution += $"따라서 {numbers[0]} < {numbers[1]} ";
                }
                else if (second_decimal[0] > second_decimal[1])
                {
                    solution += $"{ones[0]}" + "." + $"{first_decimal[0]}" + "\\color{red}{" + second_decimal[0] + "}\\color{black}" + third_decimal[0] + "는 " + second_decimal[0] + ", ";
                    solution += $"{ones[1]}" + "." + $"{first_decimal[1]}" + "\\color{red}{" + second_decimal[1] + "}\\color{black}" + third_decimal[1] + "는 " + second_decimal[1] + "로 ";
                    solution += $"{numbers[1]}가 더 작습니다.\\\\";
                    solution += $"따라서 {numbers[1]} < {numbers[0]} ";
                }
                else
                { //2 same ones, 2 same first decimal, 2 same second decimal
                    solution += $"{ones[0]}" + "." + $"{first_decimal[0]}" + "\\color{red}{" + second_decimal[0] + "}\\color{black}" + third_decimal[0] + "는 " + second_decimal[0] + ", ";
                    solution += $"{ones[1]}" + "." + $"{first_decimal[1]}" + "\\color{red}{" + second_decimal[1] + "}\\color{black}" + third_decimal[1] + "도 " + second_decimal[1] + "로 서로 같습니다.\\\\";

                    solution += "소수 셋째 자리를 비교하면 ";
                    if (third_decimal[0] < third_decimal[1])
                    {
                        solution += $"{ones[0]}" + "." + $"{first_decimal[0]}" + $"{second_decimal[0]}" + "\\color{red}{" + third_decimal[0] + "}\\color{black}는 " + third_decimal[0] + ", ";
                        solution += $"{ones[1]}" + "." + $"{first_decimal[1]}" + $"{second_decimal[1]}" + "\\color{red}{" + third_decimal[1] + "}\\color{black}는 " + third_decimal[1] + "로 ";
                        solution += $"{numbers[0]}가 더 작습니다.\\\\";
                        solution += $"따라서 {numbers[0]} < {numbers[1]} ";
                    }
                    else
                    {
                        solution += $"{ones[0]}" + "." + $"{first_decimal[0]}" + $"{second_decimal[0]}" + "\\color{red}{" + third_decimal[0] + "}\\color{black}는 " + third_decimal[0] + ", ";
                        solution += $"{ones[1]}" + "." + $"{first_decimal[1]}" + $"{second_decimal[1]}" + "\\color{red}{" + third_decimal[1] + "}\\color{black}는 " + third_decimal[1] + "로 ";
                        solution += $"{numbers[1]}가 더 작습니다.\\\\";
                        solution += $"따라서 {numbers[1]} < {numbers[0]} ";
                    }
                }

            }
            else
            { // 2 same ones, 1 smallest first decimal
                solution += "소수 첫째 자리를 비교하면 ";
                solution += ones[0] + ".\\color{red}{" + first_decimal[0] + "}\\color{black}" + $"{second_decimal[0]}" + $"{third_decimal[0]}" + "는 " + first_decimal[0] + ", ";
                solution += ones[1] + ".\\color{red}{" + first_decimal[1] + "}\\color{black}" + $"{second_decimal[1]}" + $"{third_decimal[1]}" + "는 " + first_decimal[1] + "로 ";
                if (first_decimal[0] < first_decimal[1])
                {
                    solution += $"{numbers[0]}가 더 작습니다.\\\\";
                    solution += $"따라서 {numbers[0]} < {numbers[1]} ";
                }
                else
                {
                    solution += $"{numbers[1]}가 더 작습니다.\\\\";
                    solution += $"따라서 {numbers[1]} < {numbers[0]} ";
                }
            }
        }
        else
        { // 1 smallest ones
            solution += "일의 자리를 비교하면 ";
            solution += "\\color{red}{" + ones[0] + "}.\\color{black}" + $"{first_decimal[0]}" + $"{second_decimal[0]}" + $"{third_decimal[0]}" + "는 " + ones[0] + ", ";
            solution += "\\color{red}{" + ones[1] + "}.\\color{black}" + $"{first_decimal[1]}" + $"{second_decimal[1]}" + $"{third_decimal[1]}" + "는 " + ones[1] + "로 ";
            if (ones[0] < ones[1])
            {
                solution += $"{numbers[0]}가 더 작습니다.\\\\";
                solution += $"따라서 {numbers[0]} < {numbers[1]} ";
            }
            else
            {
                solution += $"{numbers[1]}가 더 작습니다.\\\\";
                solution += $"따라서 {numbers[1]} < {numbers[0]} ";
            }
        }

        return solution;
    }
    static int GetDecimalPlaces(double number)
    {
        string numberStr = number.ToString();
        if (numberStr.Contains('.'))
        {
            return numberStr.Split('.')[1].Length;
        }
        return 0;
    }
    static string twoDPMax(double[] numbers)
    {
        int[] ones = new int[numbers.Length];
        int[] first_decimal = new int[numbers.Length];
        int[] second_decimal = new int[numbers.Length];

        for (int i = 0; i < numbers.Length; i++)
        {
            string numberStr = numbers[i].ToString("F2");
            string[] parts = numberStr.Split('.');

            ones[i] = int.Parse(parts[0]);
            first_decimal[i] = int.Parse(parts[1][0].ToString());
            second_decimal[i] = int.Parse(parts[1][1].ToString());
        }
        string solution = "";
        if (numbers[0] == numbers[1])
        {
            solution += $"{numbers[0]} {numbers[1]} 두 숫자의 크기가 같습니다.";
            return solution;
        }

        if (ones[0] == ones[1])
        { // 2 same ones
            solution += "일의 자리를 비교하면 ";
            solution += "\\color{red}{" + ones[0] + "}.\\color{black}" + $"{first_decimal[0]}" + $"{second_decimal[0]}" + "는 " + ones[0] + ", ";
            solution += "\\color{red}{" + ones[1] + "}.\\color{black}" + $"{first_decimal[1]}" + $"{second_decimal[1]}" + "도 " + ones[1] + "로 서로 같습니다.\\\\ ";

            if (first_decimal[0] == first_decimal[1])
            { // 2 same ones, 2 same first decimal
                solution += "소수 첫째 자리를 비교하면 ";
                solution += ones[0] + ".\\color{red}{" + first_decimal[0] + "}\\color{black}" + second_decimal[0] + "는 " + first_decimal[0] + ", ";
                solution += ones[1] + ".\\color{red}{" + first_decimal[1] + "}\\color{black}" + second_decimal[1] + "도 " + first_decimal[1] + "로 서로 같습니다.\\\\ ";

                solution += $"{ones[0]}" + "." + $"{first_decimal[0]}" + "\\color{red}{" + second_decimal[0] + "}\\color{black}" + "는 " + second_decimal[0] + ", ";
                solution += $"{ones[1]}" + "." + $"{first_decimal[1]}" + "\\color{red}{" + second_decimal[1] + "}\\color{black}" + "는 " + second_decimal[1] + "로 ";
                if (second_decimal[0] > second_decimal[1])
                {
                    solution += $"{numbers[0]}가 더 큽니다.\\\\";
                    solution += $"따라서 {numbers[0]} > {numbers[1]} ";
                }
                else
                {
                    solution += $"{numbers[1]}가 더 큽니다.\\\\";
                    solution += $"따라서 {numbers[1]} > {numbers[0]} ";
                }
            }
            else
            { // 2 same ones, 1 smallest first decimal
                solution += "소수 첫째 자리를 비교하면 ";
                solution += ones[0] + ".\\color{red}{" + first_decimal[0] + "}\\color{black}" + second_decimal[0] + "는 " + first_decimal[0] + ", ";
                solution += ones[1] + ".\\color{red}{" + first_decimal[1] + "}\\color{black}" + second_decimal[1] + "는 " + first_decimal[1] + "로 ";
                if (first_decimal[0] > first_decimal[1])
                {
                    solution += $"{numbers[0]}가 더 큽니다.\\\\";
                    solution += $"따라서 {numbers[0]} > {numbers[1]} ";
                }
                else
                {
                    solution += $"{numbers[1]}가 더 큽니다.\\\\";
                    solution += $"따라서 {numbers[1]} > {numbers[0]} ";
                }
            }
        }
        else
        { // 1 smallest ones
            solution += "일의 자리를 비교하면 ";
            solution += "\\color{red}{" + ones[0] + "}.\\color{black}" + $"{first_decimal[0]}" + $"{second_decimal[0]}" + "는 " + ones[0] + ", ";
            solution += "\\color{red}{" + ones[1] + "}.\\color{black}" + $"{first_decimal[1]}" + $"{second_decimal[1]}" + "는 " + ones[1] + "로 ";
            if (ones[0] > ones[1])
            {
                solution += $"{numbers[0]}가 더 큽니다.\\\\";
                solution += $"따라서 {numbers[0]} > {numbers[1]} ";
            }
            else
            {
                solution += $"{numbers[1]}가 더 큽니다.\\\\";
                solution += $"따라서 {numbers[1]} > {numbers[0]} ";
            }
        }

        return solution;
    }
    static string threeDPMax(double[] numbers)
    {
        int[] ones = new int[numbers.Length];
        int[] first_decimal = new int[numbers.Length];
        int[] second_decimal = new int[numbers.Length];
        int[] third_decimal = new int[numbers.Length];

        for (int i = 0; i < numbers.Length; i++)
        {
            string numberStr = numbers[i].ToString("F3");  // Convert number to string with 3 decimal places
            string[] parts = numberStr.Split('.');  // Split the number into integer and decimal parts

            ones[i] = int.Parse(parts[0]);
            first_decimal[i] = int.Parse(parts[1][0].ToString());
            second_decimal[i] = int.Parse(parts[1][1].ToString());
            third_decimal[i] = int.Parse(parts[1][2].ToString());
        }

        string solution = "";
        if (numbers[0] == numbers[1])
        {
            solution += $"{numbers[0]} {numbers[1]} 두 숫자의 크기가 같습니다.";
            return solution;
        }

        if (ones[0] == ones[1])
        { // 2 same ones
            solution += "일의 자리를 비교하면 ";
            solution += "\\color{red}{" + ones[0] + "}.\\color{black}" + $"{first_decimal[0]}" + $"{second_decimal[0]}" + $"{third_decimal[0]}" + "는 " + ones[0] + ", ";
            solution += "\\color{red}{" + ones[1] + "}.\\color{black}" + $"{first_decimal[1]}" + $"{second_decimal[1]}" + $"{third_decimal[1]}" + "도 " + ones[1] + "로 서로 같습니다.\\\\ ";

            if (first_decimal[0] == first_decimal[1])
            { // 2 same ones, 2 same first decimal
                solution += "소수 첫째 자리를 비교하면 ";
                solution += ones[0] + ".\\color{red}{" + first_decimal[0] + "}\\color{black}" + $"{second_decimal[0]}" + $"{third_decimal[0]}" + "는 " + first_decimal[0] + ", ";
                solution += ones[1] + ".\\color{red}{" + first_decimal[1] + "}\\color{black}" + $"{second_decimal[1]}" + $"{third_decimal[1]}" + "도 " + first_decimal[1] + "로 서로 같습니다.\\\\ ";

                solution += "소수 둘째 자리를 비교하면 ";
                if (second_decimal[0] > second_decimal[1])
                {
                    solution += $"{ones[0]}" + "." + $"{first_decimal[0]}" + "\\color{red}{" + second_decimal[0] + "}\\color{black}" + third_decimal[0] + "는 " + second_decimal[0] + ", ";
                    solution += $"{ones[1]}" + "." + $"{first_decimal[1]}" + "\\color{red}{" + second_decimal[1] + "}\\color{black}" + third_decimal[1] + "는 " + second_decimal[1] + "로 ";
                    solution += $"{numbers[0]}가 더 큽니다.\\\\";
                    solution += $"따라서 {numbers[0]} > {numbers[1]} ";
                }
                else if (second_decimal[1] > second_decimal[0])
                {
                    solution += $"{ones[0]}" + "." + $"{first_decimal[0]}" + "\\color{red}{" + second_decimal[0] + "}\\color{black}" + third_decimal[0] + "는 " + second_decimal[0] + ", ";
                    solution += $"{ones[1]}" + "." + $"{first_decimal[1]}" + "\\color{red}{" + second_decimal[1] + "}\\color{black}" + third_decimal[1] + "는 " + second_decimal[1] + "로 ";
                    solution += $"{numbers[1]}가 더 큽니다.\\\\";
                    solution += $"따라서 {numbers[1]} > {numbers[0]} ";
                }
                else
                { //2 same ones, 2 same first decimal, 2 same second decimal
                    solution += $"{ones[0]}" + "." + $"{first_decimal[0]}" + "\\color{red}{" + second_decimal[0] + "}\\color{black}" + third_decimal[0] + "는 " + second_decimal[0] + ", ";
                    solution += $"{ones[1]}" + "." + $"{first_decimal[1]}" + "\\color{red}{" + second_decimal[1] + "}\\color{black}" + third_decimal[1] + "도 " + second_decimal[1] + "로 서로 같습니다.\\\\";

                    solution += "소수 셋째 자리를 비교하면 ";
                    if (third_decimal[0] > third_decimal[1])
                    {
                        solution += $"{ones[0]}" + "." + $"{first_decimal[0]}" + $"{second_decimal[0]}" + "\\color{red}{" + third_decimal[0] + "}\\color{black}는 " + third_decimal[0] + ", ";
                        solution += $"{ones[1]}" + "." + $"{first_decimal[1]}" + $"{second_decimal[1]}" + "\\color{red}{" + third_decimal[1] + "}\\color{black}는 " + third_decimal[1] + "로 ";
                        solution += $"{numbers[0]}가 더 큽니다.\\\\";
                        solution += $"따라서 {numbers[0]} > {numbers[1]} ";
                    }
                    else
                    {
                        solution += $"{ones[0]}" + "." + $"{first_decimal[0]}" + $"{second_decimal[0]}" + "\\color{red}{" + third_decimal[0] + "}\\color{black}는 " + third_decimal[0] + ", ";
                        solution += $"{ones[1]}" + "." + $"{first_decimal[1]}" + $"{second_decimal[1]}" + "\\color{red}{" + third_decimal[1] + "}\\color{black}는 " + third_decimal[1] + "로 ";
                        solution += $"{numbers[1]}가 더 큽니다.\\\\";
                        solution += $"따라서 {numbers[1]} > {numbers[0]} ";
                    }
                }

            }
            else
            { // 2 same ones, 1 smallest first decimal
                solution += "소수 첫째 자리를 비교하면 ";
                solution += ones[0] + ".\\color{red}{" + first_decimal[0] + "}\\color{black}" + $"{second_decimal[0]}" + $"{third_decimal[0]}" + "는 " + first_decimal[0] + ", ";
                solution += ones[1] + ".\\color{red}{" + first_decimal[1] + "}\\color{black}" + $"{second_decimal[1]}" + $"{third_decimal[1]}" + "는 " + first_decimal[1] + "로 ";
                if (first_decimal[0] > first_decimal[1])
                {
                    solution += $"{numbers[0]}가 더 큽니다.\\\\";
                    solution += $"따라서 {numbers[0]} > {numbers[1]} ";
                }
                else
                {
                    solution += $"{numbers[1]}가 더 큽니다.\\\\";
                    solution += $"따라서 {numbers[1]} > {numbers[0]} ";
                }
            }
        }
        else
        { // 1 smallest ones
            solution += "일의 자리를 비교하면 ";
            solution += "\\color{red}{" + ones[0] + "}.\\color{black}" + $"{first_decimal[0]}" + $"{second_decimal[0]}" + $"{third_decimal[0]}" + "는 " + ones[0] + ", ";
            solution += "\\color{red}{" + ones[1] + "}.\\color{black}" + $"{first_decimal[1]}" + $"{second_decimal[1]}" + $"{third_decimal[1]}" + "는 " + ones[1] + "로 ";
            if (ones[0] > ones[1])
            {
                solution += $"{numbers[0]}가 더 큽니다.\\\\";
                solution += $"따라서 {numbers[0]} > {numbers[1]} ";
            }
            else
            {
                solution += $"{numbers[1]}가 더 큽니다.\\\\";
                solution += $"따라서 {numbers[1]} > {numbers[0]} ";
            }
        }

        return solution;
    }
    static (int[] numerators, int denominator1) extractNumerator(string input)
    {
        string fracPattern = @"Fraction\((\d+),\s*(\d+)\)";
        MatchCollection matches = Regex.Matches(input, fracPattern);

        int[] numerators = new int[2];
        int denominator = 0;
        int index = 0;

        foreach (Match match in matches)
        {
            if (match.Success && index < 2)
            {
                // The first capture group is the numerator
                numerators[index] = int.Parse(match.Groups[1].Value);
                if (index == 0)
                {
                    denominator = int.Parse(match.Groups[2].Value);
                }
                index++;
            }
        }
        return (numerators, denominator);
    }
    static string fracMin(string equation)
    {
        (int[] numerators, int denominator) = extractNumerator(equation);
        int smallest = Math.Min(numerators[0], numerators[1]);
        string solution = "";
        solution += "\\frac{" + numerators[0] + "}{" + denominator + "} ,  ";
        solution += "\\frac{" + numerators[1] + "}{" + denominator + "} ";

        solution += $"주어진 분수에서 분모는 {denominator}으로 서로 같습니다.";
        solution += $"분자는 {numerators[0]}, {numerators[1]}로 서로 다릅니다.\\\\";
        solution += "이때 가장 작은 분수를 구하기 위해서는 가장 작은 분자를 찾아야 합니다.\\\\";
        solution += $"{numerators[0]}, {numerators[1]}중 가장 작은 수는 {smallest}이므로 \\\\";
        solution += "가장 작은 분수는 \\frac{" + smallest + "}{" + denominator + "} 입니다";

        return solution;
    }
    static string fracMax(string equation)
    {
        (int[] numerators, int denominator) = extractNumerator(equation);
        int largest = Math.Max(numerators[0], numerators[1]);
        string solution = "";
        solution += "\\frac{" + numerators[0] + "}{" + denominator + "} ,  ";
        solution += "\\frac{" + numerators[1] + "}{" + denominator + "} ";

        solution += $"주어진 분수에서 분모는 {denominator}으로 서로 같습니다.";
        solution += $"분자는 {numerators[0]}, {numerators[1]}로 서로 다릅니다.\\\\";
        solution += "이때 가장 큰 분수를 구하기 위해서는 가장 큰 분자를 찾아야 합니다.\\\\";
        solution += $"{numerators[0]}, {numerators[1]}중 가장 큰 수는 {largest}이므로 \\\\";
        solution += "가장 큰 분수는 \\frac{" + largest + "}{" + denominator + "} 입니다";

        return solution;
    }
    public static string TexChange(string equation, string answer)
    {
        string result = "";
        string solution = "\\extrabold ";

        if (equation.StartsWith("Fraction"))
        {
            solution += fractionChange(equation, answer);
        }
        else if (equation.StartsWith("divmod"))
        {
            solution += divmodChange(equation, answer);
        }
        else if (equation.StartsWith("gcd"))
        {
            solution += GCDVisualization(equation, answer);
        }
        else if (equation.StartsWith("lcm"))
        {
            solution += LCMVisualization(equation, answer);
        }
        else if (equation.StartsWith("statistics"))
        {
            solution += MeanEquation(equation, answer);
        }
        else if (equation.StartsWith("Min"))
        {
            if (equation.Contains("Fraction")) //min fraction
            {
                solution += fracMin(equation);
            }
            else if (equation.Contains(".")) //min decimal
            {
                string eq = equation.Replace("Min(", "").Replace(")", "");
                double[] numbers = eq.Split(',').Select(double.Parse).ToArray();

                int decimalPlaces = GetDecimalPlaces(numbers[0]);
                if (decimalPlaces == 2)
                {
                    solution += twoDPMin(numbers);
                }
                else if (decimalPlaces == 3)
                {
                    solution += threeDPMin(numbers);
                }

            }
            else //min integer
            {
                string eq = equation.Replace("Min(", "").Replace(")", "");
                int[] numbers = eq.Split(',').Select(int.Parse).ToArray();

                switch (numbers.Length)
                {
                    case 2:
                        solution += twoNumberMin(numbers);
                        break;
                    case 3:
                        solution += threeNumbersMin(numbers);
                        break;
                    case 4:
                        solution += fourNumbersMin(numbers);
                        break;
                    default:
                        solution += "잘못된 입력입니다.";
                        break;
                }

            }
        }
        else if (equation.StartsWith("Max"))
        {
            if (equation.Contains("Fraction")) //max fraction
            {
                solution += fracMax(equation);
            }
            else if (equation.Contains(".")) //max decimal
            {
                string eq = equation.Replace("Max(", "").Replace(")", "");
                double[] numbers = eq.Split(',').Select(double.Parse).ToArray();

                int decimalPlaces = GetDecimalPlaces(numbers[0]);
                if (decimalPlaces == 2)
                {
                    solution += twoDPMax(numbers);
                }
                else if (decimalPlaces == 3)
                {
                    solution += threeDPMax(numbers);
                }

            }
            else //max integer
            {
                string eq = equation.Replace("Max(", "").Replace(")", "");
                int[] numbers = eq.Split(',').Select(int.Parse).ToArray();

                switch (numbers.Length)
                {
                    case 2:
                        solution += twoNumberMax(numbers);
                        break;
                    case 3:
                        solution += threeNumbersMax(numbers);
                        break;
                    case 4:
                        solution += fourNumbersMax(numbers);
                        break;
                    default:
                        solution = "잘못된 입력입니다.";
                        break;
                }
            }
        }
        else
        {
            solution += equation + " = " + answer;
        }

        foreach (char c in solution) // '*' or '/' 
        {
            if (c == '*') { result += "\\times"; }
            else if (c == '/') { result += "\\div"; }
            else { result += c; }
        }

        return result;
    }

}
