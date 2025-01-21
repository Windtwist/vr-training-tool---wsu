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

public class HeimlichTraining : MonoBehaviour
{
    public AudioSource audioSource; // Audio source

    //menu panels and buttons - these are not the step panels
    private GameObject menuPanel;
    private GameObject menuText;
    private GameObject menuTrainingBtn;
    private GameObject menuTestingBtn;
    private GameObject menuModeSelectionBtn;
    private GameObject menuMainMenuBtn;

    //step panel text - associated with each step
    private TextMeshProUGUI info;
    private TextMeshProUGUI check;
    private TextMeshProUGUI follow;
    private TextMeshProUGUI blow;
    private TextMeshProUGUI heim;

    //audio clips associated with each step
    private AudioClip trainMode;
    private AudioClip blows;
    private AudioClip heimlich;
    private AudioClip trainMode_C;
    private AudioClip wrong;
    private AudioClip intro;

    private GameObject patientModel;
    private GameObject BtnSetup;
    private GameObject Btn2;
    private GameObject ProgressBars;
    private GameObject Setup;
    private GameObject Npc;
    private HeimlichSaveData saveData;

    //List for our objects that move the scneario forward
    private List<ScenarioStep> steps;
    private List<Action> stepActions;
    private int currentStep = 0;
    private string HeimlichSaveFilePath; //path for training file location
    [SerializeField] GameObject StepCanvas;
    [SerializeField] List<Canvas> prompts;
    [SerializeField] List<GameObject> objectiveObjects;
    [SerializeField] List<GameObject> modePanelElements;
    [SerializeField] List<TextMeshProUGUI> UImodeHelpers;
    [SerializeField] TextMeshProUGUI subText;
    [SerializeField] GameObject trigger_anim_skip;
    [SerializeField] Camera camera;
    [SerializeField] GameObject sub_can; //canvas for step subtitles

    public GameObject table;

    [SerializeField] List<AudioClip> clips;

    private bool skip_anim; //animation skip trigger - position set after training resumes
    private Animator animator; //calls for animator for npc object to itneract with animations

    
    void Start()//called on scene start
    {
        table.SetActive(true);
        trigger_anim_skip.SetActive(false);
        camera.enabled = true;
        skip_anim = false;
        menuPanel = modePanelElements[0];
        menuText = modePanelElements[1];
        menuTrainingBtn = modePanelElements[2];
        menuTestingBtn = modePanelElements[3];
        menuModeSelectionBtn = modePanelElements[5];
        menuMainMenuBtn = modePanelElements[4];

        menuMainMenuBtn.SetActive(false);
        menuModeSelectionBtn.SetActive(false);

        info = UImodeHelpers[0]; 
        check = UImodeHelpers[1];
        follow = UImodeHelpers[2];
        blow = UImodeHelpers[3];
        heim = UImodeHelpers[4];

        info.enabled = false;
        check.enabled = false;
        follow.enabled = false;
        blow.enabled = false;
        heim.enabled = false;


        trainMode = clips[0];
        blows = clips[1];
        heimlich = clips[2];
        trainMode_C = clips[3];
        wrong = clips[4];
        intro = clips[5];

        
        BtnSetup = objectiveObjects[1];
        Btn2 = objectiveObjects[2];
        Npc = objectiveObjects[0];

        saveData = new HeimlichSaveData(); //new saveData object which is used for json
        HeimlichSaveFilePath = Application.persistentDataPath + "/json saves/Heimlich.json";
        CreateFolderAndFile(); //creates directort and folder
        GetSaveData(); //checks for any previous saved data

        // disable all text prompts until user selects a mode
        foreach (var prompt in prompts)
        {
            prompt.gameObject.SetActive(false);
        }
        //dissable the two second triggers until user interacts with first step which is patient
        objectiveObjects[1].SetActive(false);
        objectiveObjects[2].SetActive(false);

        stepActions = new List<Action>();
        stepActions.Add(PatientMoved);
        stepActions.Add(BackBlows);

        steps = new List<ScenarioStep>();
        ScenarioStep tempStep = new ScenarioStep();//updates scenario step
        for (int i = 1; i <= 5; i++)
        {
            tempStep.step = i;
            tempStep.completed = false;
            steps.Add(tempStep);
        }

        menuPanel.SetActive(true);
        AdjustMenuElements(false);

        Npc.SetActive(false);

        // training menu element
        menuTrainingBtn.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(StartTrainingMode);//assign training mode trigger to menu training btn

        PlayClip(intro); //play intro clip of training mode
    }

    void PlayClip(AudioClip clip) //function that plays the audio
    {
        audioSource.clip = clip;
        audioSource.Play();
    }


    private void StartTrainingMode(SelectEnterEventArgs args)
    {
        sub_can.SetActive(true);
        camera.enabled = true;
        Canvas textparent = subText.transform.parent.GetComponent<Canvas>();
        textparent.enabled = true;
        StepCanvas.SetActive(true);
        info.enabled = true;
        check.enabled = true;

        Npc.SetActive(true);

        menuPanel.SetActive(false);
        prompts[0].gameObject.SetActive(true);

        //subscribes to events - need to do this so all triggers listen to events and must unsubscribe after we are done
        HeimlichEvents.onPatientMoved += PatientMoved;
        HeimlichEvents.onBackBlowsStarted += BackBlows;
        HeimlichEvents.onHandsPlaced += HandsPlaced;
        HeimlichEvents.onHeimlichStarted += HeimlichStarted;
        HeimlichEvents.onTrainingCompleted += TrainingCompleted;
        HeimlichEvents.onProgressSave += SaveData;
        HeimlichEvents.onAuto_move +=Move;
       
        currentStep = 1; //enforces first step
       
        HeimlichEvents.TriggerTrainingMode();
        LoadData();//checks if there is any previous data
        
        if (currentStep == 1)
        {
            PlayClip(trainMode);
            showSub(currentStep);
        }
    }

    private void showSub(int currentStep)//function to associate a subtitle text with current step 
    {
        switch (currentStep)
        {
            case 1: subText.text = "Welcome to Training mode, in this mode you need to complete steps that are specified with interactive prompts. Please turn around to locate the first step, indicated with a text prompt over the object.";
                break;
            case 2:
                subText.text = "Great Job, now wait for the patient to stop. After patient stops, get closer to the patient.";
                break;
            case 3:
                subText.text = "Give 5 back blows on the designated area. After each blow haptic feedback will be given. After 5 back blows, make sure both hands are on the button after your camera view switches.";
                break;
            case 4:
                sub_can.SetActive(false);
                break;
            case 5:
                sub_can.SetActive(false);
                break;
        }

    }
    private void PatientMoved()//first step
    {
        camera.enabled = true;


        ValidateStep(1);

        if (steps[0].completed)
        {
                table.SetActive(false);
                follow.enabled = true;
                // Cross out the text
                check.fontStyle = FontStyles.Strikethrough;
                // Set the text color to green
                check.color = Color.green;

                AudioSource.PlayClipAtPoint(blows, transform.position);
                showSub(currentStep);

                foreach (Transform child in Npc.transform)
                {
                    // Get the box collider component
                    BoxCollider boxCollider = child.GetComponent<BoxCollider>();

                    // If the box collider exists, disable it
                    if (boxCollider != null)
                    {
                        boxCollider.enabled = false;
                    }
                }
            
            HeimlichEvents.TriggerSaving();//after step is complete trigger saving to json
            if (skip_anim)//boolean that cheks if we resumed training or not.
            {
                animator = Npc.GetComponent<Animator>();
                animator.SetTrigger("stand");
                animator.SetBool("Sitting", false);
                Move();
            }
        }
    }
    private void BackBlows()//seconds step
    {
        camera.enabled = true;

        ValidateStep(2);

        if (steps[1].completed || saveData.lastFunctionFiredIndex == 2 || saveData.lastFunctionFiredIndex == 3)
        {
                blow.enabled = true;
                // Cross out the text
                follow.fontStyle = FontStyles.Strikethrough;

                // Set the text color to green
                follow.color = Color.green;
                objectiveObjects[1].SetActive(true);
                menuPanel.SetActive(false);
                showSub(currentStep);

                HeimlichEvents.TriggerSaving();

                HeimlichEvents.onBackBlowsStarted -= BackBlows;  
        }
    }

    public void test_check()//test when user leaves through door to unsubscibe from all events
    {
        HeimlichEvents.trainingCompleted = true;
        HeimlichEvents.onTrainingCompleted -= TrainingCompleted;
    }

    private void HandsPlaced()//step 3
    {

        {
            ValidateStep(3);

            if (steps[2].completed)
            {
                AudioSource.PlayClipAtPoint(heimlich, transform.position);
                showSub(currentStep);

                StepCanvas.SetActive(false);

                objectiveObjects[2].SetActive(true);
                menuPanel.SetActive(false);
                HeimlichEvents.TriggerSaving();

                HeimlichEvents.onHandsPlaced -= HandsPlaced;
            }
        }
    }

    private void HeimlichStarted()//step 4
    {
        ValidateStep(4);

        if (steps[3].completed)
        {

            menuPanel.SetActive(false);
            HeimlichEvents.TriggerSaving();

            HeimlichEvents.onHeimlichStarted -= HeimlichStarted;

            heim.enabled = true;
            // Cross out the text
            blow.fontStyle = FontStyles.Strikethrough;

            // Set the text color to green
            blow.color = Color.green;
            StepCanvas.SetActive(true);

        }
    }

    private void TrainingCompleted()//training complete function
    {
        HeimlichEvents.trainingCompleted = true;

        menuPanel.SetActive(true);
        menuModeSelectionBtn.SetActive(true);

        BtnSetup.SetActive(false);
        Btn2.SetActive(false);
        AudioSource.PlayClipAtPoint(trainMode_C, transform.position);
        showSub(currentStep);

        // Cross out the text
        heim.fontStyle = FontStyles.Strikethrough;

        // Set the text color to green
        heim.color = Color.green;
        HeimlichEvents.TriggerSaving();
        menuMainMenuBtn.SetActive(false);
        menuTestingBtn.SetActive(false);
        menuTrainingBtn.SetActive(false);

        HeimlichEvents.onTrainingCompleted -= TrainingCompleted;
        StepCanvas.SetActive(false);
        AdjustMenuElements(true);


    }
    private void TriggerReload(SelectEnterEventArgs args)//triggers a reload of scene each time we enter scene
    {
        UnsubscribeFromObjectives();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UnsubscribeFromObjectives()//unsubscribes from all listeners
    {
        HeimlichEvents.onPatientMoved -= PatientMoved;
        HeimlichEvents.onBackBlowsStarted -= BackBlows;
        HeimlichEvents.onHandsPlaced -= HandsPlaced;
        HeimlichEvents.onHeimlichStarted -= HeimlichStarted;
        HeimlichEvents.onTrainingCompleted -= TrainingCompleted;
        HeimlichEvents.onProgressSave -= SaveData;
        HeimlichEvents.onAuto_move -= Move;
    }

    private void AdjustMenuElements(bool finish)//adjusts the menu panel for completion fo training
    {
        if (finish)
        {
            TextMeshProUGUI[] info = menuText.GetComponentsInChildren<TextMeshProUGUI>();
            info[0].text = "Congratulations!\nYou have completed Heimlich Maneuver Training Mode. \nTesting mode is now unlocked";
            info[1].text = "";
            menuPanel.transform.position = new Vector3(camera.transform.position.x / 2, transform.position.y + 0.3f, camera.transform.position.z);
            menuModeSelectionBtn.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerReload);
        }
    }

    private void ValidateStep(int curStep)//used for validation of steps 
    {
        int curStepIndex = curStep - 1;

        sub_can.SetActive(true);
        
        if (curStep == currentStep && !steps[curStepIndex].completed || saveData.lastFunctionFiredIndex == 2 || saveData.lastFunctionFiredIndex == 3)
        {
            steps[curStepIndex] = UpdateStep(curStep, true);
            

            prompts[curStepIndex].gameObject.SetActive(false);
            if (curStep < 4)
            {
                prompts[curStepIndex + 1].gameObject.SetActive(true);
            }

            if (!HeimlichEvents.trainingResumed)
            {
                HeimlichEvents.TriggerCorrectAction();
            }
        }
        else
        {
            if (!HeimlichEvents.trainingResumed)
            {
                HeimlichEvents.TriggerIncorrectAction();
                AudioSource.PlayClipAtPoint(wrong, transform.position);
            }
        }
    }

    private ScenarioStep UpdateStep(int step, bool completed)//update step used in validate step
    {
        ScenarioStep tempStep = new ScenarioStep();

        if (!HeimlichEvents.trainingResumed)
        {
            currentStep = step + 1;
        }
        tempStep.step = step;
        tempStep.completed = completed;

        return tempStep;
    }

    private void Move()//function for resume training to move npc to correct location
    {
        Npc.transform.position = new Vector3(-1.377f, 0f, 0f);

    }

    #region - save/load functionality for Heimlich Training mode
    private void CreateFolderAndFile()
    {
        Directory.CreateDirectory(Application.persistentDataPath + "/json saves");

        if (!File.Exists(HeimlichSaveFilePath))
        {
            using (var sw = new StreamWriter(HeimlichSaveFilePath, true))
            {
                sw.Write("{\"lastFunctionFiredIndex\":-1,\"trainingComplete\":false,\"testingComplete\":false}");
            }
        }
    }

    private void SaveData()
    {
        if (!HeimlichEvents.trainingResumed)
        {
            HeimlichSaveData saveData = new HeimlichSaveData();
            saveData.lastFunctionFiredIndex = currentStep;
            saveData.trainingCompleted = HeimlichEvents.trainingCompleted;
            saveData.testingCompleted = HeimlichEvents.testingCompleted;

            File.WriteAllText(HeimlichSaveFilePath, JsonUtility.ToJson(saveData));
        }
    }

    private void GetSaveData()
    {
        if (File.Exists(HeimlichSaveFilePath))
        {
            string json = File.ReadAllText(HeimlichSaveFilePath);
            saveData = JsonConvert.DeserializeObject<HeimlichSaveData>(json);

            if (saveData.lastFunctionFiredIndex > 1 && saveData.lastFunctionFiredIndex !=5)
            {
                HeimlichEvents.trainingCompleted = saveData.trainingCompleted;
                skip_anim = true;
            }
        }
        else
        {
            Debug.Log("Heimlich: No Previouss Save Data");
        }
    }
    private void LoadData()
    {
        if (saveData.lastFunctionFiredIndex > 1)
        {
            FlagPreviousInteractions();
        }
    }

    private void FlagPreviousInteractions()//flags for load data - makes sure to disable last step and move forwards with the next
    {
        if (saveData.lastFunctionFiredIndex == 5)
            saveData.lastFunctionFiredIndex = 1;
        for (currentStep = 1; currentStep < saveData.lastFunctionFiredIndex; currentStep++)
        {
            int j = currentStep - 1;
            stepActions[j].Invoke();
        }
        HeimlichEvents.trainingResumed = false;
        currentStep = saveData.lastFunctionFiredIndex;
    }
    #endregion - save/load functionality for Heimlich Training mode

    private void OnDestroy()
    {
        UnsubscribeFromObjectives();
    }

    [ContextMenu("Autofill Prompts")] //fills prompts make sure they arent empty can cause issues if prompts are empty
    void AutofillPrompts()
    {
        prompts = FindObjectsOfType<Canvas>()
            .Where(t => t.name.ToLower().Contains("prompt")).OrderBy(t => t.name)
            .ToList();
    }


    private class ScenarioStep
    {
        public int step { get; set; }
        public bool completed { get; set; }
    }
    private class HeimlichSaveData //json for saving data
    {
        public int lastFunctionFiredIndex = 0;
        public bool trainingCompleted = false;
        public bool testingCompleted = false;
    }
}


