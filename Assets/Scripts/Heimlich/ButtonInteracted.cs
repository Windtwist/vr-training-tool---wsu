using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonInteracted : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    public int pressCount = 0;
    public TextMeshProUGUI pressCountText;
    public int targetPressCount = 5;

    public XRBaseController controllerL;
    public XRBaseController controllerR;

    public GameObject origin;

    [Range(0, 1)]
    public float hapticsIntensity;
    public float hapticsDuration;

    private bool isPressed = false;

    private void Awake()
    {
        animator = GetComponent<Animator>(); //only when this particular object gets activated, will we be able to get this component
    }

    public void TriggerCorrectActionAnim()
    {
        animator.SetTrigger("BackBlow");
    }


    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider is a hand controller
        if (other.CompareTag("hand"))
        {
            // Check if the target press count has been reached
            if (pressCount >= targetPressCount)
            {
                FinishBlows();
                if(pressCount == 5)
                origin.transform.localPosition = new Vector3(6.6600008f, 3.25f, -9.77900036f);//moves xr rig for better placement view 
                return;
                
            }
            // Check if the button is not already pressed
            if (!isPressed && pressCount <5)
            {
                // Increment the press count and update the UI text
                pressCount++;
                pressCountText.text =  pressCount.ToString();
                if (hapticsIntensity > 0)
                {
                    controllerL.SendHapticImpulse(hapticsIntensity, hapticsDuration);
                    controllerR.SendHapticImpulse(hapticsIntensity, hapticsDuration);

                }

                // Set the button as pressed
                isPressed = true;
            }
        }
    }

    private void FinishBlows()
    {
        HeimlichEvents.TriggerHandsInteraction();
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the collider is a hand controller
        if (other.CompareTag("hand"))
        {
            // Set the button as not pressed
            isPressed = false;
        }
    }
}
