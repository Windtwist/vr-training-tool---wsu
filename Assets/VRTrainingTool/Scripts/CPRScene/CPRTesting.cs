using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using static CprSceneManager;

public class CprTesting : MonoBehaviour
{
    private List<GameObject> objectiveObjects;
    private List<GameObject> modePanelElements;
    private TextMeshProUGUI countdownText;
    private Camera mainCamera;

    private GameObject menuPanel;
    private GameObject menuText;
    private GameObject menuTrainingBtn;
    private GameObject menuTestingBtn;
    private GameObject menuMainMenuBtn;

    private GameObject patientModel;
    private GameObject phone;
    private GameObject phoneReceiver;
    private GameObject cprBtnSetup;
    private GameObject cprProgressBars;
    private GameObject cprSetup;
    private GameObject cprNpc;
    private CprSaveData saveData;
    private Coroutine timerCoroutine;

    private List<ScenarioStep> steps;
    private string scenarioName = "CPR";
    private int currentStep = 0;
    private int correctStepsCount = 0;
    private int incorrectStepsCount = 0;
    private bool scenarioTimeout = false;
    private int timeElapsed = 0;
    private float countdownTimer = 30.0f;
    private string cprSaveFilePath;



    void Start()
    {
        CprSceneManager sceneMgr = (CprSceneManager)FindObjectOfType(typeof(CprSceneManager));

        objectiveObjects = sceneMgr.objectiveObjects;
        mainCamera = sceneMgr.mainCamera;
        countdownText = sceneMgr.countdownText;

        menuPanel = sceneMgr.modePanelElements[0];
        menuText = sceneMgr.modePanelElements[1];
        menuTrainingBtn = sceneMgr.modePanelElements[1];
        menuTestingBtn = sceneMgr.modePanelElements[3];
        menuMainMenuBtn = sceneMgr.modePanelElements[4];

        patientModel = sceneMgr.objectiveObjects[0];
        phone = sceneMgr.objectiveObjects[1];
        phoneReceiver = sceneMgr.objectiveObjects[2];
        cprBtnSetup = sceneMgr.objectiveObjects[3];
        cprProgressBars = sceneMgr.objectiveObjects[4];
        cprSetup = sceneMgr.objectiveObjects[5];
        cprNpc = sceneMgr.objectiveObjects[6];

        saveData = new CprSaveData();
        cprSaveFilePath = Path.Combine(Application.persistentDataPath, "json saves/CPR.json");

        // Generate scenario steps for further tracking of correct and incorrect actions
        List<string> stepNames = new List<string>();
        stepNames.Add("Called 911");
        stepNames.Add("Placed patient on hard surface");
        stepNames.Add("Placed hands on chest");
        stepNames.Add("Started CPR procedure");

        steps = new List<ScenarioStep>();
        for (int i = 1; i <= 4; i++)
        {
            ScenarioStep tempStep = new ScenarioStep();
            tempStep.step = i;
            tempStep.completed = false;
            tempStep.correct = true;
            tempStep.stepName = stepNames[i-1];
            steps.Add(tempStep);
        }

        //CprEventHandler.trainingCompleted = true;

        // If user has not completed training mode yet, testing should be locked
        if (!CprEventHandler.trainingCompleted)
        {
            //testing menu element
            menuTestingBtn.GetComponent<Image>().color = Color.gray;
            menuTestingBtn.GetComponent<XRSimpleInteractable>().hoverEntered.AddListener(EnableTestingMenuHint);
            menuTestingBtn.GetComponent<XRSimpleInteractable>().hoverExited.AddListener(DisableTestingMenuHint);

            //CprEventHandler.onTestingModeSelected += BeginTesting;
        }
        else
        {
            // disable CPR button and progress bars until interacted with body
            cprBtnSetup.SetActive(false);
            cprProgressBars.SetActive(false);

            //disable testing menu hint
            menuTestingBtn.GetComponentsInChildren<TextMeshProUGUI>()[1].enabled = false;
            menuTestingBtn.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerTestingMode);
            CprEventHandler.onProgressSave += SaveData;
        }
    }

    private void TriggerTestingMode(SelectEnterEventArgs args)
    {
        BeginTesting();
    }

    public void BeginTesting()
    {
        cprNpc.SetActive(true);
        EnableGameObjects();
        menuPanel.SetActive(false); // Menu Panel
        //patientModel.SetActive(true);
        //phone.SetActive(true);
        //phoneReceiver.SetActive(true);

        CprEventHandler.onPhoneSelected += PhoneSelected;
        CprEventHandler.onPatientMoved += PatientMoved;
        CprEventHandler.onHandsPlaced += HandsPlaced;
        CprEventHandler.onCPRStarted += CPRStarted;
        CprEventHandler.onTestingCompleted += TestingCompleted;
        CprEventHandler.onTestingCountdown += BeginTimerCountdown;
        CprEventHandler.onTestingCompleted += StopTimerCountdown;

        currentStep = 1;

        CprEventHandler.TriggerTestingMode();
    }

    private void PhoneSelected()
    {
        ValidateStep(1);
    }

    private void PatientMoved()
    {
        ValidateStep(2);

        // enable CPR button and progress bars
        if (steps[1].completed)
        {
            cprSetup.transform.localPosition = new Vector3(1.36f, -1.26f, cprSetup.transform.localPosition.z);
            patientModel.transform.localPosition = new Vector3(0.0f, 1.45f, patientModel.transform.localPosition.z);
        }
    }

    private void HandsPlaced()
    {
        ValidateStep(3);

        if (steps[2].completed)
        {
            cprBtnSetup.SetActive(true);
            cprProgressBars.SetActive(true);
            CprEventHandler.onHandsPlaced -= HandsPlaced;
        }
    }

    private void CPRStarted()
    {
        ValidateStep(4);
        if (steps[3].completed)
        {
            CprEventHandler.onCPRStarted -= CPRStarted;
        }
    }

    private IEnumerator ScenarioCountdown()
    {
        string textColor = "";

        while (true)
        {
            countdownTimer--;

            if (countdownTimer > 0)
            {
                if (countdownTimer >= 15)
                {
                    textColor = "green";
                }
                else if (countdownTimer <= 15 && countdownTimer >= 5)
                {
                    textColor = "yellow";
                }
                else
                {
                    textColor = "red";
                }

                if (countdownTimer <= 30)
                {
                    countdownText.text = String.Format("Timer: <color=\"{0}\">{1} </color>", textColor, countdownTimer.ToString());
                }
            }
            if (countdownTimer <= 0)
            {
                countdownText.text = "";
                InitiateScenarioTimeout();
            }
            yield return new WaitForSeconds(1);
        }
    }

    private void BeginTimerCountdown()
    {
        timerCoroutine = StartCoroutine(ScenarioCountdown());
        // InvokeRepeating(nameof(ScenarioCountdown), 1.0f, 1.0f);
    }

    private void StopTimerCountdown()
    {
        StopCoroutine(timerCoroutine);
    }

    private void InitiateScenarioTimeout()
    {
        scenarioTimeout = true;
        CprEventHandler.TriggerTestingCompletion();
    }

    private void TestingCompleted()
    {
        CprEventHandler.testingCompleted = true;
        // disable CPR setup
        cprBtnSetup.SetActive(false);
        cprProgressBars.SetActive(false);

        menuPanel.SetActive(true);
        ShowCompletionDialog();

        menuPanel.transform.position = new Vector3(mainCamera.transform.position.x / 2, transform.position.y + 0.3f, mainCamera.transform.position.z);
        CprEventHandler.TriggerSaving();

        UnsubscribeFromObjectives();

        menuTestingBtn.GetComponent<XRSimpleInteractable>().selectEntered.RemoveAllListeners();
        menuTestingBtn.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerSceneReload);

        TestResultsController resultCtrl = new TestResultsController();
        if (!scenarioTimeout)
        {
            timeElapsed = 30 - (int)countdownTimer;
        }
        else
        {
            timeElapsed = 30;
            foreach (ScenarioStep step in steps)
            {
                step.correct = false;
            }
        }
        // Save results for displaying on Score log in Main Menu
        resultCtrl.SaveTestResults(scenarioName, incorrectStepsCount, correctStepsCount, timeElapsed, steps);
    }

    private void ShowCompletionDialog()
    {
        menuMainMenuBtn.SetActive(!menuMainMenuBtn.activeSelf);

        TextMeshProUGUI[] info = menuText.GetComponentsInChildren<TextMeshProUGUI>();
        if (incorrectStepsCount == 0)
        {
            info[0].text = "Congratulations!";
            info[1].text = "You have completed CPR Testing mode without any mistakes";
        }
        if (incorrectStepsCount > 0)
        {
            info[0].text = "Ooops!";
            info[1].text = $"You have completed CPR Testing mode with {incorrectStepsCount} mistakes. You can retry again or repeat Training mode to refresh your knowledge";
        }
        if (scenarioTimeout)
        {
            info[0].text = "Ooops!";
            info[1].text = "You ran out of time. You can reset your progress and attempt training again to refresh your knowledge";
        }
    }

    // Reload scene to have all objects reset to their initial state
    //instead of tracking state of each of them throghout scene
    private void TriggerSceneReload(SelectEnterEventArgs args)
    {
        UnsubscribeFromObjectives();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void EnableGameObjects()
    {
        for (int i = 0; i < objectiveObjects.Count; i++)
        {
            // disable CPR pressure button and progress bars
            if (i != 3 && i != 4)
            {
                objectiveObjects[i].SetActive(true);
            }
        }
    }

    private void DisableGameObjects()
    {
        foreach (var obj in objectiveObjects)
        {
            obj.SetActive(false);
        }
    }

    // Hit is displayed on testing button on hover stating that user needs to complete training first
    private void EnableTestingMenuHint(HoverEnterEventArgs args)
    {
        menuTestingBtn.GetComponentsInChildren<TextMeshProUGUI>()[1].enabled = true;
    }

    private void DisableTestingMenuHint(HoverExitEventArgs args)
    {
        menuTestingBtn.GetComponentsInChildren<TextMeshProUGUI>()[1].enabled = false;
    }

    // Main logic control method that validates 
    // each step to confirm that it was completed in order
    private void ValidateStep(int curStep)
    {
        int curStepIndex = curStep - 1;

        if (curStep == currentStep && !steps[curStepIndex].completed)
        {
            steps[curStepIndex] = UpdateStep(curStep, curStepIndex, true, true);
            correctStepsCount++;
        }
        else
        {
            incorrectStepsCount++;
            steps[curStepIndex] = UpdateStep(curStep, curStepIndex, false, false);
        }
    }

    private ScenarioStep UpdateStep(int step, int stepIndex, bool completed, bool correct)
    {
        ScenarioStep tempStep = new ScenarioStep();
        // only move to next step if current one was correct
        if (!CprEventHandler.trainingResumed && correct)
        {
            currentStep = step + 1;
        }
        tempStep.step = step;
        tempStep.completed = completed;
        // keep step marked incorrect throughout testing if done in wrong order
        if (steps[stepIndex].correct)
        {
            tempStep.correct = correct;
        }

        tempStep.stepName = steps[stepIndex].stepName;

        return tempStep;
    }

    private void SaveData()
    {
        if (!CprEventHandler.trainingResumed)
        {
            CprSaveData saveData = new CprSaveData();
            saveData.lastFunctionFiredIndex = -1;
            saveData.trainingCompleted = CprEventHandler.trainingCompleted;
            saveData.testingCompleted = CprEventHandler.testingCompleted;

            File.WriteAllText(cprSaveFilePath, JsonUtility.ToJson(saveData));
        }
    }

    private void UnsubscribeFromObjectives()
    {
        CprEventHandler.onPhoneSelected -= PhoneSelected;
        CprEventHandler.onPatientMoved -= PatientMoved;
        CprEventHandler.onHandsPlaced -= HandsPlaced;
        CprEventHandler.onCPRStarted -= CPRStarted;
        CprEventHandler.onTestingCompleted -= TestingCompleted;
        CprEventHandler.onTestingCountdown -= BeginTimerCountdown;
        CprEventHandler.onTestingCompleted -= StopTimerCountdown;
    }

    private void OnDestroy()
    {
        UnsubscribeFromObjectives();
    }
}