using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class CprSceneManager : MonoBehaviour
{
    public List<Canvas> prompts;
    public List<GameObject> objectiveObjects;
    public List<GameObject> modePanelElements;
    public TextMeshProUGUI countdownText;
    public Camera mainCamera;

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        CprEventHandler.onTestingModeSelected += FlagTrainingResumed;
    }

    private void FlagTrainingResumed()
    {
        CprEventHandler.trainingResumed = false;
    }

    private void OnDestroy()
    {
        CprEventHandler.onTestingModeSelected -= FlagTrainingResumed;
    }

    public class ScenarioStep
    {
        public int step { get; set; }
        public bool completed { get; set; }
        public bool correct { get; set; }
        public string stepName { get; set; }
    }

    public class CprSaveData
    {
        public int lastFunctionFiredIndex = 0;
        public bool trainingCompleted = false;
        //public string trainingLastAttempt { get; set; }
        public bool testingCompleted = false;
        //public string testingLastAttempt { get; set; }
    }

    // Helper method for Inspector to quickly find all step prompts
    // Can be activated in Inspector by right clicking on
    // CPR Scene Manager script in ScriptPlaceholder object
    [ContextMenu("Autofill Prompts")]
    void AutofillPrompts()
    {
        prompts = FindObjectsOfType<Canvas>()
            .Where(t => t.name.ToLower().Contains("prompt")).OrderBy(t => t.name)
            .ToList();
    }
}