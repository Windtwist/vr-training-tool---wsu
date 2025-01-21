using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CprAudioController : MonoBehaviour
{
    // Class for subtitles and audio for CPR scenario
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject subtitleCanvas;
    [SerializeField] private GameObject subtitleWindow;

    public CprTraining cprTr;

    private AudioClip incorrectAction;
    private AudioClip trainingModePhone;
    private AudioClip testingMode;
    private AudioClip patientInteraction;
    private AudioClip handsInteraction;
    private AudioClip cprProcedure;
    private AudioClip trainingCompleted;
    private Coroutine testingBeginningCoroutine;

    private string subIncorrectAction = "This is incorrect action. Please follow the prompt.";
    private string subTestingMode = "Welcome to Testing Mode. In this mode your knowledge will be tested against time and what you learned in Training Mode. There will be no prompts or feedback during this mode.";
    private string subTrainingModePhone = "Welcome to Training Mode. In this mode you need to complete steps that are specified with interactive prompts. Please turn around to see the first step.";
    private string subPatientInteraction = "Great job. Now move to the patient on bed and move him to the hard surface. You can complete the action by interacting with patient's right hand.";
    private string subHandsInteraction = "Next place your hands on top of each other and place them on patient's chest.";
    private string subCprProcedure = "Great. Note two bars near the patient. Right bar indicates the simulated pressure while left bar indicates procedure progress. Please note that left bar will only fill when controllers are moved down in rapid pace. Now proceed to complete CPR procedure.";
    private string subTrainingCompleted = "Congratulations! You have completed CPR Training Mode. Testing Mode is now unlocked.";


    void Start()
    {
        incorrectAction = Resources.Load<AudioClip>("CPRAudio/4.CPR-IncorrectAction");
        trainingModePhone = Resources.Load<AudioClip>("CPRAudio/2.1-CPR-TrainingMode-PhoneInteraction");
        testingMode = Resources.Load<AudioClip>("CPRAudio/3.CPR-TestingMode");
        patientInteraction = Resources.Load<AudioClip>("CPRAudio/2.2-PatientInteraction");
        handsInteraction = Resources.Load<AudioClip>("CPRAudio/2.3-HandsInteraction");
        cprProcedure = Resources.Load<AudioClip>("CPRAudio/2.4-CPRProcedure");
        trainingCompleted = Resources.Load<AudioClip>("CPRAudio/2.5-CPRTrainingCompleted");

        if (!CprEventHandler.trainingResumed)
        {
            CprEventHandler.onTrainingModeSelected += PlayTrainingAndPhone;
            CprEventHandler.onTrainingModeSelected += SubscribeTrainingAudioAndSubs;
        }
        else
        {
            CprEventHandler.onLoadingCompleted += SubscribeTrainingAudioAndSubs;
        }
        CprEventHandler.onTestingModeSelected += PlayTestingMode; // SubscribeTestingAudioAndSubs;
        CprEventHandler.onTestingCountdown += DisableSubtitles;
    }

    private void DisableSubtitles()
    {
        subtitleWindow.SetActive(false);
    }

    private void SetSubtitle(string text)
    {
        subtitleCanvas.GetComponent<TextMeshProUGUI>().text = "<color=\"yellow\">Doctor: </color>" + text;
    }

    private void SubscribeTrainingAudioAndSubs()
    {
        // Subscribing to audio/subtitles events based on whether training was resumed
        // or not. If training is resumed, then previously completed steps should only have
        // incorrect action audio subscriber
        CprEventHandler.onIncorrectAction += PlayIncorrect;
        if (!cprTr.steps[0].completed)
        {
            CprEventHandler.onPhoneSelectedVerified += PlayPatientInteraction;
        }
        if (!cprTr.steps[1].completed)
        {
            CprEventHandler.onPatientMovedVerified += PlayHandsInteraction;
        }
        if (!cprTr.steps[2].completed)
        {
            CprEventHandler.onHandsPlacedVerified += PlayCprProcedure;
        }
        CprEventHandler.onCPRStarted += CprStarted;
        CprEventHandler.onTrainingCompleted += PlayTrainingCompleted;
    }

    //private void SubscribeTestingAudioAndSubs()
    //{
    //    CprEventHandler.onTestingModeSelected += PlayTestingMode;
    //}

    private void PlayIncorrect()
    {
        SetSubtitle(subIncorrectAction);
        audioSource.clip = incorrectAction;
        audioSource.Play();
    }

    private void PlayTrainingAndPhone()
    {
        SetSubtitle(subTrainingModePhone);
        audioSource.clip = trainingModePhone;
        audioSource.Play();

        CprEventHandler.onTrainingModeSelected -= PlayTrainingAndPhone;
    }

    private void PlayTestingMode()
    {
        SetSubtitle(subTestingMode);
        audioSource.clip = testingMode;
        audioSource.Play();

        testingBeginningCoroutine = StartCoroutine(WaitForSound());

        CprEventHandler.onTestingModeSelected -= PlayTestingMode;
    }

    // Timer in testing mode should start only after audio is finished playing
    private IEnumerator WaitForSound()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        CprEventHandler.TriggerTestingCountdown();
        StopCoroutine(testingBeginningCoroutine);
    }

    private void PlayPatientInteraction()
    {
        SetSubtitle(subPatientInteraction);
        audioSource.clip = patientInteraction;
        audioSource.Play();

        CprEventHandler.onPhoneSelectedVerified -= PlayPatientInteraction;
    }

    private void PlayHandsInteraction()
    {
        SetSubtitle(subHandsInteraction);
        audioSource.clip = handsInteraction;
        audioSource.Play();

        CprEventHandler.onPatientMovedVerified -= PlayHandsInteraction;
    }

    private void PlayCprProcedure()
    {
        SetSubtitle(subCprProcedure);
        audioSource.clip = cprProcedure;
        audioSource.Play();

        CprEventHandler.onHandsPlacedVerified -= PlayCprProcedure;
    }

    private void CprStarted()
    {
        CprEventHandler.onTrainingModeSelected -= PlayTrainingAndPhone;
        CprEventHandler.onPhoneSelectedVerified -= PlayPatientInteraction;
        CprEventHandler.onPatientMovedVerified -= PlayHandsInteraction;
        CprEventHandler.onHandsPlacedVerified -= PlayCprProcedure;
    }

    private void PlayTrainingCompleted()
    {
        SetSubtitle(subTrainingCompleted);
        audioSource.clip = trainingCompleted;
        audioSource.Play();

        UnsubscribeFromObjectives();
    }

    private void UnsubscribeFromObjectives()
    {
        CprEventHandler.onIncorrectAction -= PlayIncorrect;
        CprEventHandler.onTrainingModeSelected -= PlayTrainingAndPhone;
        CprEventHandler.onTrainingModeSelected -= SubscribeTrainingAudioAndSubs;
        CprEventHandler.onPhoneSelectedVerified -= PlayPatientInteraction;
        CprEventHandler.onPatientMovedVerified -= PlayHandsInteraction;
        CprEventHandler.onHandsPlacedVerified -= PlayCprProcedure;
        CprEventHandler.onCPRStarted -= CprStarted;
        CprEventHandler.onTrainingCompleted -= PlayTrainingCompleted;
        CprEventHandler.onTrainingCompleted -= UnsubscribeFromObjectives;
        CprEventHandler.onLoadingCompleted -= SubscribeTrainingAudioAndSubs;
        //CprEventHandler.onTestingModeSelected -= SubscribeTestingAudioAndSubs;
        CprEventHandler.onTestingModeSelected -= PlayTestingMode;
        CprEventHandler.onTestingCountdown -= DisableSubtitles;
    }

    private void OnDestroy()
    {
        UnsubscribeFromObjectives();
    }
}
