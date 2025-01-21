using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using TMPro;
using System.Linq;

public class ListJson : MonoBehaviour
{
    public GameObject buttonPrefab;
    public RectTransform content;
    private string[] filePaths;
    public TextMeshProUGUI score;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI time_elap;
    public TextMeshProUGUI steps;
    public GameObject recordPanel;
    private bool delete;
    private List<GameObject> stepObjects = new List<GameObject>();
    private int playerScore;

    void Start()//get called on scene start
    {
        Refresh();
        delete = false;
    }

    public void Refresh()
    {
        // Clear existing buttons
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }

        // Get files
        string directoryPath = Application.persistentDataPath + "/json tests";
        if (Directory.Exists(directoryPath))
        {
            filePaths = Directory.GetFiles(directoryPath, "*.json");

            // Create buttons for each file
            Vector2 buttonPosition = new Vector2(0, -10);
            foreach (string file in filePaths)
            {
                string jsonString = File.ReadAllText(file);
                TestData data = JsonConvert.DeserializeObject<TestData>(jsonString);

                GameObject button = Instantiate(buttonPrefab, content);
                button.GetComponent<RectTransform>().anchoredPosition = buttonPosition;
                buttonPosition.y -= 50;

                CalculatePlayerScore(data.Steps, int.Parse(data.TimeElapsed));

                string fileName = Path.GetFileNameWithoutExtension(file);
                string recordName = fileName.Substring(0, fileName.IndexOf("-"));
                string show = "Scene: " + recordName + "\nDate " + data.Date + "\nScore " + playerScore.ToString() + '%';
                button.GetComponentInChildren<TextMeshProUGUI>().text = show;
                button.GetComponent<Button>().onClick.AddListener(() => LoadRecord(data));
            }

            // Set size of Content object to fit all buttons
            content.sizeDelta = new Vector2(0, -buttonPosition.y);
        }
        else
        {
            Debug.LogError("Cannot find directory: " + directoryPath);
        }
    }

    private void CalculatePlayerScore(List<StepData> steps, int timeElapsed) //used to calucate score shown in the view scores menu
    {//uses both the time elapsed-function and the correct steps taken to calculate percentage
        int timeThreshold = 20;
        int stepScore = 0;
        int timeScore = 100;
        int incorrectSteps = steps.Where(step => step.Correct == false).Count();
        int totalSteps = steps.Count();

        if (incorrectSteps < totalSteps)
        {
            stepScore = 100 - ((incorrectSteps * 100) / totalSteps);
        }

        if (timeElapsed > 20)
        {
            timeScore = (10 - (timeElapsed-timeThreshold) * 10);
            if (timeScore < 0)
            {
                timeScore = 0;
            }
        }
        playerScore = (stepScore + timeScore) / 2;
    }

    private void LoadRecord(TestData playerData)
    {
        recordPanel.SetActive(true);
        Transform loc = recordPanel.transform;

        CalculatePlayerScore(playerData.Steps, int.Parse(playerData.TimeElapsed));

        dateText.text = playerData.Date.ToString();
        score.text = "Score: "+ playerScore.ToString() + '%';
        time_elap.text = "Time: " + playerData.TimeElapsed.ToString();
        float y = 0f;

        foreach (GameObject stepObject in stepObjects)
        {
            Destroy(stepObject);
        }

        // Clear the list of step objects
        stepObjects.Clear();

        {
            foreach (StepData step in playerData.Steps)
            {
                GameObject stepObj = new GameObject("Step Description"); //generate the steps 
                TextMeshProUGUI descriptionText = stepObj.AddComponent<TextMeshProUGUI>();
                descriptionText.rectTransform.SetParent(loc);
                descriptionText.text = step.Description;
                descriptionText.fontSize = 15;
                
                
                    if (step.Correct == false)
                        descriptionText.color = Color.red; // set the color to red
                    else
                        descriptionText.color = Color.green; // set the color to green
                    descriptionText.enableWordWrapping = false;
                    descriptionText.transform.localPosition = new Vector3(0f, -0.10f, 0f);
                    descriptionText.rectTransform.localScale = new Vector3(0.0114152655f, 0.0114152655f, 0.0114152655f);
                    descriptionText.rectTransform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                    descriptionText.rectTransform.anchoredPosition = new Vector2(0f, y); // set position
                    y -= 0.40f; // decrement y by 1 pixel for next record

                    stepObjects.Add(stepObj);
            }
        }
    }
}


public class TestData //json property to achieve viewing of items
{
    [JsonProperty("correct")]
    public int Correct { get; set; }

    [JsonProperty("incorrect")]
    public int Incorrect { get; set; }

    [JsonProperty("timeElapsed")]
    public string TimeElapsed { get; set; }

    [JsonProperty("testingComplete")]
    public bool TestingComplete { get; set; }

    [JsonProperty("dateStamp")]
    public DateTime Date { get; set; }

    [JsonProperty("steps")]
    public List<StepData> Steps { get; set; }
}

public class StepData //used for the step part in log viewer.
{
    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("correct")]
    public bool Correct { get; set; }
}