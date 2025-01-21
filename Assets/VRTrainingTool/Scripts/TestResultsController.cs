using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using static CprSceneManager;

public class TestResultsController //: MonoBehaviour
{
    // public string scenarioName;
    // public int incorrectStepsCount;
    // public int correctStepsCount;
    // public int timeElapsed;
    // public List<ScenarioStep> steps;
    // private string testResultsFilePath;
    // private string testResultsDir;


    // Generates json object that contains all relevant information to show in Score log
    // and saves it to file
    public void SaveTestResults(string scenarioName, int incorrectStepsCount, int correctStepsCount, int timeElapsed, List<ScenarioStep> steps)
    {
        string testResultsDir = Path.Combine(Application.persistentDataPath, "json tests");
        string testFileName = scenarioName + "-test-" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
        string testResultsFilePath = Path.Combine(testResultsDir, testFileName);

        Directory.CreateDirectory(testResultsDir);

        if (!File.Exists(testResultsFilePath))
        {
            TestResults testResults = new TestResults();
            testResults.correct = correctStepsCount;
            testResults.incorrect = incorrectStepsCount;
            testResults.timeElapsed = timeElapsed;
            testResults.testingComplete = true;
            testResults.dateStamp = DateTime.Now.ToString("R");
            testResults.steps = new List<TestResultsStep>();

            for (int i = 0; i < steps.Count; i++)
            {
                TestResultsStep step = new TestResultsStep();
                step.description = steps[i].stepName;
                step.correct = steps[i].correct;
                testResults.steps.Add(step);
            }

            string json = JsonConvert.SerializeObject(testResults, Formatting.Indented);
            // Debug.Log(json);

            using (var sw = new StreamWriter(testResultsFilePath, true))
            {
                sw.Write(json);
                Debug.Log("file written");
            }
        }
    }

    public class TestResults
    {
        public int correct { get; set; }
        public int incorrect { get; set; }
        public int timeElapsed { get; set; }
        public bool testingComplete { get; set; }
        public string dateStamp { get; set; }
        public List<TestResultsStep> steps { get; set; }
    }

    public class TestResultsStep
    {
        public string description { get; set; }
        public bool correct { get; set; }
    }
}
