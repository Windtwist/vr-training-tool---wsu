using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//This class functions as a testing mode controller,
//it detects when actions are done out of order and will determine pass/fail
public class HTTestingMode : MonoBehaviour
{   
    //Boolean for each interaction, and one for passing
    private bool step1Complete;
    private bool step2Complete;
    private bool step3aComplete;
    private bool step3bComplete;
    private bool step3cComplete;
    private bool pass;

    //All objects that are relevant to testing mode
    public GameObject phone;
    public GameObject phoneTest;
    public GameObject step2Box;
    public GameObject step2BoxTest;
    public GameObject sinkDoor;
    public GameObject sinkDoorTest;
    public GameObject bandage;
    public GameObject bandageTest;
    public GameObject attachPoint;
    public GameObject attachPointTest;
    public GameObject passCanvas;
    public GameObject failCanvas;
    public GameObject step3Indicator;
    public Button TestingButton;

    //This function is called when testing mode starts
    public void TestingStart()
    {
        //Set training mode objects to false
        phone.SetActive(false);
        sinkDoor.SetActive(false);
        bandage.SetActive(false);
        attachPoint.SetActive(false);
        step2Box.SetActive(false);
        step3Indicator.SetActive(false);

        //Set testing mode objects to true
        phoneTest.SetActive(true);
        sinkDoorTest.SetActive(true);
        bandageTest.SetActive(true);
        attachPointTest.SetActive(true);
        step2BoxTest.SetActive(true);
        TestingButton.interactable = true;

        //initialize all step boolean values to false
        step1Complete = false;
        step2Complete = false;
        step3aComplete = false;
        step3bComplete = false;
        step3cComplete = false;
        pass = true;

    }

    //This function is called if the user fails testing mode
    //and wants to try again
    public void TestingRetry()
    {
        TestingStart();
        TestingButton.interactable = true;
        ResetScene();
        
    }

    //This function is called upon completion of step 1
    public void Step1()
    {
        if (step2Complete || step3aComplete)
        {
            pass = false;
        }

        step1Complete = true;

    }

    //This function is called upon completion of step 2
    public void Step2()
    {
        if (!step1Complete || step3aComplete || step2Complete)
        {
            pass = false;
        }

        step2Complete = true;

    }

    //This function is called when the user opens the sink door
    public void Step3a()
    {
        if (!step1Complete || !step2Complete)
        {
            pass = false;
        }

        step3aComplete = true;
    }

    //This function is called when the user grabs the bandage
    public void Step3b()
    {
        if (!step1Complete || !step2Complete)
        {
            pass = false;
        }

        step3bComplete = true;
    }

    //This function is called when the user places the bandage on the head of the patient
    public void Step3c()
    {
        if (!step1Complete || !step2Complete || !step3aComplete || !step3bComplete)
        {
            pass = false;
        }

        step3cComplete = true;

    }

    //This function resets the scene, and is called when the user retrys testing mode
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Scene Reset");
    }

    //This function is called when the user finishes testing mode,
    //and displays the appropriate UI.
    public void FinishedTest()
    {
        if (pass)
        {
            passCanvas.SetActive(true);
        }
        else
        {
            failCanvas.SetActive(true);
        }
    }
        

}
