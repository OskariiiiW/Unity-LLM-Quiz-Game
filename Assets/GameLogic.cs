using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Python.Runtime;
using UnityEngine.UI;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

public class GameLogic : MonoBehaviour
{
	public List<string> answers;
	public TextMeshProUGUI question;
	public List<GameObject> answerPrefabs;
	public GameObject answerPrefab;
	public Transform answerParent;
	public List<Sprite> answerStateSprites;
	public int difficulty;
	public string theme;
	public int correctCounter;
	public TextMeshProUGUI correctUI;
	public GameObject questionUI, gameOverScreen, winScreen;

	public TextAsset jsonFileTEST;
	public string incJson;
	public Image incData;

	private string correctAnswer;
	private int round;

	public static GameLogic Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	public void Start()
	{
		Runtime.PythonDLL = @"C:\Users\Ninja\AppData\Local\Programs\Python\Python310\Python310.dll";
		PythonEngine.Initialize();
	}

	public void SelectDifficulty(int _difficulty)
	{
		difficulty = _difficulty;
		correctUI.text = "0";

	}
	public void SelectTheme(string _theme)
	{
		theme = _theme;
		StartGame();
	}

	public void StartGame()
	{
		questionUI.SetActive(true);
		round = 0;
		correctCounter = 0;
		Debug.Log("Started game with difficulty: " + difficulty + ", theme: " + theme);
		NextRound();
	}

	public void NextRound()
	{
		if (answerPrefabs.Count > 0)
		{
			foreach (GameObject obj in answerPrefabs)
			{
				Destroy(obj);
			}
		}

		var answerCaller = answerPrefab.GetComponent<AnswerPrefab>();

		Debug.Log("API request with: " + difficulty + ", " + theme);
		FetchDataFromAI(difficulty, theme);

		for (int i = 0; i < answers.Count; i++) 
		{
			answerCaller.SpawnPrefab(answers[i], i);
		}
	}
	public void FetchDataFromAI(int _diff, string _theme)					// text generation
	{
		using (Py.GIL())
		{
			dynamic pythonModule = Py.Import("gptapi18");
			//var output = pythonModule.DoAPIRequest(_diff, _theme);
			//var output = pythonModule.TestFunc(_diff, _theme);
			//incJson = output;
		}

		//APIData apiData = JsonUtility.FromJson<APIData>(incJson);

		/*
		List<string> answerList = new List<string>
		{
			apiData.data[2].question,
			apiData.data[3].answers.answer1, // correct
			apiData.data[3].answers.answer2,
			apiData.data[3].answers.answer3,
			apiData.data[3].answers.answer4
		};*/

		List<string> testanswerList = new List<string>
		{
			"What is the fly speed of an unladen swallow?",
			"African or Europian?", // correct
			"2kw",
			"Apple",
			"9m/s"
		};

		question.text = testanswerList[0];
		correctAnswer = testanswerList[1];

		List<string> shuffledList = ShuffleAnswers(testanswerList);
		answers = shuffledList;
	}

	public async void GenerateImage()												// image generation
	{
		//string prompt = "spongebob driving a racing car with shades on";
		string testt = "";

		using (Py.GIL())
		{
			dynamic pythonModule = Py.Import("gptapi18");
			//var output = pythonModule.GenerateImage(prompt);
			//incData.sprite = output;
			//testt = output;
		}
		/*
		using (HttpClient client = new HttpClient())
		{
			try
			{
				using (HttpResponseMessage response = await client.GetAsync(testt))
				{
					if (response.IsSuccessStatusCode)
					{
						using (Stream imageStream = await response.Content.ReadAsStreamAsync())
						{
							// Save the image to a file
							using (FileStream fileStream = File.Create("downloaded_image.jpg"))
							{
								await imageStream.CopyToAsync(fileStream);
							}
							Debug.Log("Image downloaded successfully.");
						}
					}
					else
					{
						Debug.Log($"Failed to download image. Status code: {response.StatusCode}");
					}
				}
			}
			catch (System.Exception ex)
			{
				Debug.Log($"Error: {ex.Message}");
			}
		}*/

		byte[] imageData = await DownloadImage(testt);

		// Convert the downloaded image into a Sprite
		Sprite newSprite = CreateSprite(imageData);

		// Assign the new Sprite
		if (incData != null)
		{
			incData.sprite = newSprite;
		}
		else
		{
			Debug.LogError("SpriteRenderer or Image UI reference is missing!");
		}
	}

	private async Task<byte[]> DownloadImage(string url)
	{
		using (HttpClient client = new HttpClient())
		{
			try
			{
				HttpResponseMessage response = await client.GetAsync(url);
				if (response.IsSuccessStatusCode)
				{
					return await response.Content.ReadAsByteArrayAsync();
				}
				else
				{
					Debug.LogError($"Failed to download image. Status code: {response.StatusCode}");
					return null;
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Error downloading image: {ex.Message}");
				return null;
			}
		}
	}

	private Sprite CreateSprite(byte[] imageData)
	{
		if (imageData == null)
		{
			return null;
		}

		Texture2D texture = new Texture2D(1, 1);
		texture.LoadImage(imageData);

		return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
	}

	public List<string> ShuffleAnswers(List<string> _answers)
	{
		string tempAnswer;
		List<string> shuffledList = new();

		while (shuffledList.Count < 4)
		{
			int randomInt = Random.Range(1, _answers.Count);
			tempAnswer = _answers[randomInt];

			if (!shuffledList.Contains(tempAnswer))
			{
				shuffledList.Add(tempAnswer);
			}
		}

		return shuffledList;
	}

	public void CheckChoise(string choise)
	{
		if (correctAnswer == choise)
		{
			correctCounter++;
			correctUI.text = correctCounter.ToString();

			if (round < 9)
			{
				if (difficulty < 10)
				{
					difficulty++;
				}
				round++;
				NextRound();
			}
			else
			{
				EndGame(false);
			}
		}
		else
		{
			EndGame(true);
		}
	}

	public void EndGame(bool playerLost)
	{
		questionUI.SetActive(false);

		if (playerLost)
		{
			gameOverScreen.SetActive(true);
		}
		else
		{
			winScreen.SetActive(true);
		}
	}
}

[System.Serializable]
public class APIData
{
	public APIDataObject[] data;
}

[System.Serializable]
public class APIDataObject
{
	public string difficulty;
	public string theme;
	public string question;
	public ObjectAnswers answers;
}

[System.Serializable]
public class ObjectAnswers
{
	public string answer1;
	public string answer2;
	public string answer3;
	public string answer4;
}