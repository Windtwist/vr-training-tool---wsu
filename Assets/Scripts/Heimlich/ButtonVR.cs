using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonVR : MonoBehaviour
{
    
    public XRBaseController controllerL;
    public XRBaseController controllerR;
    public GameObject xr;
    public XRSimpleInteractable buttonInteractable;
    public TextMeshProUGUI pressCountText;

    private int controllersOnButton = 0;
    public Camera main;
    public Camera side;
    public int pressCount = 0;

    [Range(0, 1)]
    public float hapticsIntensity;
    public float hapticsDuration;

    private void Awake()
    {
        main.enabled = false;
        side.enabled = true; //new camera view
    }

public void OnSelectHover()
    {
        // Increment the count of controllers on the button
        controllersOnButton++;

        // Check if both controllers are on the button
        if (controllersOnButton == 2)
        
            controllerL.transform.localPosition = new Vector3(-0.530510783f, -0.434479624f, 0.206294686f);
            controllerR.transform.localPosition = new Vector3(0.104779407f, -0.424406886f, 0.336293727f);
            controllerL.transform.localRotation = new Quaternion(0.183311298f, -0.0707063153f, -0.426107556f, 0.883079827f);
            controllerR.transform.localRotation = new Quaternion(0.0738871321f, -0.129809186f, 0.47210443f, 0.868796706f);
            //moves controller position so it can be seen from new angle

            pressCount++;
            
            pressCountText.text = "Hands on:  " + pressCount.ToString();
            if (hapticsIntensity > 0)
            {
                controllerL.SendHapticImpulse(hapticsIntensity, hapticsDuration);
                controllerR.SendHapticImpulse(hapticsIntensity, hapticsDuration);

            }
            // Trigger the next event after count reaches 5
            if (pressCount == 5)
            {
                side.enabled = false;
                main.enabled = true;
                HeimlichEvents.TriggerHeimlichInteraction();

            }

        }
    

    public void OnHoverExited()
    {
        // Decrement the count of controllers on the button after user mvoes one hand off
        controllersOnButton--;
        pressCountText.text = "hands off";

    }
}