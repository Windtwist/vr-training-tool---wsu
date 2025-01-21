using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using static CprSceneManager;

public class CprTraining : MonoBehaviour
{

    private GameObject menuPanel;
    private GameObject menuText;
    private GameObject menuTrainingBtn;
    private GameObject menuTestingBtn;
    private GameObject menuModeSelectionBtn;
    private GameObject menuMainMenuBtn;
    private GameObject patientModel;
    private GameObject phone;
    private GameObject phoneReceiver;
    private GameObject cprBtnSetup;
    private GameObject cprProgressBars;
    private GameObject cprSetup;
    private GameObject cprNpc;
    private CprSaveData saveData;
    public List<ScenarioStep> steps;
    private List<Action> stepActions;
    private int currentStep = 0;
    private string cprSaveFilePath;

    private List<Canvas> prompts;
    private List<GameObject> objectiveObjects;
    private List<GameObject> modePanelElements;
    private Camera mainCamera;

    void Awake()
    {
        CprSceneManager sceneMgr = (CprSceneManager)FindObjectOfType(typeof(CprSceneManager));

        prompts = sceneMgr.prompts;
        objectiveObjects = sceneMgr.objectiveObjects;
        modePanelElements = sceneMgr.modePanelElements;
        mainCamera = sceneMgr.mainCamera;

        menuPanel = sceneMgr.modePanelElements[0];
        menuText = sceneMgr.modePanelElements[1];
        menuTrainingBtn = sceneMgr.modePanelElements[2];
        menuTestingBtn = sceneMgr.modePanelElements[3];
        menuModeSelectionBtn = sceneMgr.modePanelElements[5];
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
        CreateFolderAndFile();
        GetSaveData();

        // disable all text prompts until user selects mode
        foreach (var prompt in prompts)
        {
            prompt.gameObject.SetActive(false);
        }

        DisableGameObjects();

        stepActions = new List<Action>();
        stepActions.Add(PhoneSelected);
        stepActions.Add(PatientMoved);
        stepActions.Add(HandsPlaced);

        steps = new List<ScenarioStep>();
        for (int i = 1; i <= 4; i++)
        {
            ScenarioStep tempStep = new ScenarioStep();
            tempStep.step = i;
            tempStep.completed = false;
            steps.Add(tempStep);
        }

        menuPanel.SetActive(true);
        AdjustMenuElements(false);

        cprNpc.SetActive(false);

        // training menu element
        menuTrainingBtn.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerTrainingMode);
    }

    private void TriggerTrainingMode(SelectEnterEventArgs args)
    {
        StartTrainingMode();
    }

    private void StartTrainingMode()
    {
        //DisableGameObjects();

        cprNpc.SetActive(true);

        menuPanel.SetActive(false);
        patientModel.SetActive(true);
        phone.SetActive(true);
        phoneReceiver.SetActive(true);
        prompts[0].gameObject.SetActive(true);

        CprEventHandler.onPhoneSelected += PhoneSelected;
        CprEventHandler.onPatientMoved += PatientMoved;
        CprEventHandler.onHandsPlaced += HandsPlaced;
        CprEventHandler.onCPRStarted += CPRStarted;
        CprEventHandler.onTrainingCompleted += TrainingCompleted;
        CprEventHandler.onProgressSave += SaveData;

        currentStep = 1;
        // Resume training from step that user last left on
        LoadData();
        CprEventHandler.TriggerTrainingMode();
    }

    private void PhoneSelected()
    {
        ValidateStep(1);

        if (steps[0].completed)
        {
            CprEventHandler.TriggerVerifiedPhoneInteraction();
            cprSetup.SetActive(true);
            patientModel.SetActive(true);
            CprEventHandler.TriggerSaving();
        }
    }

    private void PatientMoved()
    {
        ValidateStep(2);

        if (steps[1].completed)
        {
            CprEventHandler.TriggerVefiriedPatientInteraction();
            cprSetup.transform.localPosition = new Vector3(1.36f, -1.26f, cprSetup.transform.localPosition.z);
            patientModel.transform.localPosition = new Vector3(0.0f, 1.45f, patientModel.transform.localPosition.z);
            CprEventHandler.TriggerSaving();
        }
    }

    private void HandsPlaced()
    {
        ValidateStep(3);

        if (steps[2].completed)
        {
            CprEventHandler.TriggerVerifiedHandsInteraction();
            EnableGameObjects();
            menuPanel.SetActive(false);
            CprEventHandler.TriggerSaving();

            CprEventHandler.onHandsPlaced -= HandsPlaced;
        }
    }

    private void CPRStarted()
    {
        ValidateStep(4);

        if (steps[3].completed)
        {
            EnableGameObjects();
            menuPanel.SetActive(false);
            CprEventHandler.TriggerSaving();

            CprEventHandler.onCPRStarted -= CPRStarted;
        }
    }

    private void TrainingCompleted()
    {
        AdjustMenuElements(true);
        menuPanel.SetActive(true);

        cprBtnSetup.SetActive(false);
        cprProgressBars.SetActive(false);
        CprEventHandler.TriggerSaving();

        CprEventHandler.onTrainingCompleted -= TrainingCompleted;
    }

    private void EnableGameObjects()
    {
        foreach (var obj in objectiveObjects)
        {
            obj.SetActive(true);
        }
    }

    private void DisableGameObjects()
    {
        for (int i = 0; i < objectiveObjects.Count; i++)
        {
            // disable CPR pressure button and progress bars
            if (i != 0 && i != 5)
            {
                objectiveObjects[i].SetActive(false);
            }
        }
    }

    private void TriggerReload(SelectEnterEventArgs args)
    {
        UnsubscribeFromObjectives();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UnsubscribeFromObjectives()
    {
        CprEventHandler.onPhoneSelected -= PhoneSelected;
        CprEventHandler.onPatientMoved -= PatientMoved;
        CprEventHandler.onHandsPlaced -= HandsPlaced;
        CprEventHandler.onCPRStarted -= CPRStarted;
        CprEventHandler.onTrainingCompleted -= TrainingCompleted;
        CprEventHandler.onTrainingModeSelected -= StartTrainingMode;
        CprEventHandler.onProgressSave -= SaveData;
    }

    // Completion menu will show in front of user after training is completed
    // with additional buttons for mode selection and main menu
    private void AdjustMenuElements(bool finish)
    {
        menuModeSelectionBtn.SetActive(!menuModeSelectionBtn.activeSelf);
        menuMainMenuBtn.SetActive(!menuMainMenuBtn.activeSelf);

        if (finish)
        {
            TextMeshProUGUI[] info = menuText.GetComponentsInChildren<TextMeshProUGUI>();
            info[0].text = "Congratulations!";
            info[1].text = "You have completed CPR Training Mode. Testing mode is now unlocked";

            menuPanel.transform.position = new Vector3(mainCamera.transform.position.x / 2, transform.position.y + 0.3f, mainCamera.transform.position.z);

            menuModeSelectionBtn.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerReload);
            var trainingBtnPos = menuTrainingBtn.transform.position;
            var modeSelectBtnPos = menuModeSelectionBtn.transform.position;
            menuTrainingBtn.SetActive(false);

            // Mode selection button
            menuModeSelectionBtn.transform.position = trainingBtnPos;
            // Main menu element
            menuMainMenuBtn.transform.position = modeSelectBtnPos;
        }
    }

    // Main logic control method that validates
    // each step to confirm that it was completed
    // or trigger corresponding response that it is wrong step
    private void ValidateStep(int curStep)
    {
        int curStepIndex = curStep - 1;

        if (curStep == currentStep && !steps[curStepIndex].completed)
        {
            steps[curStepIndex] = UpdateStep(curStep, true);

            prompts[curStepIndex].gameObject.SetActive(false);
            if (curStep < 4)
            {
                prompts[curStepIndex + 1].gameObject.SetActive(true);
            }

            if (!CprEventHandler.trainingResumed)
            {
                CprEventHandler.TriggerCorrectAction();
            }
        }
        else
        {
            if (!CprEventHandler.trainingResumed)
            {
                CprEventHandler.TriggerIncorrectAction();
            }
        }
    }

    private ScenarioStep UpdateStep(int step, bool completed)
    {
        ScenarioStep tempStep = new ScenarioStep();

        if (!CprEventHandler.trainingResumed)
        {
            if (currentStep < 4)
            {
                currentStep = step + 1;
            }
        }
        tempStep.step = step;
        tempStep.completed = completed;
        tempStep.correct = true;

        return tempStep;
    }

    #region - save/load functionality for CPR Training mode
    private void CreateFolderAndFile()
    {
        System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/json saves");

        if (!File.Exists(cprSaveFilePath))
        {
            using (var sw = new StreamWriter(cprSaveFilePath, true))
            {
                sw.Write("{\"lastFunctionFiredIndex\":-1,\"trainingComplete\":false,\"testingComplete\":false}");
            }
        }
    }

    private void SaveData()
    {
        if (!CprEventHandler.trainingResumed)
        {
            CprSaveData saveData = new CprSaveData();
            saveData.lastFunctionFiredIndex = currentStep;
            saveData.trainingCompleted = CprEventHandler.trainingCompleted;
            saveData.testingCompleted = CprEventHandler.testingCompleted;

            File.WriteAllText(cprSaveFilePath, JsonUtility.ToJson(saveData));
        }
    }

    private void GetSaveData()
    {
        if (File.Exists(cprSaveFilePath))
        {
            string json = File.ReadAllText(cprSaveFilePath);
            saveData = JsonConvert.DeserializeObject<CprSaveData>(json);

            if (saveData.lastFunctionFiredIndex > 1)
            {
                CprEventHandler.ResumeTraining();
            }
            if (saveData.lastFunctionFiredIndex > 1 || (saveData.lastFunctionFiredIndex == -1 && saveData.trainingCompleted))
            {
                CprEventHandler.trainingCompleted = saveData.trainingCompleted;
            }
        }
    }

    private void LoadData()
    {
        if (saveData.lastFunctionFiredIndex > 1)
        {
            // CprEventHandler.ResumeTraining();
            FlagPreviousInteractions();
        }
    }

    // Runs methods for previous steps if user left training unfinished
    private void FlagPreviousInteractions()
    {
        int j = 0;

        for (currentStep = 1; currentStep < saveData.lastFunctionFiredIndex; currentStep++)
        {
            j = currentStep - 1;
            if (j <= 2)
            {
                stepActions[j].Invoke();
            }
        }
        CprEventHandler.trainingResumed = false;
        currentStep = saveData.lastFunctionFiredIndex;
        CprEventHandler.NotifyLoadingFinished();
    }
    #endregion - save/load functionality for CPR Training mode

    private void OnDestroy()
    {
        UnsubscribeFromObjectives();
    }
}