using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandAttachTransformer : MonoBehaviour
{
    public GameObject inHand;
    public GameObject onBase;

    private XRBaseInteractable grabComponent;

    private void Awake()
    {
        grabComponent = this.gameObject.GetComponent<XRGrabInteractable>();
        grabComponent.selectEntered.AddListener(ChangeHandAttachPoint);
        grabComponent.selectExited.AddListener(ChangeBaseAttachPoint);
    }

    private void ChangeHandAttachPoint(SelectEnterEventArgs eventArgs)
    {
        bool isDirect = eventArgs.interactorObject is XRDirectInteractor;

        if (isDirect)
        {
            if (TryGetComponent(out XRGrabInteractable interactable))
            {
                interactable.attachTransform = inHand.transform;
            }
        }
    }

    private void ChangeBaseAttachPoint(SelectExitEventArgs eventArgs)
    {
        if (TryGetComponent(out XRGrabInteractable interactable))
        {
            interactable.attachTransform = onBase.transform;
        }
    }

    private void OnDestroy()
    {
        grabComponent.selectEntered.RemoveAllListeners();
        grabComponent.selectExited.RemoveAllListeners();
    }

}
