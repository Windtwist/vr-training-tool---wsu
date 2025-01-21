using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public static int correctActions = 0;
    public static int incorrectActions = 0;

    public delegate void SmallCutsClicked();
    public delegate void LargeWoundsClicked();
    public delegate void CorrectAction();
    public delegate void IncorrectAction();

    public delegate void SinkInteraction();
    public delegate void SinkDoorOpened();
    public delegate void BandageGrabbed();
    public delegate void PressureInteraction();
    public delegate void AntibioticGrabbed();
    public delegate void AntibioticApplied();
    public delegate void NewBandageGrabbed();
    public delegate void NewBandageApplied();

    public delegate void PhoneInteraction();
    public delegate void RemoveDebrisInteraction();
    public delegate void PressureLargeWoundsInteraction();
    public delegate void LieDownInteraction();
    public delegate void TourniquetGrabbed();
    public delegate void TourniquetApplied();

    public delegate void Timeout();

    public static event SmallCutsClicked OnSmallCutsClicked;
    public static event LargeWoundsClicked OnLargeWoundsClicked;
    public static event CorrectAction OnCorrectAction;
    public static event IncorrectAction OnIncorrectAction;

    public static event SinkInteraction OnSinkInteraction;
    public static event SinkDoorOpened OnSinkDoorOpened;
    public static event BandageGrabbed OnBandageGrabbed;
    public static event PressureInteraction OnPressureInteraction;
    public static event AntibioticGrabbed OnAntibioticGrabbed;
    public static event AntibioticApplied OnAntibioticApplied;
    public static event NewBandageGrabbed OnNewBandageGrabbed;
    public static event NewBandageApplied OnNewBandageApplied;

    public static event PhoneInteraction OnPhoneInteraction;
    public static event RemoveDebrisInteraction OnRemoveDebrisInteraction;
    public static event PressureLargeWoundsInteraction OnPressureLargeWoundsInteraction;
    public static event LieDownInteraction OnLieDownInteraction;
    public static event TourniquetGrabbed OnTourniquetGrabbed;
    public static event TourniquetApplied OnTourniquetApplied;

    public static event Timeout OnTimeout;

    private void Start()
    {
        OnIncorrectAction += IncrementIncorrect;
        OnCorrectAction += IncrementCorrect;
    }

    public static void TriggerTimeout()
    {
        OnTimeout?.Invoke();
    }

    private void IncrementCorrect()
    {
        correctActions++;
    }

    private void IncrementIncorrect()
    {
        incorrectActions++;
    }

    public void TriggerSmallCutsClicked()
    {
        OnSmallCutsClicked?.Invoke();
    }

    public void TriggerLargeWoundsClicked()
    {
        OnLargeWoundsClicked?.Invoke();
    }

    public static void TriggerCorrectAction()
    {
        Debug.Log("Correct Action");
        OnCorrectAction?.Invoke();  
    }

    public static void TriggerIncorrectAction()
    {
        Debug.Log("Incorrect Action");
        OnIncorrectAction?.Invoke();
    }

    public static void TriggerSinkInteraction()
    {
        OnSinkInteraction?.Invoke();
    }


    public static void TriggerSinkDoorOpened()
    {
        OnSinkDoorOpened?.Invoke();
    }

    public static void TriggerBandageGrabbed()
    {
        OnBandageGrabbed?.Invoke();
    }

    public static void TriggerPressureInteraction()
    {
        OnPressureInteraction?.Invoke();
    }

    public static void TriggerAntibioticGrabbed()
    {
        OnAntibioticGrabbed?.Invoke();
    }

    public static void TriggerAntibioticApplied()
    {
        OnAntibioticApplied?.Invoke();
    }

    public static void TriggerNewBandageGrabbed()
    {
        OnNewBandageGrabbed?.Invoke();
    }

    public static void TriggerNewBandageApplied() 
    { 
        OnNewBandageApplied?.Invoke(); 
    }

    public static void TriggerPhoneInteraction()
    {
        OnPhoneInteraction?.Invoke();
    }

    public static void TriggerRemoveDebrisInteraction()
    {
        OnRemoveDebrisInteraction?.Invoke();
    }

    public static void TriggerPressureLargeWounds()
    {
        OnPressureLargeWoundsInteraction?.Invoke();
    }

    public static void TriggerLieDownInteraction()
    {
        OnLieDownInteraction?.Invoke();
    }

    public static void TriggerTourniquetGrabbed()
    {
        OnTourniquetGrabbed?.Invoke();
    }

    public static void TriggerTourniquetApplied()
    {
        OnTourniquetApplied?.Invoke();
    }
}
