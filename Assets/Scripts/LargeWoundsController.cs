using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;

public class LargeWoundsController : MonoBehaviour
{
    [SerializeField] GameObject gameController;

    [SerializeField] GameObject phone;
    [SerializeField] GameObject debrisInteractable;
    [SerializeField] GameObject sinkDoorHandle;
    [SerializeField] GameObject bandage;
    [SerializeField] GameObject pressureSocket;
    [SerializeField] GameObject lieDownInteractable;
    [SerializeField] GameObject newBandage;
    [SerializeField] GameObject newBandageSocket;
    [SerializeField] GameObject tourniquet;
    [SerializeField] GameObject tourniquetSocket;
    [SerializeField] GameObject sinkHandle1;
    [SerializeField] GameObject sinkHandle2;

    [SerializeField] GameObject testButton;

    [SerializeField] List<TextMeshProUGUI> stepsTextMeshPros;

    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] GameObject timoutUI;


    private bool isTestMode = false;
    private int lastFunctionFiredIndex = -1;
    private string path;

    public static bool trainingComplete;
    public static bool testingComplete;

    private List<Action<SelectEnterEventArgs>> actionList = new List<Action<SelectEnterEventArgs>>();


    private void Start()
    {
        path = Application.persistentDataPath + "/json saves/LargeWounds.json";
        actionList.Add(TriggerPhoneGrabbed);
        actionList.Add(TriggerDebrisInteraction);
        actionList.Add(TriggerPessureInteraction);
        actionList.Add(TriggerLieDownInteraction);
        actionList.Add(TriggerNewBandageApplied);
        actionList.Add(TriggerTourniquetApplied);

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
            testButton.GetComponent<Button>().interactable = true;
            testButton.GetComponent<XRSimpleInteractable>().enabled = true;
        }

        EventHandler.OnLargeWoundsClicked += SetIncorrectListeners;
    }

    private void OnDestroy()
    {
        EventHandler.OnLargeWoundsClicked -= SetIncorrectListeners;
        EventHandler.OnTimeout -= HandleTimeout;

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

        File.WriteAllText(path, JsonUtility.ToJson(save));
    }

    public void SetIsTest(bool isTest)
    {
        isTestMode = isTest;
    }

    private void TriggerCorrectAction(SelectEnterEventArgs args)
    {
        EventHandler.TriggerCorrectAction();
    }

    private void TriggerIncorrectAction(SelectEnterEventArgs args)
    {
        EventHandler.TriggerIncorrectAction();
    }

    private void SetIncorrectListeners()
    {
        debrisInteractable.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        sinkDoorHandle.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        bandage.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        pressureSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerIncorrectAction);
        lieDownInteractable.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        newBandage.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        newBandageSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerIncorrectAction);
        tourniquet.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        tourniquetSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerIncorrectAction);
        sinkHandle1.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        sinkHandle2.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerIncorrectAction);

        if (isTestMode)
        {
            lastFunctionFiredIndex = -1;
            ScoreController.correctActions = 0;
            ScoreController.incorrectActions = 0;
            gameController.GetComponent<TestTimerController>().StartTimer();
            EventHandler.OnTimeout += HandleTimeout;
        }

        if (lastFunctionFiredIndex > 2) { 
            lieDownInteractable.GetComponent<ActivateAnimation>().ObjectHover();
        }

        if (lastFunctionFiredIndex == -1) InitLargeWounds();
        else StartFromStep();
    }

    private void StartFromStep()
    {
        SelectEnterEventArgs args = new();
        actionList[lastFunctionFiredIndex].Invoke(args);

        int crossOffIndex = 0;
        if (lastFunctionFiredIndex == 0) crossOffIndex = 1;
        else if (lastFunctionFiredIndex == 1) crossOffIndex = 2;
        else if (lastFunctionFiredIndex == 2) crossOffIndex = 4;
        else crossOffIndex = lastFunctionFiredIndex + 2;

        for (int i = 0; i < crossOffIndex; i++)
        {
            stepsTextMeshPros[i].color = Color.green;             
            stepsTextMeshPros[i].text = "<s>" + stepsTextMeshPros[i].text + "</s>";
        }
    }

    private void HandleTimeout()
    {
        countdownText.transform.parent.gameObject.SetActive(false);
        timoutUI.SetActive(true);
        testingComplete = true;
    }

    private void InitLargeWounds()
    {
        phone.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerPhoneGrabbed);
        if (!isTestMode) phone.transform.GetChild(0).gameObject.SetActive(true);
        phone.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerIncorrectAction);
        phone.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerCorrectAction);
    }

    private void TriggerPhoneGrabbed(SelectEnterEventArgs args)
    {
        EventHandler.TriggerPhoneInteraction();
        phone.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        phone.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        phone.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerPhoneGrabbed);

        debrisInteractable.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerCorrectAction);
        debrisInteractable.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerIncorrectAction);
        debrisInteractable.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerDebrisInteraction);

        if (!isTestMode)
        {
            phone.transform.GetChild(0).gameObject.SetActive(false);
            debrisInteractable.transform.GetChild(0).gameObject.SetActive(true);
            lastFunctionFiredIndex = 0;
            WriteSaveData();
            stepsTextMeshPros[0].color = Color.green;             
            stepsTextMeshPros[0].text = "<s>" + stepsTextMeshPros[0].text + "</s>";
        }


    }

    private void TriggerDebrisInteraction(SelectEnterEventArgs args)
    {
        EventHandler.TriggerRemoveDebrisInteraction();
        debrisInteractable.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        debrisInteractable.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        debrisInteractable.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerDebrisInteraction);

        sinkDoorHandle.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerCorrectAction);
        sinkDoorHandle.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerIncorrectAction);
        sinkDoorHandle.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerSinkDoorInteraction);
        
        if (!isTestMode)
        {
            debrisInteractable.transform.GetChild(0).gameObject.SetActive(false);
            sinkDoorHandle.transform.GetChild(0).gameObject.SetActive(true);
            lastFunctionFiredIndex = 1;
            WriteSaveData();
            stepsTextMeshPros[1].color = Color.green;             
            stepsTextMeshPros[1].text = "<s>" + stepsTextMeshPros[1].text + "</s>";
        }


    }

    private void TriggerSinkDoorInteraction(SelectEnterEventArgs args)
    {
        //EventHandler.TriggerSinkDoorOpened();
        sinkDoorHandle.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        sinkDoorHandle.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        sinkDoorHandle.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerSinkDoorInteraction);
        if (!isTestMode) sinkDoorHandle.transform.GetChild(0).gameObject.SetActive(false);

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
        EventHandler.TriggerBandageGrabbed();
        bandage.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        bandage.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerBandageGrabbed);
        if (!isTestMode) bandage.transform.GetChild(0).gameObject.SetActive(false);

        // !!
        pressureSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerCorrectAction);
        pressureSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerIncorrectAction);
        pressureSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerPessureInteraction);
        pressureSocket.GetComponent<XRSocketInteractor>().selectExited.AddListener((SelectExitEventArgs args) =>
        {
            //EventHandler.TriggerIncorrectAction();
            bandage.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        });
        if (!isTestMode)
        {
            pressureSocket.transform.GetChild(0).gameObject.SetActive(true);

            WriteSaveData();
            stepsTextMeshPros[2].color = Color.green;             
            stepsTextMeshPros[2].text = "<s>" + stepsTextMeshPros[2].text + "</s>";
        }

    }

    private void TriggerPessureInteraction(SelectEnterEventArgs args)
    {
        // !!
        EventHandler.TriggerPressureLargeWounds();
        pressureSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerCorrectAction);
        pressureSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerPessureInteraction);

        lieDownInteractable.SetActive(true);
        lieDownInteractable.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerCorrectAction);
        lieDownInteractable.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerIncorrectAction);
        lieDownInteractable.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerLieDownInteraction);

        if (!isTestMode)
        {
            pressureSocket.transform.GetChild(0).gameObject.SetActive(false);
            lieDownInteractable.transform.GetChild(0).gameObject.SetActive(true);
            lastFunctionFiredIndex = 2;
            WriteSaveData();
            stepsTextMeshPros[3].color = Color.green;             
            stepsTextMeshPros[3].text = "<s>" + stepsTextMeshPros[3].text + "</s>";
        }


    }

    private void TriggerLieDownInteraction(SelectEnterEventArgs args)
    {
        EventHandler.TriggerLieDownInteraction();
        lieDownInteractable.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        lieDownInteractable.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        lieDownInteractable.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerLieDownInteraction);
        lieDownInteractable.SetActive(false);
        bandage.SetActive(false);

        newBandage.SetActive(true);
        newBandage.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerCorrectAction);
        newBandage.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerIncorrectAction);
        newBandage.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerNewBandageGrabbed);

        if (!isTestMode)
        {
            lieDownInteractable.transform.GetChild(0).gameObject.SetActive(false);
            newBandage.transform.GetChild(0).gameObject.SetActive(true);
            lastFunctionFiredIndex = 3;
            WriteSaveData();
            stepsTextMeshPros[4].color = Color.green;             
            stepsTextMeshPros[4].text = "<s>" + stepsTextMeshPros[4].text + "</s>";
        }


    }

    private void TriggerNewBandageGrabbed(SelectEnterEventArgs args)
    {
        //EventHandler.TriggerNewBandageGrabbed();
        newBandage.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        newBandage.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerNewBandageGrabbed);
        newBandage.GetComponent<XRGrabInteractable>().interactionLayers = InteractionLayerMask.GetMask("New Bandage");
        if (!isTestMode) newBandage.transform.GetChild(0).gameObject.SetActive(false);

        newBandageSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerCorrectAction);
        newBandageSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerIncorrectAction);
        newBandageSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerNewBandageApplied);
        newBandageSocket.GetComponent<XRSocketInteractor>().selectExited.AddListener((SelectExitEventArgs args) =>
        {
            newBandage.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
            EventHandler.TriggerIncorrectAction();
        });
        if (!isTestMode)
        {
            newBandageSocket.transform.GetChild(0).gameObject.SetActive(true);

            WriteSaveData();
        }
    }

    private void TriggerNewBandageApplied(SelectEnterEventArgs args)
    {
        EventHandler.TriggerNewBandageApplied();
        newBandageSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerCorrectAction);
        newBandageSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerNewBandageApplied);

        tourniquet.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerCorrectAction);
        tourniquet.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerIncorrectAction);
        tourniquet.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerTourniquetGrabbed);

        if (!isTestMode)
        {
            newBandageSocket.transform.GetChild(0).gameObject.SetActive(false);
            tourniquet.transform.GetChild(0).gameObject.SetActive(true);
            lastFunctionFiredIndex = 4;
            WriteSaveData();
            stepsTextMeshPros[5].color = Color.green;             
            stepsTextMeshPros[5].text = "<s>" + stepsTextMeshPros[5].text + "</s>";
        }


    }

    private void TriggerTourniquetGrabbed(SelectEnterEventArgs args)
    {
        //EventHandler.TriggerTourniquetGrabbed();
        tourniquet.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        tourniquet.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(TriggerTourniquetGrabbed);
        if (!isTestMode) tourniquet.transform.GetChild(0).gameObject.SetActive(false);

        tourniquetSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerCorrectAction);
        tourniquetSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerIncorrectAction);
        tourniquetSocket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(TriggerTourniquetApplied);
        tourniquetSocket.GetComponent<XRSocketInteractor>().selectExited.AddListener((SelectExitEventArgs args) =>
        {
            tourniquet.GetComponent<XRGrabInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
            EventHandler.TriggerIncorrectAction();
        });
        if (!isTestMode)
        {
            tourniquetSocket.transform.GetChild(0).gameObject.SetActive(true);

            WriteSaveData();
        }
    }

    private void TriggerTourniquetApplied(SelectEnterEventArgs args)
    {
        EventHandler.TriggerTourniquetApplied();
        tourniquetSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerCorrectAction);
        tourniquetSocket.GetComponent<XRSocketInteractor>().selectEntered.RemoveListener(TriggerTourniquetApplied);

        sinkHandle1.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerCorrectAction);
        sinkHandle1.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerIncorrectAction);
        sinkHandle1.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerSinkInteraction);

        sinkHandle2.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerCorrectAction);
        sinkHandle2.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerIncorrectAction);
        sinkHandle2.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerSinkInteraction);

        if (!isTestMode)
        {
            tourniquetSocket.transform.GetChild(0).gameObject.SetActive(false);
            sinkHandle1.transform.GetChild(0).gameObject.SetActive(true);
            lastFunctionFiredIndex = 5;
            WriteSaveData();
            stepsTextMeshPros[6].color = Color.green;             
            stepsTextMeshPros[6].text = "<s>" + stepsTextMeshPros[6].text + "</s>";
        }


    }

    private void TriggerSinkInteraction(SelectEnterEventArgs args)
    {
        EventHandler.TriggerSinkInteraction();
        sinkHandle1.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        sinkHandle1.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        sinkHandle1.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerTourniquetApplied);
        if (!isTestMode) sinkHandle1.transform.GetChild(0).gameObject.SetActive(false);

        sinkHandle2.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(TriggerIncorrectAction);
        sinkHandle2.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerCorrectAction);
        sinkHandle2.GetComponent<XRSimpleInteractable>().selectEntered.RemoveListener(TriggerTourniquetApplied);

        lastFunctionFiredIndex = -1;
        if (isTestMode) 
        { 
            testingComplete = true;
            gameController.GetComponent<TestTimerController>().StopTimer();
        }
        else trainingComplete = true;

        WriteSaveData();
        stepsTextMeshPros[7].color = Color.green;             
        stepsTextMeshPros[7].text = "<s>" + stepsTextMeshPros[7].text + "</s>";

    }

}
