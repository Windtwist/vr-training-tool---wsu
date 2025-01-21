using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SmallCutsAudioController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject subtitleCanvas;
    private AudioClip welcome;
    private AudioClip washHands;
    private AudioClip washHandsIncorrect;
    private AudioClip openDoor;
    private AudioClip openDoorIncorrect;
    private AudioClip applyPressure;
    private AudioClip applyPressureIncorrect;
    private AudioClip antibioticGrab;
    private AudioClip antibioticGrabIncorrect;
    private AudioClip antibioticApplied;
    private AudioClip antibioticAppliedIncorrect;
    private AudioClip newBandageGrabbed;
    private AudioClip newBandageGrabbedIncorrect;
    private AudioClip endOfSmallCuts;

    private string welcomeTranscription = "Welcome to bandage training! Select an option to begin.";
    private string washHandsTranscription = "In this scenario, I will teach you the basics steps required to properly treat small cuts and scrapes. The first step will be to wash your hands to ensure you don't infect the wound. There is a sink against the wall. Wash your hands now.";
    private string washHandsIncorrectTranscription = "Remember, the first step is to wash your hands. You must do that before we can move forward.";
    private string openDoorTranscription = "Great job. We can now begin treating the wound. Do do so, we must first apply pressure to the wound with a clean bandage to stop the bleeding. There are clean bandages under the sink. Grab them now.";
    private string openDoorIncorrectTranscription = "Remember, we must get the bandages to apply pressure to the wound. Interact with the sink door handle to open the door and grab the bandage.";
    private string applyPressureTranscription = "Good. Now that you have the bandage, drop it near the wound to simulate applying pressure. In practice, you should apply pressure until the wound stops bleeding. Do that now.";
    private string applyPressureIncorrectTranscription = "Make sure to apply pressure to the wound to stop bleeding. This must be done before we can continue.";
    private string antibioticGrabTranscription = "Great job. Now that the wound has stopped bleeding, we can disinfect the wound. This decreases the chances of the wound becoming infected. Grab the antibiotic off of the sink now.";
    private string antibioticGrabIncorrectTranscription = "Remember, we must disinfect the wound to reduce chances of infection. Get the antibiotic substance from the counter top near the sink.";
    private string antibioticAppliedTranscription = "Now, apply the antibiotic to the wound.";
    private string antibioticAppliedIncorrectTranscription = "You must apply the antibiotic to the wound before we can continue";
    private string newBandageGrabbedTranscription = "Great job! We're almost done. The last step is to bandage the wound. Find a clean bandage and apply it to the wound.";
    private string newBandageGrabbedIncorrectTranscription = "You must apply a bandage to the wound to continue.";
    private string endOfSmallCutsTranscription = "Congratulations, you completed the training. After applying the first bandage, it is important to monitor the wound for infection. You should remove the bandage and look for pus or increased swelling in the area around the wound. Then, you should bandage the wound again with a clean bandage. If signs of infection appear, consult a doctor immediately. You may now complete the test for the small cuts and scrapes scenario.";

    private AudioClip incorrectClip;
    private string incorrectSubtitle;

    private bool isTestMode = false;

    void Awake()
    {
        welcome = Resources.Load<AudioClip>("SmallCutsAudio/welcome");
        washHands = Resources.Load<AudioClip>("SmallCutsAudio/wash_hands");
        washHandsIncorrect = Resources.Load<AudioClip>("SmallCutsAudio/wash_hands_incorrect");
        openDoor = Resources.Load<AudioClip>("SmallCutsAudio/open_door");
        openDoorIncorrect = Resources.Load<AudioClip>("SmallCutsAudio/open_door_incorrect");
        applyPressure = Resources.Load<AudioClip>("SmallCutsAudio/apply_pressure");
        applyPressureIncorrect = Resources.Load<AudioClip>("SmallCutsAudio/apply_pressure_incorrect");
        antibioticGrab = Resources.Load<AudioClip>("SmallCutsAudio/antibiotic_grab");
        antibioticGrabIncorrect = Resources.Load<AudioClip>("SmallCutsAudio/antibiotic_grab_incorrect");
        antibioticApplied = Resources.Load<AudioClip>("SmallCutsAudio/antibiotic_applied");
        antibioticAppliedIncorrect = Resources.Load<AudioClip>("SmallCutsAudio/antibiotic_applied_incorrect");
        newBandageGrabbed = Resources.Load<AudioClip>("SmallCutsAudio/new_bandage");
        newBandageGrabbedIncorrect = Resources.Load<AudioClip>("SmallCutsAudio/new_bandage_incorrect");
        endOfSmallCuts = Resources.Load<AudioClip>("SmallCutsAudio/end_of_small_cuts");
    }

    private void Start()
    {
        audioSource.clip = welcome;
        SetSubtitle(welcomeTranscription);
        audioSource.Play();
        EventHandler.OnSmallCutsClicked += BeginAudioSequence;
    }

    public void SetIsTest(bool test)
    {
        isTestMode = test;
    }

    private void PlayCorrect(AudioClip clip, string subtitle)
    {
        audioSource.clip = clip;
        audioSource.Play();
        SetSubtitle(subtitle);
    }

    private void PlayIncorrect()
    {
        audioSource.clip = incorrectClip;
        audioSource.Play();
        SetSubtitle(incorrectSubtitle);
    }

    private void SetSubtitle(string text)
    {
        subtitleCanvas.GetComponent<TextMeshProUGUI>().text = "<color=\"yellow\">Doctor: </color>" + text;
    }

    private void BeginAudioSequence()
    {
        if (isTestMode) return;
        EventHandler.OnSmallCutsClicked -= BeginAudioSequence;
        EventHandler.OnIncorrectAction += PlayIncorrect;
        EventHandler.OnSinkInteraction += SetOpenDoorClips;
        EventHandler.OnBandageGrabbed += SetPressureClips;
        EventHandler.OnPressureInteraction += SetAntibioticGrabbedClips;
        EventHandler.OnAntibioticGrabbed += SetAntibioticAppliedClips;
        EventHandler.OnAntibioticApplied += SetNewBandageClips;
        EventHandler.OnNewBandageApplied += PlayEndOfScene;

        incorrectClip = washHandsIncorrect;
        PlayCorrect(washHands, washHandsTranscription);
    }

    private void OnDestroy()
    {
        EventHandler.OnSmallCutsClicked -= BeginAudioSequence;
        EventHandler.OnIncorrectAction -= PlayIncorrect;
        EventHandler.OnSinkInteraction -= SetOpenDoorClips;
        EventHandler.OnBandageGrabbed -= SetPressureClips;
        EventHandler.OnPressureInteraction -= SetAntibioticGrabbedClips;
        EventHandler.OnAntibioticGrabbed -= SetAntibioticAppliedClips;
        EventHandler.OnAntibioticApplied -= SetNewBandageClips;
        EventHandler.OnNewBandageApplied -= PlayEndOfScene;
    }

    private void SetOpenDoorClips()
    {
        PlayCorrect(openDoor, openDoorTranscription);
        incorrectClip = openDoorIncorrect;
        incorrectSubtitle = openDoorIncorrectTranscription;
    }

    private void SetPressureClips()
    {
        PlayCorrect(applyPressure, applyPressureTranscription);
        incorrectClip = applyPressureIncorrect;
        incorrectSubtitle = applyPressureIncorrectTranscription;
    }

    private void SetAntibioticGrabbedClips()
    {
        PlayCorrect(antibioticGrab, antibioticGrabTranscription);
        incorrectClip = antibioticGrabIncorrect;
        incorrectSubtitle = antibioticGrabIncorrectTranscription;
    }
    private void SetAntibioticAppliedClips()
    {
        PlayCorrect(antibioticApplied, antibioticAppliedTranscription);
        incorrectClip = antibioticAppliedIncorrect;
        incorrectSubtitle = antibioticAppliedIncorrectTranscription;
    }

    private void SetNewBandageClips()
    {
        PlayCorrect(newBandageGrabbed, newBandageGrabbedTranscription);
        incorrectClip = newBandageGrabbedIncorrect;
        incorrectSubtitle = newBandageGrabbedIncorrectTranscription;
    }

    private void PlayEndOfScene()
    {
        audioSource.clip = endOfSmallCuts;
        audioSource.Play();
        SetSubtitle(endOfSmallCutsTranscription);
    }
}
