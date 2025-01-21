using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CprMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject rayInteractors;
    [SerializeField] private GameObject handsInteractors;

    // Start is called before the first frame update
    void Start()
    {
        handsInteractors.SetActive(true);
        rayInteractors.SetActive(false);

        // CprEventHandler.onTrainingModeSelected += SubscribeOnTraining;
        // CprEventHandler.onTestingModeSelected += SubscribeOnTesting;
    }

    // conditional subscription due to both onTrainingCompleted and onTestingCompleted
    // being triggered on scenario end in ProgressBar.cs

    private void SubscribeOnTraining()
    {
        RaysChangeState();
        HandsChangeState();
        CprEventHandler.onTrainingCompleted += RaysChangeState;
        CprEventHandler.onTrainingCompleted += HandsChangeState;
    }

    private void SubscribeOnTesting()
    {
        RaysChangeState();
        HandsChangeState();
        CprEventHandler.onTestingCompleted += RaysChangeState;
        CprEventHandler.onTestingCompleted += HandsChangeState;
    }

    private void HandsChangeState()
    {
        bool active = handsInteractors.activeSelf;
        handsInteractors.SetActive(!active);
    }

    private void RaysChangeState()
    {
        bool active = rayInteractors.activeInHierarchy;
        rayInteractors.SetActive(!active);
    }

    private void OnDestroy()
    {
        CprEventHandler.onTrainingModeSelected -= SubscribeOnTraining;
        CprEventHandler.onTestingModeSelected -= SubscribeOnTesting;
        CprEventHandler.onTrainingCompleted -= RaysChangeState;
        CprEventHandler.onTrainingCompleted -= HandsChangeState;
        CprEventHandler.onTestingCompleted -= RaysChangeState;
        CprEventHandler.onTestingCompleted -= HandsChangeState;
    }
}
