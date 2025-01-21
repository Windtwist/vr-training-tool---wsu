using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NewBandageAppliedController : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    private bool isLargeWounds = false;

    // Start is called before the first frame update
    void Start()
    {
        EventHandler.OnNewBandageApplied += HideUI;
        EventHandler.OnNewBandageGrabbed += ShowUI;
        EventHandler.OnLargeWoundsClicked += SetLargeWounds;
    }

    private void HideUI()
    {
        canvas.SetActive(false);
        EventHandler.TriggerCorrectAction();
        EventHandler.OnNewBandageApplied -= HideUI;
        EventHandler.OnNewBandageGrabbed -= ShowUI;

        if (isLargeWounds) 
        {
            canvas.GetComponentInParent<XRSimpleInteractable>().selectEntered.RemoveAllListeners();
            canvas.GetComponentInParent<XRSimpleInteractable>().selectEntered.AddListener(TriggerTourniquetApplied);
        }
    }

    private void ShowUI()
    {
        canvas.SetActive(true);
    }

    private void SetLargeWounds()
    {
        isLargeWounds = true;
    }

    private void TriggerTourniquetApplied(SelectEnterEventArgs args)
    {
        EventHandler.TriggerTourniquetApplied();
    }
}
