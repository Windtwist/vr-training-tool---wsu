using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeimlichEvents : MonoBehaviour
{



    public int passCount = 0;
    public int failCount = 0;
    public static bool trainingCompleted = false;
    public static bool testingCompleted = false;
    public static bool trainingResumed = false;


    public delegate void CorrectAction();
    public delegate void IncorrectAction();
    public delegate void TrainingMode();
    public delegate void TestingMode();
    public delegate void CheckPatient();
    public delegate void BackBlows();
    public delegate void HandsInteraction(); //camera switch
    public delegate void Heimlich();
    public delegate void PhoneInteraction();
    public delegate void Auto_move();
    public delegate void SaveInteraction();
    public delegate void SaveTest();

    public static event CorrectAction onCorrectAction;
    public static event IncorrectAction onIncorrectAction;
    public static event TrainingMode onTrainingModeSelected;
    public static event TestingMode onTestingModeSelected;
    public static event PhoneInteraction onPhoneSelected;
    public static event Auto_move onAuto_move;
    public static event CheckPatient onPatientMoved;
    public static event HandsInteraction onHandsPlaced;
    public static event Heimlich onHeimlichStarted;
    public static event BackBlows onBackBlowsStarted;
    public static event TrainingMode onTrainingCompleted;
    public static event TestingMode onTestingCompleted;
    public static event TestingMode onTestingTimeout;
    public static event TestingMode onTestingCountdown;
    public static event TrainingMode onTrainingResumed;
    public static event SaveInteraction onProgressSave;
    public static event SaveTest Step_save;
    private void Start()//called on scene start
    {
        onIncorrectAction += IncrementIncorrect;
        onCorrectAction += IncrementCorrect;
    }

    private void IncrementCorrect()//increment correct actions - so next step taken is next action then
    {
        passCount++;
    }

    private void IncrementIncorrect() //error count
    {
        failCount++;
    }

    public static void TriggerCorrectAction()//used for animations 
    {
        onCorrectAction?.Invoke();
    }
    public static void TriggerSaving()//saves to json
    {
        onProgressSave?.Invoke();
    }
    public static void TriggerTest()//triggers testing mode
    {
        Step_save?.Invoke();
    }
    public static void TriggerIncorrectAction()//triggers testing mdoe incorrect audio
    {
        onIncorrectAction?.Invoke();
    }

    public static void TriggerPhoneInteraction()
    {
        onPhoneSelected?.Invoke();
    }

    public static void Auto_moved()//makes sure the npc is in correct placement after resuming training
    {
        onAuto_move?.Invoke();
    }

    public static void TriggerPatientInteraction()//step 1 trigger
    {
        onPatientMoved?.Invoke();
    }

    public static void TriggerBlowsInteraction()//step 2 trigger
    {
        onBackBlowsStarted?.Invoke();
    }


    public static void TriggerHandsInteraction()//step 3 trigger
    {
        onHandsPlaced?.Invoke();
    }

    public static void TriggerHeimlichInteraction()//step 4 trigger
    {
        onHeimlichStarted?.Invoke();
    }

    public static void TriggerTrainingMode()//training mode trigger - start
    {
        onTrainingModeSelected?.Invoke();
    }

    public static void TriggerTestingMode()//testing mode trigger - start
    {
        onTestingModeSelected?.Invoke();
    }
    
    public static void TriggerTestingCountdown()//testing mode countdown start
    {
        onTestingCountdown?.Invoke();
    }

    public static void TriggerTestingTimeout()//testing mode coundtdown reaches 0
    {
        onTestingTimeout?.Invoke();
    }

    public static void TriggerTrainingCompletion()//training mode completion trigger
    {
        trainingCompleted = true;
        onTrainingCompleted?.Invoke();
    }
    public static void ResumeTraining()//resume training trigger
    {
        trainingResumed = true;
        onTrainingResumed?.Invoke();

    }
    public static void TriggerTestingCompletion()//testing mode completion trigger
    {
        testingCompleted = true;
        onTestingCompleted?.Invoke();
    }
}
