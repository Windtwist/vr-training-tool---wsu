using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class CprHaptics : MonoBehaviour
{
    public PressureBar pb;

    public XRBaseInteractable interactable;

    [Range(0, 1)]
    public float hapticsIntensity;
    public float hapticsDuration;

    // Start is called before the first frame update
    void Start()
    {
        //interactable = GetComponent<XRBaseInteractable>();
        //interactable.hoverEntered.AddListener(TriggerHaptic);

        //controller = GameObject.FindObjectOfType(typeof(ActionBasedController));
    }

    //protected override void Awake()
    //{
    //    base.Awake();

    //    interactable.hoverEntered.AddListener(TriggerHaptic);
    //}

    private void OnDestroy()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TriggerHaptic(BaseInteractionEventArgs eventArgs)
    {
        if (eventArgs.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            TriggerHaptic(controllerInteractor.xrController);
        }
    }

    public void TriggerHaptic(XRBaseController controller)
    {
        if (hapticsIntensity > 0 && pb.fillAmount >= 0.90f)
        {
            controller.SendHapticImpulse(hapticsIntensity, hapticsDuration);
        }
    }
}
