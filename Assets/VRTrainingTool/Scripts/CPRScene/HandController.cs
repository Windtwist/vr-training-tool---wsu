using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandController : MonoBehaviour
{
    // class for controlling haptic feedback based on correct or incorrect user actions
    // in scenario
    [Range(0, 1)]
    private float intensity;
    private float duration;
    [SerializeField] private XRBaseController controller;


    private void Start()
    {
        CprEventHandler.onCorrectAction += OnCorrectAction;
        CprEventHandler.onIncorrectAction += OnIncorrectAction;
        CprEventHandler.onCprProgressed += TriggerCprHaptics;
    }

    private void OnCorrectAction()
    {
        TriggerHaptics(true);
    }

    private void OnIncorrectAction()
    {
        TriggerHaptics(false);
    }

    // correct actions are indicated with short vibration
    // incorrect actions have stronger intensity and duration
    // to make two distinguishable
    private void TriggerHaptics(bool correct)
    {
        if (correct)
        {
            intensity = 0.3f;
            duration = 0.2f;

            controller.SendHapticImpulse(intensity, duration);
        }
        else
        {
            intensity = 0.5f;
            duration = 0.7f;
        }

        if (intensity > 0)
        {
            controller.SendHapticImpulse(intensity, duration);
        }
    }

    private void TriggerCprHaptics()
    {
        intensity = 0.7f;
        duration = 0.1f;

        if (intensity > 0)
        {
            controller.SendHapticImpulse(intensity, duration);
        }
    }

    private void OnDestroy()
    {
        CprEventHandler.onCorrectAction -= OnCorrectAction;
        CprEventHandler.onIncorrectAction -= OnIncorrectAction;
        CprEventHandler.onCprProgressed -= TriggerCprHaptics;
    }
}