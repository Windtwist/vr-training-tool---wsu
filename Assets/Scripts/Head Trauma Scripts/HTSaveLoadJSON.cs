using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;


//This class is the two boolean variables that will be saved to JSON
public class HTSaveData
{
    public bool testingUnlocked = false;
    public bool trainingSave = false;

}

//This class functions as a controller for training mode,
//it also contains the functions to save/load data.
public class HTSaveLoadJSON : MonoBehaviour
{
    //audio source to play audio
    [SerializeField] AudioSource audioSource;

    //local variables for the ones we will save
    public static bool testingUnlocked;
    public static bool trainingSave;

    //These are all the gameobjects that will need to be
    //enabled/disabled for testing/training mode
    public GameObject testButton;
    public GameObject phone;
    public GameObject phoneTest;
    public GameObject step2Box;
    public GameObject step2BoxTest;
    public GameObject sinkDoor;
    public GameObject sinkDoorTest;
    public GameObject bandage;
    public GameObject bandageTest;
    public GameObject attachPoint;
    public GameObject attachPointTest;
    public GameObject passCanvas;
    public GameObject failCanvas;
    public GameObject step3Indicator;

    //These gameobjects will need to be enabled/disabled based
    //on whether or not training mode needs to load save data 
    //or start from the beginning
    public GameObject stepTrackingCanvas;
    public AudioClip step3aSound;
    public AudioClip step1Sound;
    public GameObject sinkDoorTT;
    public GameObject sinkDoorGrabBox;
    public GameObject phoneTT;
    [SerializeField] public TextMeshProUGUI step2;
    [SerializeField] public TextMeshProUGUI step1;

    //This string is for the file path location of save data
    String path;
    
    //Start is called as soon as the scene is loaded. 
    public void Start() 
    {
        //save correct path into the path string & create directories. 
        path = Application.persistentDataPath + "/json saves/HeadTrauma.json";
        System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/json saves");
        System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/json loader");

        //check if file exists, and if it doesn't, then create it.
        if (!File.Exists(path))
        {
            using (var sw = new StreamWriter(path, true))
            {
                sw.Write("{\"testingUnlocked\":false,\"trainingSave\":false,}");
            }
        }

        //read data from file and store it in local variables 
        string jsonString = File.ReadAllText(path);
        HTSaveData HTSave = JsonConvert.DeserializeObject<HTSaveData>(jsonString);
        testingUnlocked = HTSave.testingUnlocked;
        trainingSave = HTSave.trainingSave;

        //If training mode has been completed,
        //this will make the testing mode button clickable
        if (testingUnlocked) { testButton.GetComponent<Button>().interactable =  true; }

    }
    
    //This function is called when training mode starts
    public void startTraining()
    {
        //Set training mode objects to true
        phone.SetActive(true);
        sinkDoor.SetActive(true);
        bandage.SetActive(true);
        attachPoint.SetActive(true);
        step2Box.SetActive(true);
        step3Indicator.SetActive(false);

        //Set testing mode objects to false
        phoneTest.SetActive(false);
        sinkDoorTest.SetActive(false);
        bandageTest.SetActive(false);
        attachPointTest.SetActive(false);
        step2BoxTest.SetActive(false); 

        //If there is a training mode save then this block of code will set up
        //the scene to continue from after step 2 completion.
        if(trainingSave)
        {
            step3aSound = Resources.Load<AudioClip>("HeadTraumaAudio/HT Step 3A");
            step2Box.SetActive(false);
            phone.SetActive(false);
            step1.GetComponent<TextMeshProUGUI>().color = Color.green;
            step2.GetComponent<TextMeshProUGUI>().color = Color.green;
            sinkDoorTT.SetActive(true);
            sinkDoorGrabBox.SetActive(true);
            audioSource.clip = step3aSound;
            audioSource.Play();


        }
        //Otherwise training starts as normal
        else 
        {
            phoneTT.SetActive(true);
            step1Sound = Resources.Load<AudioClip>("HeadTraumaAudio/HT Step 1");
            audioSource.clip = step1Sound;
            audioSource.Play();
            
        }

    }
    
    //This function writes the save data to the file
    private void WriteSaveData()
    {
        HTSaveData save = new()
        {
            testingUnlocked = testingUnlocked,
            trainingSave = trainingSave
        };

        Debug.Log(JsonUtility.ToJson(save));

        File.WriteAllText(path, JsonUtility.ToJson(save));
    }

    //This function is called after step 2 is completed,
    //and saves progress of user
    public void saveTraining()
    {
        trainingSave = true;
        WriteSaveData();
    }
    //This function is called when training mode is completed,
    //then saves progress of user
    public void unlockTesting()
    {
        testingUnlocked = true;
        WriteSaveData();
    }
    //This function is called when training mode is completed,
    //so next time training mode is selected it starts from the beginning
    public void trainingComplete()
    {
        trainingSave = false;
        WriteSaveData();
    }
    //This function resets all the save data
    public void resetSaveData()
    {
        trainingSave = false;
        testingUnlocked = false;
    }
    




   
}
