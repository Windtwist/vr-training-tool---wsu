using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CprEventHandler : MonoBehaviour
{

    // Main event class for CPR scene. It connects all interactions and interactable objects
    // together for both training and testing scenario
    public static bool trainingCompleted = false;
    public static bool trainingResumed = false;
    public static bool testingCompleted = false;

    public static int controllersActive = 0;

    public delegate void CorrectAction();
    public delegate void IncorrectAction();
    public delegate void TrainingMode();
    public delegate void TestingMode();
    public delegate void PhoneInteraction();
    public delegate void PatientInteraction();
    public delegate void HandsInteraction();
    public delegate void CPRInteraction();
    public delegate void SaveInteraction();
    public delegate void LoadInteraction();

    public static event CorrectAction onCorrectAction;
    public static event IncorrectAction onIncorrectAction;
    public static event TrainingMode onTrainingModeSelected;
    public static event TrainingMode onTrainingResumed;
    public static event TestingMode onTestingModeSelected;
    public static event PhoneInteraction onPhoneSelected;
    public static event PhoneInteraction onPhoneSelectedVerified;
    public static event PatientInteraction onPatientMoved;
    public static event PatientInteraction onPatientMovedVerified;
    public static event HandsInteraction onHandsPlaced;
    public static event HandsInteraction onHandsPlacedVerified;
    public static event CPRInteraction onCPRStarted;
    public static event CPRInteraction onCprProgressed;
    public static event TrainingMode onTrainingCompleted;
    public static event TestingMode onTestingCompleted;
    public static event TestingMode onTestingTimeout;
    public static event TestingMode onTestingCountdown;
    public static event SaveInteraction onProgressSave;
    public static event LoadInteraction onLoadingCompleted;

    public static void TriggerCorrectAction()
    {
        onCorrectAction?.Invoke();
    }

    public static void TriggerIncorrectAction()
    {
        onIncorrectAction?.Invoke();
    }

    public static void TriggerPhoneInteraction()
    {
        onPhoneSelected?.Invoke();
    }

    public static void TriggerVerifiedPhoneInteraction()
    {
        onPhoneSelectedVerified?.Invoke();
    }

    public static void TriggerPatientInteraction()
    {
        onPatientMoved?.Invoke();
    }

    public static void TriggerVefiriedPatientInteraction()
    {
        onPatientMovedVerified?.Invoke();
    }

    public static void TriggerHandsInteraction()
    {
        onHandsPlaced?.Invoke();
    }

    public static void TriggerVerifiedHandsInteraction()
    {
        onHandsPlacedVerified?.Invoke();
    }

    public static void TriggerCPRInteraction()
    {
        onCPRStarted?.Invoke();
    }

    public static void TriggerCprProgress()
    {
        onCprProgressed?.Invoke();
    }

    public static void TriggerTrainingMode()
    {
        onTrainingModeSelected?.Invoke();
    }

    public static void ResumeTraining()
    {
        trainingResumed = true;
        onTrainingResumed?.Invoke();
    }

    public static void TriggerTestingMode()
    {
        onTestingModeSelected?.Invoke();
    }

    public static void TriggerTestingCountdown()
    {
        onTestingCountdown?.Invoke();
    }

    public static void TriggerTestingTimeout()
    {
        onTestingTimeout?.Invoke();
    }

    public static void TriggerTrainingCompletion()
    {
        trainingCompleted = true;
        onTrainingCompleted?.Invoke();
    }

    public static void TriggerTestingCompletion()
    {
        testingCompleted = true;
        onTestingCompleted?.Invoke();
    }

    public static void TriggerSaving()
    {
        onProgressSave?.Invoke();
    }

    public static void NotifyLoadingFinished()
    {
        onLoadingCompleted?.Invoke();
    }
}
