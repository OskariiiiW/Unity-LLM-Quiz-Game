using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnswerPrefab : MonoBehaviour
{
	public TextMeshProUGUI answer, answerLetter;
	public GameObject answerPrefab, answerState;

	public void SpawnPrefab(string _answer, int order)
	{
		GameObject currentPrefab = Instantiate(answerPrefab, GameLogic.Instance.answerParent);

		currentPrefab.GetComponent<AnswerPrefab>().answer.text = _answer;

		if (order == 0)
		{
			currentPrefab.GetComponent<AnswerPrefab>().answerLetter.text = "A)";
		}
		else if (order == 1)
		{
			currentPrefab.GetComponent<AnswerPrefab>().answerLetter.text = "B)";
		}
		else if (order == 2)
		{
			currentPrefab.GetComponent<AnswerPrefab>().answerLetter.text = "C)";
		}
		else if (order == 3)
		{
			currentPrefab.GetComponent<AnswerPrefab>().answerLetter.text = "D)";
		}
		else
		{
			Debug.Log("error with ordering");
		}

		GameLogic.Instance.answerPrefabs.Add(currentPrefab);
	}

	public void ChooseAnswer()
	{
		GameLogic.Instance.CheckChoise(answer.text);
	}
}
