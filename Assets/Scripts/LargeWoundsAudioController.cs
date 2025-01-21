using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static EventHandler;

public class LargeWoundsAudioController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject subtitleCanvas;
    private AudioClip phone;
    private AudioClip phoneIncorrect;
    private AudioClip debris;
    private AudioClip debrisIncorrect;
    private AudioClip grabBandage;
    private AudioClip grabBandageIncorrect;
    private AudioClip applyPressure;
    private AudioClip applyPressureIncorrect;
    private AudioClip lieDown;
    private AudioClip lieDownIncorrect;
    private AudioClip newBandage;
    private AudioClip newBandageIncorrect;
    private AudioClip tourniquet;
    private AudioClip tourniquetIncorrect;
    private AudioClip washHands;
    private AudioClip washHandsIncorrect;
    private AudioClip endOfLargeWounds;

    private string phoneTranscription = "In this scenario, I will teach you how to handle large wounds that are bleeding heavily. The first step is to call emergency services for assistance. There is a phone next to the door. Call for help now.";
    private string phoneIncorrectTranscription = "Remember, calling emergency services for help is the first step. The phone is near the door. You must call for help before we can continue.";
    private string debrisTranscription = "Good. Now that help is on the way, we can begin handling the wound. First, we must remove debris from around the wound. If there is debris in the wound itself do not remove it. Interact with the wound to remove debris from around the wound now.";
    private string debrisIncorrectTranscription = "Remember, the next step is to remove debris from around the wound. Interact with the wound so that we can continue.";
    private string grabBandageTranscription = "Great job. Now you should slow bleeding from the wound by applying pressure with a clean bandage. There is a clean bandage under the sink. Grab it and apply pressure now.";
    private string grabBandageIncorrectTranscription = "You must get the bandage to continue. Interact with the sink door handle to open the door, then grab the bandage.";
    private string applyPressureTranscription = "Good. Now that you have the bandage, apply it to the wound to simulate applying pressure. In practice, this should be done until help arrives. ";
    private string applyPressureIncorrectTranscription = "You must apply pressure to the wound to continue. Place the bandage near the wound and drop it to simulate applying pressure. ";
    private string lieDownTranscription = "Great job. Now, you should help the patient lie down. This can slow the bleeding by reducing the patient's heart rate and reducing blood flow to the wound. Interact with the patient to help them lie down now.";
    private string lieDownIncorrectTranscription = "You must help the patient lie down to slow bleeding. Interact with the patient to help them lie down now.";
    private string newBandageTranscription = "Good. Now that the patient is lying down, you should replace the bandage. This should be done continuously as the patient bleeds through the bandages. Grab a new bandage and apply it.";
    private string newBandageIncorrectTranscription = "You must replace the bandages to continue. Grab a new bandage and drop it near the wound to simulate applying the bandage.";
    private string tourniquetTranscription = "If the patient is losing blood at an alarming rate and is at risk of dying due to blood loss, applying a tourniquet may be necessary. If you are not trained in applying tourniquets, you should continue applying pressure and wait for help to arrive. However, for this scenario, we will simulate applying a tournquet. Grab the tourniquet and apply it to the patient's leg now.";
    private string tourniquetIncorrectTranscription = "You must apply the tourniquet to continue. Grab the tourniquet and drop it near the patient's leg to simulate applying the tourniquet.";
    private string washHandsTranscription = "Great job, the patient has been helped by emergency services. After dealing with patients where blood loss is significant, it is important to wash your hands immediately afterwards. Do that now.";
    private string washHandsIncorrectTranscription = "You must wash your hands to complete this scenario. There is a sink near the wall. Interact with the sink handle to wash your hands.";
    private string endOfLargeWoundsTranscription = "Congratulations, you've completed the large wounds with heavy bleeding scenario. Remember, when dealing with patients with life-threatening wounds, the most important thing to do is call for help. You may now continue with the testing scenario for large wounds with heavy bleeding.";

    private AudioClip incorrectClip;
    private string incorrectSubtitle;

    private bool isTestMode = false;

    void Awake()
    {
    phone = Resources.Load<AudioClip>("LargeWoundsAudio/phone");
    phoneIncorrect = Resources.Load<AudioClip>("LargeWoundsAudio/phone_incorrect");
    debris = Resources.Load<AudioClip>("LargeWoundsAudio/debris");
    debrisIncorrect = Resources.Load<AudioClip>("LargeWoundsAudio/debris_incorrect");
    grabBandage = Resources.Load<AudioClip>("LargeWoundsAudio/grab_bandage");
    grabBandageIncorrect = Resources.Load<AudioClip>("LargeWoundsAudio/grab_bandage_incorrect");
    applyPressure = Resources.Load<AudioClip>("LargeWoundsAudio/apply_pressure");
    applyPressureIncorrect = Resources.Load<AudioClip>("LargeWoundsAudio/apply_pressure_incorrect");
    lieDown = Resources.Load<AudioClip>("LargeWoundsAudio/lie_down");
    lieDownIncorrect = Resources.Load<AudioClip>("LargeWoundsAudio/lie_down_incorrect");
    newBandage = Resources.Load<AudioClip>("LargeWoundsAudio/new_bandage");
    newBandageIncorrect = Resources.Load<AudioClip>("LargeWoundsAudio/new_bandage_incorrect");
    tourniquet = Resources.Load<AudioClip>("LargeWoundsAudio/tourniquet");
    tourniquetIncorrect = Resources.Load<AudioClip>("LargeWoundsAudio/tourniquet_incorrect");
    washHands = Resources.Load<AudioClip>("LargeWoundsAudio/wash_hands");
    washHandsIncorrect = Resources.Load<AudioClip>("LargeWoundsAudio/wash_hands_incorrect");
    endOfLargeWounds = Resources.Load<AudioClip>("LargeWoundsAudio/end_of_large_wounds");
}

    void Start()
    {
        EventHandler.OnLargeWoundsClicked += SetAudioSequence;
    }

    private void OnDestroy()
    {
        EventHandler.OnLargeWoundsClicked -= SetAudioSequence;
        EventHandler.OnIncorrectAction -= PlayIncorrect;
        EventHandler.OnPhoneInteraction -= SetDebrisClips;
        EventHandler.OnRemoveDebrisInteraction -= SetBandageClips;
        EventHandler.OnBandageGrabbed -= SetPressureClips;
        EventHandler.OnPressureLargeWoundsInteraction -= SetLieDownClips;
        EventHandler.OnLieDownInteraction -= SetNewBandageClips;
        EventHandler.OnNewBandageApplied -= SetTourniquetClips;
        EventHandler.OnTourniquetApplied -= SetWashHandsClips;
        EventHandler.OnSinkInteraction -= PlayEndOfScene;
    }

    public void SetIsTest(bool isTest)
    {
        isTestMode = isTest;
    }

    void PlayCorrect(AudioClip clip, string subtitle)
    {
        audioSource.clip = clip;
        audioSource.Play();
        SetSubtitle(subtitle);
    }

    void PlayIncorrect()
    {
        audioSource.clip = incorrectClip;
        SetSubtitle(incorrectSubtitle);
        audioSource.Play();
    }

    private void SetSubtitle(string text)
    {
        subtitleCanvas.GetComponent<TextMeshProUGUI>().text = "<color=\"yellow\">Doctor: </color>" + text;
    }

    private void SetAudioSequence()
    {
        if (isTestMode) return;
        EventHandler.OnIncorrectAction -= PlayIncorrect;
        EventHandler.OnPhoneInteraction += SetDebrisClips;
        EventHandler.OnRemoveDebrisInteraction += SetBandageClips;
        EventHandler.OnBandageGrabbed += SetPressureClips;
        EventHandler.OnPressureLargeWoundsInteraction += SetLieDownClips;
        EventHandler.OnLieDownInteraction += SetNewBandageClips;
        EventHandler.OnNewBandageApplied += SetTourniquetClips;
        EventHandler.OnTourniquetApplied += SetWashHandsClips;
        EventHandler.OnSinkInteraction += PlayEndOfScene;

        
        incorrectClip = phoneIncorrect;
        incorrectSubtitle = phoneIncorrectTranscription;
        PlayCorrect(phone, phoneTranscription);
    }

    private void SetDebrisClips()
    {
        PlayCorrect(debris, debrisTranscription);
        incorrectClip = debrisIncorrect;
        incorrectSubtitle = debrisIncorrectTranscription;
    }

    private void SetBandageClips()
    {
        PlayCorrect(grabBandage, grabBandageTranscription);
        incorrectClip = grabBandageIncorrect;
        incorrectSubtitle = grabBandageIncorrectTranscription;
    }

    private void SetPressureClips()
    {
        PlayCorrect(applyPressure, applyPressureTranscription);
        incorrectClip = applyPressureIncorrect;
        incorrectSubtitle = applyPressureIncorrectTranscription;
    }

    private void SetLieDownClips()
    {
        PlayCorrect(lieDown, lieDownTranscription);
        incorrectClip = lieDownIncorrect;
        incorrectSubtitle = lieDownIncorrectTranscription;
    }

    private void SetNewBandageClips()
    {
        PlayCorrect(newBandage, newBandageTranscription);
        incorrectClip = newBandageIncorrect;
        incorrectSubtitle= newBandageIncorrectTranscription;
    }

    private void SetTourniquetClips()
    {
        PlayCorrect(tourniquet, tourniquetTranscription);
        incorrectClip = tourniquetIncorrect;
        incorrectSubtitle = tourniquetIncorrectTranscription;
    }

    private void SetWashHandsClips()
    {
        PlayCorrect(washHands, washHandsTranscription);
        incorrectClip = washHandsIncorrect;
        incorrectSubtitle = washHandsIncorrectTranscription;
    }

    private void PlayEndOfScene()
    {
        audioSource.clip = endOfLargeWounds;
        audioSource.Play();
        SetSubtitle(endOfLargeWoundsTranscription);
    }
}
