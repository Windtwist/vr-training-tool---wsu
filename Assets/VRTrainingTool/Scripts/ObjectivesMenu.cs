using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class ObjectivesMenu : MonoBehaviour
{
    // public Camera Camera2Follow;
    // public float CameraDistance = 3.0F;
    // public float smoothTime = 0.3F;
    // private Vector3 velocity = Vector3.zero;
    // private Transform target;

    public CprTraining cprTr;

    [SerializeField] private InputActionReference showObjectives = null;
    [SerializeField] private List<TextMeshProUGUI> objectivesList;

    private bool objDisabledOnce = false;


    void Start()
    {
        CprEventHandler.onTrainingModeSelected += SubscribeOnTrainingMode;
        CprEventHandler.onLoadingCompleted += ObjectivesFromResumedTraining;
        CprEventHandler.onPhoneSelectedVerified += MarkPhoneInteraction;
        CprEventHandler.onPatientMovedVerified += MarkPatientInteraction;
        CprEventHandler.onHandsPlacedVerified += MarkHandsInteraction;
        CprEventHandler.onCPRStarted += MarkCprInteraction;

        gameObject.SetActive(false);
    }

    private void Awake()
    {
        // target = Camera2Follow.transform;
    }

    private void OnDestroy()
    {
        showObjectives.action.started -= ToggleObjectivesMenu;
        CprEventHandler.onTrainingModeSelected -= SubscribeOnTrainingMode;
        CprEventHandler.onLoadingCompleted -= ObjectivesFromResumedTraining;
        CprEventHandler.onPhoneSelectedVerified -= MarkPhoneInteraction;
        CprEventHandler.onPatientMovedVerified -= MarkPatientInteraction;
        CprEventHandler.onHandsPlacedVerified -= MarkHandsInteraction;
        CprEventHandler.onCPRStarted -= MarkCprInteraction;
    }

    void Update()
    {
        // Vector3 targetPosition = target.TransformPoint(new Vector3(0, 0, CameraDistance));

        // transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        // //transform.LookAt(transform.position + Camera2Follow.transform.rotation * Vector3.forward, Camera2Follow.transform.rotation * Vector3.up);
        // var lookAtPos = new Vector3(Camera2Follow.transform.position.x/50, transform.position.y, Camera2Follow.transform.position.z * 180);
        // transform.LookAt(lookAtPos);
    }

    private void SubscribeOnTrainingMode()
    {
        showObjectives.action.started += ToggleObjectivesMenu;
        gameObject.SetActive(true);

    }

    private void ToggleObjectivesMenu(InputAction.CallbackContext context)
    {
        bool isActive = !gameObject.activeSelf;
        gameObject.SetActive(isActive);

        objDisabledOnce = true;

        if (objDisabledOnce)
        {
            objectivesList[4].text = "";
        }
    }

    private void ObjectivesFromResumedTraining()
    {
        for (int i = 0; i < cprTr.steps.Count; i++)
        {
            if (cprTr.steps[i].completed)
            {
                MarkObjectiveComplete(i);
            }
        }
    }

    private void MarkPhoneInteraction()
    {
        MarkObjectiveComplete(0);
    }

    private void MarkPatientInteraction()
    {
        MarkObjectiveComplete(1);
    }

    private void MarkHandsInteraction()
    {
        MarkObjectiveComplete(2);
    }

    private void MarkCprInteraction()
    {
        MarkObjectiveComplete(3);
    }

    private void MarkObjectiveComplete(int id)
    {
        objectivesList[id].color = Color.green;
        objectivesList[id].text = "<s>" + objectivesList[id].text + "</s>";
    }
}
