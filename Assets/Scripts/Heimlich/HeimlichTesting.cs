using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class HeimlichTesting : MonoBehaviour
{
    public AudioSource audioSource; // audio source reference

    //sounds for appropriate step
    private AudioClip test_mode;
    private AudioClip wrong;
    private AudioClip correct;
    private AudioClip finish;

    //menu panels and buttons
    private GameObject menuPanel;
    private GameObject menuText;
    private GameObject menuTrainingBtn;
    private GameObject menuTestingBtn;
    private GameObject menuMainMenuBtn;
    private GameObject menuModeBtn;

    
    private GameObject BtnSetup;
    private GameObject Btn2;
    private GameObject Npc;
    private HeimlichSaveDataTest saveData;
    private Reset_data data;

    public string[] step_desc = new string[] { "Interacted with patient", "Moved to patient location", "Performed 5 back blows", "Performed the Heimlich Manuever" };
    public bool[] step_correct = new bool[4];//bolean for steps

    private int currentStep = 0;
    private int errorCount = 0;
    public float countdownTimer = 30.0f;

    private bool testing;
    public float initialtime = 0f;
    public double currtime = 0f;
    private string HeimlichSaveFilePath;
    public int global_step;
    //list of gameobject needed for scenario
    [SerializeField] List<GameObject> objectiveObjects;
    [SerializeField] List<GameObject> modePanelElements;
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] List<AudioClip> clips;
    [SerializeField] TextMeshProUGUI subText;
    [SerializeField] GameObject sub_can;
    [SerializeField] GameObject particle;
    [SerializeField] Camera camera;
    public GameObject table;

    private double points;
    bool first = true;

    private void Start()//called on scene start
    {
        testing = false;
        table.SetActive(true);
        camera.enabled = true;
        global_step = 0;
        menuPanel = modePanelElements[0];
        menuText = modePanelElements[1];
        menuTrainingBtn = modePanelElements[2];
        menuTestingBtn = modePanelElements[3];
        menuMainMenuBtn = modePanelElements[4];
        menuModeBtn = modePanelElements[5];
        test_mode = clips[0];
        wrong = clips[1];
        correct = clips[2];
        finish = clips[3];
        sub_can.SetActive(false);

        
        BtnSetup = objectiveObjects[2];
        Btn2 = objectiveObjects[3];
        
        Npc = objectiveObjects[0];
        string SavePath = Application.persistentDataPath + "/json saves/Heimlich.json";

        if (File.Exists(SavePath))
        {
            // Read the existing content of the file
            string json = File.ReadAllText(SavePath);

            data = JsonConvert.DeserializeObject<Reset_data>(json);
            if (!data.trainingCompleted)
            {
                //testing menu element - trigger doestn exist if training is completed
                menuTestingBtn.GetComponent<Image>().color = Color.gray;
                menuTestingBtn.GetComponent<XRSimpleInteractable>().hoverEntered.AddListener(EnableTestingMenuHint);
                menuTestingBtn.GetComponent<XRSimpleInteractable>().hoverExited.AddListener(DisableTestingMenuHint);

            }
            else
            {
                BtnSetup.SetActive(false);

                //disable testing menu hint - if training mode is completed, trigger testing mode enabled for button
                menuTestingBtn.GetComponentsInChildren<TextMeshProUGUI>()[1].enabled = false;
                menuTestingBtn.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerTestingMode);
            }
        }
    }
    void PlayClip(AudioClip clip)//funciton to play audio
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
    private void TriggerTestingMode(SelectEnterEventArgs args)
    {
        BeginTesting();
        testing = true;
        particle.SetActive(false);
    }

    public void BeginTesting()
    {
        camera.enabled = true;
        initialtime = countdownTimer;
        PlayClip(test_mode);
        points = 0;
        sub_can.SetActive(true);

        objectiveObjects[0].SetActive(true);//npc
        objectiveObjects[1].SetActive(true);//trigger where to stand for step2
        objectiveObjects[2].SetActive(true);//button on back for step 3
        subText.text = "Welcome to testing mode. In this mode your knowledge will be tested based on what you have learned in training mode. There will be no prompts or feedback during this mode.";

        Npc.SetActive(true);
        Npc.transform.position = new Vector3(2.05999994f, 0f, 0.340000004f);//make sure proper position
        //enforces proper position on start

        menuPanel.SetActive(false);
        
        //subscribe to event listeners
        HeimlichEvents.onPatientMoved += PatientMoved;
        HeimlichEvents.onBackBlowsStarted += BackBlows;
        HeimlichEvents.onHandsPlaced += HandsPlaced;
        HeimlichEvents.onHeimlichStarted += HeimlichStarted;
        HeimlichEvents.onTestingCompleted += TestingCompleted;
        HeimlichEvents.onTestingCountdown += BeginTimerCountdown;
        HeimlichEvents.onTestingCompleted += StopTimerCountdown;

        string SavePath = Application.persistentDataPath + "/json saves/Heimlich.json";

        if (File.Exists(SavePath))
        {

            // Read the existing content of the file
            string json = File.ReadAllText(SavePath);

            data = JsonConvert.DeserializeObject<Reset_data>(json);

            // Update the value of lastFunctionFiredIndex to 5
            data.lastFunctionFiredIndex = 5;
            string updatedJson = JsonConvert.SerializeObject(data);

            // Overwrite the contents of the file with the updated JSON string
            File.WriteAllText(SavePath, updatedJson);
        }


        HeimlichEvents.TriggerTestingMode();

        global_step = 1; //makes sure step 1 is first step always

    }

    private void PatientMoved()
    {
        currentStep = 1;
        camera.enabled = true;
        sub_can.SetActive(false);
        
        if (ValidateStep(1))
        {
            
            table.SetActive(false);
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

        }

    }

    private void BackBlows()//follow patient to dest and triggers step 3
    {
        currentStep = 2;
        camera.enabled = true;

        if (ValidateStep(2))
        {
            menuPanel.SetActive(false);
            HeimlichEvents.onBackBlowsStarted -= BackBlows;
        }
    }

    private void HandsPlaced()
    {

        currentStep = 3;

            if (ValidateStep(3))
            {
                objectiveObjects[3].SetActive(true);
                menuPanel.SetActive(false);
                HeimlichEvents.onHandsPlaced -= HandsPlaced;
            }
    }

    private void HeimlichStarted()
    {
        currentStep = 4;

        if (ValidateStep(4))
        {

            menuPanel.SetActive(false);

            HeimlichEvents.onHeimlichStarted -= HeimlichStarted;
            TestingCompleted();
        }
    }

    private IEnumerator ScenarioCountdown() //countdown timer for testing mode
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
                
                    countdownText.text = String.Format("Timer: <color=\"{0}\">{1} </color>", textColor, countdownTimer.ToString());
                
            }
            else
            {
                countdownText.text = "";
                InitiateScenarioTimeout();
            }
            yield return new WaitForSeconds(1);
        }
    }

    private void BeginTimerCountdown() //starts the time routine
    {
        StartCoroutine(ScenarioCountdown());
    }

    private void StopTimerCountdown() //stops the timer
    {
        StopCoroutine(ScenarioCountdown());
    }

    private void InitiateScenarioTimeout()//triggers if timer reaches 0 
    {
        errorCount++;
        HeimlichEvents.TriggerTestingCompletion();
        HeimlichEvents.TriggerTestingTimeout();
    }

    private void TestingCompleted() //testing comple function
    {
        HeimlichEvents.testingCompleted = true;
        menuModeBtn.SetActive(!menuModeBtn.activeSelf);


        BtnSetup.SetActive(false);
        Btn2.SetActive(false);

        menuPanel.SetActive(true);
        ShowCompletionDialog();

        menuPanel.transform.position = new Vector3(camera.transform.position.x / 2, transform.position.y + 0.3f, camera.transform.position.z);
        HeimlichEvents.TriggerTest();

        HeimlichEvents.onPatientMoved -= PatientMoved;
        HeimlichEvents.onBackBlowsStarted -= BackBlows;
        HeimlichEvents.onHandsPlaced -= HandsPlaced;
        HeimlichEvents.onHeimlichStarted -= HeimlichStarted;
        HeimlichEvents.onTestingCompleted -= TestingCompleted;

        UnsubscribeFromObjectives();

        menuMainMenuBtn.SetActive(false);
        menuModeBtn.GetComponent<XRSimpleInteractable>().selectEntered.RemoveAllListeners();
        menuModeBtn.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerSceneReload);

        menuTestingBtn.SetActive(false);
        menuTrainingBtn.SetActive(false);

        string dateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss"); //adds a date time for that particular test file and will be sent to json
        saveData = new HeimlichSaveDataTest();
        HeimlichSaveFilePath = Application.persistentDataPath + "/json tests/Heimlich-test-" + dateTime + ".json";
        CreateFolderAndFile();
    }

    private void ShowCompletionDialog()//last dialog seen after testing mdoe finishes
    {
        menuMainMenuBtn.SetActive(!menuMainMenuBtn.activeSelf);
        countdownText.enabled = false;
        currtime = initialtime - countdownTimer;
        if (currtime < 0)
            currtime += (2 * -(currtime));//if timer is more since we add +5 s for correct steps, this cna result in negative number so take only the positive ie. if its -7s it will be 7s.
        
        TextMeshProUGUI[] info = menuText.GetComponentsInChildren<TextMeshProUGUI>();
        if (errorCount == 0) //no mistakes
        {
            info[0].text = "\nYou have completed Heimlich Testing mode \nwithout any mistakes\nScore: " + points;
            info[1].text = "Congratulations!";
            PlayClip(finish);
        }
        if (errorCount > 0)//some mistakes
        {
            info[0].text = "\nYou have completed Heimlich Testing mode \nwith some mistakes. \nScore: " + (points-errorCount); 
            info[1].text = "Ooops!";
        }
    }
    private void TriggerSceneReload(SelectEnterEventArgs args)//reload scene on start
    {
        UnsubscribeFromObjectives();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UnsubscribeFromObjectives()
    {
        HeimlichEvents.onPatientMoved -= PatientMoved;
        HeimlichEvents.onBackBlowsStarted -= BackBlows;
        HeimlichEvents.onHandsPlaced -= HandsPlaced;
        HeimlichEvents.onHeimlichStarted -= HeimlichStarted;
        HeimlichEvents.onTestingCompleted -= TestingCompleted;
        HeimlichEvents.onTestingCountdown -= BeginTimerCountdown;
        HeimlichEvents.onTestingCompleted -= StopTimerCountdown;
    }
    
    private void EnableTestingMenuHint(HoverEnterEventArgs args)//used for enabling the hint menu for testing - tells us if we have compelted training mode or not
    {
        menuTestingBtn.GetComponentsInChildren<TextMeshProUGUI>()[1].enabled = true;
    }

    private void DisableTestingMenuHint(HoverExitEventArgs args)
    {
        menuTestingBtn.GetComponentsInChildren<TextMeshProUGUI>()[1].enabled = false;
    }

    private bool ValidateStep(int currentStep) //validation of steps
    {

        if (currentStep == global_step)
        {
            global_step++;
            PlayClip(correct);
            points++;
            testing = true;
            if(countdownTimer < initialtime)
            countdownTimer += 5;
            if (countdownTimer >= initialtime)
                countdownTimer = initialtime;
            if (first == true)
                step_correct[(currentStep - 1)] = true;
            else
            {
                step_correct[(currentStep - 1)] = false;
                first = true;
            }

            return true;
        }
        else
        {
            errorCount++;
            PlayClip(wrong);
            countdownTimer -= 5;
            first = false;
            return false;

        }
        
    }

    private void CreateFolderAndFile()
    {
        DateTime theTime = DateTime.Now;
        String datetime = theTime.ToString("R");

        Directory.CreateDirectory(Application.persistentDataPath + "/json tests");

        if (!File.Exists(HeimlichSaveFilePath))//writes the json file
        {
            using (var sw = new StreamWriter(HeimlichSaveFilePath, true))
            {
                sw.Write("{");
                sw.Write("\"correct\":" + points + ",");
                sw.Write("\"incorrect\":" + errorCount + ",");
                sw.Write("\"timeElapsed\":\"" + currtime + "\",");
                sw.Write("\"testingComplete\":true,");
                sw.Write("\"dateStamp\":\"" + datetime + "\",");
                sw.Write("\"steps\":[");
                for (int i = 0; i < step_desc.Length; i++)
                {
                    sw.Write("{");
                    sw.Write("\"description\":\"" + step_desc[i] + "\",");
                    sw.Write("\"correct\":" + step_correct[i].ToString().ToLower() + "");
                    if (i == step_desc.Length - 1)
                    {
                        sw.Write("}");
                    }
                    else
                    {
                        sw.Write("},");
                    }
                }
                sw.Write("]}");
            }
        }
    }

    private ScenarioStep UpdateStep(int step, bool completed)
    {
        ScenarioStep tempStep = new ScenarioStep();
        
        currentStep = step + 1;
        
        tempStep.step = step;
        tempStep.completed = completed;

        return tempStep;
    }
    private void SaveDataTest()//writes data to json
    {
        
            HeimlichSaveDataTest saveDataTest = new HeimlichSaveDataTest();
            saveDataTest.testingCompleted = HeimlichEvents.testingCompleted;
            saveDataTest.score = points;
            File.WriteAllText(HeimlichSaveFilePath, JsonUtility.ToJson(saveDataTest));
        
    }
    private void OnDestroy()
    {
        UnsubscribeFromObjectives();
    }

    private class ScenarioStep
    {
        public int step { get; set; }
        public bool completed { get; set; }
    }
    private class HeimlichSaveDataTest
    {
        public double score;
        public DateTime date;
        public bool testingCompleted = true;
    }

    private class Reset_data //resets the data after each new testing attempt
    {
        public int lastFunctionFiredIndex { get; set; }
        public bool trainingCompleted { get; set; }
        public bool testingCompleted { get; set; }

    }
}
