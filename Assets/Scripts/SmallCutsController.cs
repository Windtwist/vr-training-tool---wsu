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

public class BandageSaveData
{
    public int lastFunctionFiredIndex = 0;
    public bool trainingComplete = false;
    public bool testingComplete = false;
}

public class SmallCutsController : MonoBehaviour
{
    [SerializeField] GameObject gameController;

    [SerializeField] GameObject sinkHandle1;
    [SerializeField] GameObject sinkHandle2;
    [SerializeField] GameObject doorHandle;
    [SerializeField] GameObject bandage;
    [SerializeField] GameObject pressureSocket;
    [SerializeField] GameObject antibioticBottle;
    [SerializeField] GameObject antibioticSocket;
    [SerializeField] GameObject newBandage;
    [SerializeField] GameObject newBandageSocket;

    [SerializeField] GameObject testButton;

    [SerializeField] List<TextMeshProUGUI> stepsTextMeshPros;

    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] GameObject timeoutUI;

    private int countdownTimer = 30;
    private bool isTestMode = false;
    private int lastFunctionFiredIndex = -1;
    private string path;

    public static bool trainingComplete;
    public static bool testingComplete;

    private List<Action<SelectEnterEventArgs>> actionList = new List<Action<SelectEnterEventArgs>>();

    private void Start()
    {
        path = Application.persistentDataPath + "/json saves/SmallCuts.json";
        actionList.Add(TriggerSinkInteraction);
        actionList.Add(TriggerPressureInteraction);
        actionList.Add(TriggerAntibioticAppliedInteraction);

        System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/json saves");
        System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/json loader");

        if (!File.Exists(path))
        {
            using (var sw = new StreamWriter(path, true))
            {
                sw.Write("{\"lastFunctionFiredIndex\":-1,\"trainingComplete\":false,\"testingComplete\":false}");
            }
        }

        string jsonString = File.ReadAllText(path);
        BandageSaveData smallCutsSave = JsonConvert.DeserializeObject<BandageSaveData>(jsonString);
        lastFunctionFiredIndex = smallCutsSave.lastFunctionFiredIndex;
        trainingComplete = smallCutsSave.trainingComplete;
        testingComplete = smallCutsSave.testingComplete;

        if (trainingComplete) {
            testButton.GetComponent<XRSimpleInteractable>().enabled = true;
            testButton.GetComponent<Button>().interactable =  true;
        }
        
        EventHandler.OnSmallCutsClicked += SetIncorrectListeners;
    }

    private void OnDestroy()
    {
        EventHandler.OnSmallCutsClicked -= InitSmallCuts;
        EventHandler.OnSmallCutsClicked -= SetIncorrectListeners;
        EventHandler.OnTimeout -= HandleTimeout;


        if (bandage != null) bandage.GetComponent<XRGrabInteractable>().selectEntered.RemoveAllListeners();
        if (doorHandle != null) doorHandle.GetComponent<XRSimpleInteractable>().selectEntered.RemoveAllListeners();
        if (sinkHandle1 != null)
            sinkHandle1.GetComponent<XRSimpleInteractable>().selectEntered.RemoveAllListeners();
        if (sinkHandle2 != null)
            sinkHandle2.GetComponent<XRSimpleInteractable>().selectEntered.RemoveAllListeners();
        if (pressureSocket != null)
            pressureSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveAllListeners();
        if (antibioticBottle != null)
            antibioticBottle.GetComponent<XRGrabInteractable>().selectEntered.RemoveAllListeners();
        if (antibioticSocket != null)
            antibioticSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveAllListeners();
        if (newBandage != null) newBandage.GetComponent<XRGrabInteractable>().selectEntered.RemoveAllListeners();
        if (newBandageSocket != null)
            newBandageSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveAllListeners();


        WriteSaveData();
    }

    private void WriteSaveData()
    {
        BandageSaveData save = new()
        {
            lastFunctionFiredIndex = lastFunctionFiredIndex,
            trainingComplete = trainingComplete,
            testingComplete = testingComplete
        };

        Debug.Log(JsonUtility.ToJson(save));

        File.WriteAllText(path, JsonUtility.ToJson(save));
    }

    public void SetTestMode(bool test)
    {
        isTestMode = test;
    }

    private void TriggerIncorrectAction(SelectEnterEventArgs args)
    {
        EventHandler.TriggerIncorrectAction();
    }

    private void TriggerCorrectAction(SelectEnterEventArgs args)
    {
        EventHandler.TriggerCorrectAction();
    }

    private void SetIncorrectListeners()
    {
        EventHandler.OnSmallCutsClicked -= SetIncorrectListeners;
        doorHandle.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        bandage.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        antibioticBottle.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        newBandage.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerIncorrectAction);

        if (isTestMode) 
        { 
            lastFunctionFiredIndex = -1;
            ScoreController.correctActions = 0;
            ScoreController.incorrectActions = 0;
            gameController.GetComponent<TestTimerController>().StartTimer();
            EventHandler.OnTimeout += HandleTimeout;
        }

        if (lastFunctionFiredIndex == -1) InitSmallCuts();
        else StartFromStep();
    }

    private void StartFromStep()
    {
        SelectEnterEventArgs args = new();
        actionList[lastFunctionFiredIndex].Invoke(args);

        int crossOffIndex = 0;
        if (lastFunctionFiredIndex == 0) crossOffIndex = 1;
        if (lastFunctionFiredIndex == 1) crossOffIndex = 3;
        if (lastFunctionFiredIndex == 2) crossOffIndex = 4;


        for (int i = 0; i < crossOffIndex; i++)
        {
            stepsTextMeshPros[i].color = Color.green;
            stepsTextMeshPros[i].text = "<s>" + stepsTextMeshPros[i].text + "</s>";

        }
    }

    private void HandleTimeout()
    {
        countdownText.transform.parent.gameObject.SetActive(false);
        timeoutUI.SetActive(true);
        testingComplete = true;
    }

    private void InitSmallCuts()
    {
        EventHandler.OnSmallCutsClicked -= InitSmallCuts;
        sinkHandle1.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerSinkInteraction);
        if (!isTestMode) sinkHandle1.transform.GetChild(0).gameObject.SetActive(true);
        sinkHandle1.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerIncorrectAction);
        sinkHandle1.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerCorrectAction);

        sinkHandle2.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerSinkInteraction);
        sinkHandle2.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerIncorrectAction);
        sinkHandle2.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerCorrectAction);
    }

    private void TriggerSinkInteraction(SelectEnterEventArgs args)
    {
        // Clean Up Sink Handle
        EventHandler.TriggerSinkInteraction();
        sinkHandle1.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        sinkHandle1.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerSinkInteraction);

        sinkHandle2.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        sinkHandle2.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerSinkInteraction);

        // Set Up Sink Door
        doorHandle.GetComponent<XRSimpleInteractable>().selectEntered.RemoveAllListeners();
        doorHandle.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerCorrectAction);
        doorHandle.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerSinkDoorInteraction);

        if (!isTestMode) 
        {
            sinkHandle1.transform.GetChild(0).gameObject.SetActive(false);
            doorHandle.transform.GetChild(0).gameObject.SetActive(true);
            lastFunctionFiredIndex = 0;
            WriteSaveData();
            stepsTextMeshPros[0].color = Color.green;
            stepsTextMeshPros[0].text = "<s>" + stepsTextMeshPros[0].text + "</s>";
        }

    }

    private void TriggerSinkDoorInteraction(SelectEnterEventArgs args)
    {
        // Clean Up Sink Door
        //EventHandler.TriggerSinkDoorOpened();
        doorHandle.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        doorHandle.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        doorHandle.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerSinkDoorInteraction);
        if (!isTestMode) doorHandle.transform.GetChild(0).gameObject.SetActive(false);

        sinkHandle1.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        sinkHandle2.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerIncorrectAction);

        // Set Up Bandage
        bandage.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerCorrectAction);
        bandage.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerIncorrectAction);
        bandage.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerBandageGrabbed);
        if (!isTestMode)
        {
            bandage.transform.GetChild(0).gameObject.SetActive(true);
            WriteSaveData();
        }
    }

    private void TriggerBandageGrabbed(SelectEnterEventArgs args)
    {
        // Clean Up Bandage
        EventHandler.TriggerBandageGrabbed();
        bandage.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        bandage.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerBandageGrabbed);
        if (!isTestMode) bandage.transform.GetChild(0).gameObject.SetActive(false);

        // Set Up Pressure Socket - ! WILL NEED TO CHANGE TO SOCKET INTERACTABLE !
        pressureSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerCorrectAction);
        pressureSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerPressureInteraction);
        if (!isTestMode)
        {
            pressureSocket.transform.GetChild(0).gameObject.SetActive(true);

            WriteSaveData();
            stepsTextMeshPros[1].color = Color.green;
            stepsTextMeshPros[1].text = "<s>" + stepsTextMeshPros[1].text + "</s>";

        }
    }

    private void TriggerPressureInteraction(SelectEnterEventArgs args)
    {
        // Clean Up Pressure Socket - ! WILL NEED TO CHANGE TO SOCKET INTERACTABLE !
        EventHandler.TriggerPressureInteraction();
        pressureSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerCorrectAction);
        pressureSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerPressureInteraction);
        pressureSocket.GetComponent<XRSocketInteractor>().selectExited.AddListener((SelectExitEventArgs args) => { 
            EventHandler.TriggerIncorrectAction();
            bandage.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        });

        // Set Up Antibiotic Bottle
        antibioticBottle.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerCorrectAction);
        antibioticBottle.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerIncorrectAction);
        antibioticBottle.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerAntibioticGrabbed);

        if (!isTestMode)
        {
            pressureSocket.transform.GetChild(0).gameObject.SetActive(false);
            antibioticBottle.transform.GetChild(0).gameObject.SetActive(true);
            lastFunctionFiredIndex = 1;

            WriteSaveData();
            stepsTextMeshPros[2].color = Color.green;
            stepsTextMeshPros[2].text = "<s>" + stepsTextMeshPros[2].text + "</s>";

        }

    }

    private void TriggerAntibioticGrabbed(SelectEnterEventArgs args)
    {
        // Clean Up Antibiotic Bottle
        if (!isTestMode) EventHandler.TriggerAntibioticGrabbed();
        antibioticBottle.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        antibioticBottle.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerAntibioticGrabbed);
        if (!isTestMode) antibioticBottle.transform.GetChild(0).gameObject.SetActive(false);

        // Set Up Antibiotic Socket - ! WILL NEED TO CHANGE TO SOCKET INTERACTABLE !
        antibioticSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerCorrectAction);
        antibioticSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerAntibioticAppliedInteraction);
        antibioticSocket.GetComponent<XRSocketInteractor>().selectExited.AddListener((SelectExitEventArgs args) => { 
            EventHandler.TriggerIncorrectAction();
            antibioticBottle.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        });
        if (!isTestMode)
        {
            antibioticSocket.transform.GetChild(0).gameObject.SetActive(true);

            WriteSaveData();
        }
    }

    private void TriggerAntibioticAppliedInteraction(SelectEnterEventArgs args)
    {
        // Clean Up Antibiotic Socket - ! WILL NEED TO CHANGE TO SOCKET INTERACTABLE !
        EventHandler.TriggerAntibioticApplied();
        antibioticSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerCorrectAction);
        antibioticSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerAntibioticAppliedInteraction);

        // Set Up Bandage
        newBandage.SetActive(true);
        newBandage.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerCorrectAction);
        newBandage.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerIncorrectAction);
        newBandage.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerNewBandageGrabbed);

        if (!isTestMode)
        {
            antibioticSocket.transform.GetChild(0).gameObject.SetActive(false);
            newBandage.transform.GetChild(0).gameObject.SetActive(true);

            lastFunctionFiredIndex = 2;
            WriteSaveData();
            stepsTextMeshPros[3].color = Color.green;
            stepsTextMeshPros[3].text = "<s>" + stepsTextMeshPros[3].text + "</s>";

        }

    }

    private void TriggerNewBandageGrabbed(SelectEnterEventArgs args)
    {
        // Clean Up New Bandage
        //EventHandler.TriggerNewBandageGrabbed();
        newBandage.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        newBandage.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerNewBandageGrabbed);
        //newBandage.GetComponent<XRGrabInteractable>().interactionLayers = InteractionLayerMask.GetMask("New Bandage");
        if (!isTestMode) newBandage.transform.GetChild(0).gameObject.SetActive(false);

        // Set Up New Bandage Socket - ! WILL NEED TO CHANGE TO SOCKET INTERACTABLE !
        newBandageSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerCorrectAction);
        newBandageSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerNewBandageApplied);
        newBandageSocket.GetComponent<XRSocketInteractor>().selectExited.AddListener((SelectExitEventArgs args) => { 
            EventHandler.TriggerIncorrectAction();
            newBandage.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        });
        if (!isTestMode)
        {
            newBandageSocket.transform.GetChild(0).gameObject.SetActive(true);

            WriteSaveData();
        }
    }

    private void TriggerNewBandageApplied(SelectEnterEventArgs args)
    {
        // Clean Up New Bandage Socket - ! WILL NEED TO CHANGE TO SOCKET INTERACTABLE !
        EventHandler.TriggerNewBandageApplied();
        newBandageSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerCorrectAction);
        newBandageSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerNewBandageGrabbed);
        if (!isTestMode) newBandageSocket.transform.GetChild(0).gameObject.SetActive(false);

        lastFunctionFiredIndex = -1;
        if (!isTestMode) trainingComplete = true;
        else {
            gameController.GetComponent<TestTimerController>().StopTimer();
            testingComplete = true; 
        }

        WriteSaveData();
        stepsTextMeshPros[4].color = Color.green;
        stepsTextMeshPros[4].text = "<s>" + stepsTextMeshPros[4].text + "</s>";


    }
}
