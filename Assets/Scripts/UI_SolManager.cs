using System.Collections;
using System.Collections.Generic;
using TexDrawLib;
using UnityEngine;
using UnityEngine.UI;

public class SolManager : MonoBehaviour
{
	[SerializeField] Text question;
	[SerializeField] TEXDraw solution;
	[SerializeField] Button leftButton;
	[SerializeField] Button rightButton;
	[SerializeField] Canvas canvas;

	private void Start()
	{
		if (canvas.tag == "Quiz1")
		{
			SolutionManager.Instance.GetQuiz(1, (quest, sol) =>
			{
				if (quest != null && sol != null)
				{
					question.text = quest;
					solution.text = sol;
				}
				else
				{
					Debug.LogError("Failed to retrieve quiz data.");
				}
			});
		}

		else if (canvas.tag == "Quiz2")
		{
			SolutionManager.Instance.GetQuiz(2, (quest, sol) =>
			{
				if (quest != null && sol != null)
				{
					question.text = quest;
					solution.text = sol;
				}
				else
				{
					Debug.LogError("Failed to retrieve quiz data.");
				}
			});
		}

		else if (canvas.tag == "Quiz3")
		{
			SolutionManager.Instance.GetQuiz(3, (quest, sol) =>
			{
				if (quest != null && sol != null)
				{
					question.text = quest;
					solution.text = sol;
				}
				else
				{
					Debug.LogError("Failed to retrieve quiz data.");
				}
			});
		}
	}
}
