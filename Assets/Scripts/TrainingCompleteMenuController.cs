using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrainingCompleteMenuController : MonoBehaviour
{
    [SerializeField] GameObject trainingCompleteCanvas;
    [SerializeField] GameObject canvasToHide;
    [SerializeField] GameObject testCompleteCanvas;

    void Start()
    {
        EventHandler.OnSmallCutsClicked += SetSmallCutsEndOfSceneListener;
        EventHandler.OnLargeWoundsClicked += SetLargeWoundsEndOfSceneListener;
    }

    private void OnDestroy()
    {
        EventHandler.OnSmallCutsClicked -= SetSmallCutsEndOfSceneListener;
        EventHandler.OnLargeWoundsClicked -= SetLargeWoundsEndOfSceneListener;
        EventHandler.OnNewBandageApplied -= ShowUI;
        EventHandler.OnSinkInteraction -= ShowUI;
    }

    private void SetSmallCutsEndOfSceneListener()
    {
        EventHandler.OnNewBandageApplied += ShowUI;
    }

    private void SetLargeWoundsEndOfSceneListener()
    {
        EventHandler.OnSinkInteraction += ShowUI;
    }

    private void ShowUI()
    {
        if (StartScene.isTestMode)
        {
            testCompleteCanvas.SetActive(true);
            string passFailText = "Sorry, you didn't pass.";
            if (ScoreController.incorrectActions == 0)
            {
                passFailText = "Congratulations, you passed!";
            }
            else
            {
                testCompleteCanvas.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Incorrect Actions: " + ScoreController.incorrectActions;

            }
            testCompleteCanvas.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = passFailText;
        }
        else
        {
            trainingCompleteCanvas.SetActive(true);
        }
        canvasToHide.SetActive(false);
    }

}
