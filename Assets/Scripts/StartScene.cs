using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartScene : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject SmallCutsModel;
    [SerializeField] GameObject LargeWoundsModel;

    public static bool isTestMode = false;

    private void Start()
    {
        EventHandler.OnSmallCutsClicked += SmallCutsWhiteboard;
        EventHandler.OnLargeWoundsClicked += LargeWoundsWhiteboard;
    }

    public void SetIsTest(bool isTest)
    {
        isTestMode = isTest;
    }

    public void BeginSmallCuts()
    {
        SmallCutsModel.SetActive(true);

        string sceneName = "Small cuts and scrapes";

        foreach (Transform child in canvas.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        string text = "This scene will teach you to deal with small cuts and scrapes. The steps are:\n";
        if (isTestMode)
        {
            text = "You have selected test mode. Good luck!";
            AddTextComponent(sceneName, 8.0f, TextAlignmentOptions.Top, new Vector3(0, 38.5f), 200);
            AddTextComponent(text, 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, 20), 200);
            return;
        }

        AddTextComponent(sceneName, 8.0f, TextAlignmentOptions.Top, new Vector3(0, 38.5f), 200);
        AddTextComponent(text, 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, 20), 200);
        AddTextComponent("Step 1: Wash your hands", 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, 3), 160);
        AddTextComponent("Step 2: Get clean cloth from under sink", 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, -5), 160);
        AddTextComponent("Step 3: Apply pressure to the wound until it stops bleeding", 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, -17), 160);
        AddTextComponent("Step 4: Get antibiotic substance and clean the wound", 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, -28), 160);
        AddTextComponent("Step 5: Get a new bandage and cover the wound", 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, -41), 160);

        //EventHandler.OnSinkInteraction += SinkInteraction;
        //EventHandler.OnBandageGrabbed += BandageGrabbed;
        //EventHandler.OnPressureInteraction += PressureApplied;
        //EventHandler.OnAntibioticApplied += AntibioticApplied;
        //EventHandler.OnNewBandageApplied += NewBandageApplied;
        //EventHandler.OnSmallCutsClicked -= SmallCutsWhiteboard;

    }

    private void BeginLargeWounds()
    {
        LargeWoundsModel.SetActive(true);
        string sceneName = "Large wounds with heavy bleeding";

        foreach (Transform child in canvas.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        string text = "This scene will teach you to deal with large wounds with heavy bleeding. The steps are:\n";

        if (isTestMode)
        {
            text = "You have selected test mode. Good luck!";
            AddTextComponent(sceneName, 8.0f, TextAlignmentOptions.Top, new Vector3(0, 38.5f), 220);
            AddTextComponent(text, 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, 22), 220);
            return;
        }

        AddTextComponent(sceneName, 8.0f, TextAlignmentOptions.Top, new Vector3(0, 38.5f), 220);
        AddTextComponent(text, 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, 22), 220);
        AddTextComponent("Step 1: Call emergency services for help", 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, 10), 200);
        AddTextComponent("Step 2: Remove clothing and debris from around the wound", 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, 4), 200);
        AddTextComponent("Step 3: Get clean bandage from under sink", 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, -7), 200);
        AddTextComponent("Step 4: Apply pressure to the wound to slow bleeding", 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, -12), 200);
        AddTextComponent("Step 5: Help the patient lie down", 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, -24), 200);
        AddTextComponent("Step 6: Add more bandages as needed", 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, -31), 200);
        AddTextComponent("Step 7: Apply tourniquet if necessary", 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, -39), 200);
        AddTextComponent("Step 8: After help has arrived, wash your hands", 4.0f, TextAlignmentOptions.TopLeft, new Vector3(0, -45), 200);



        EventHandler.OnLargeWoundsClicked -= LargeWoundsWhiteboard;
        //EventHandler.OnPhoneInteraction += PhoneInteraction;
        //EventHandler.OnRemoveDebrisInteraction += RemoveDebrisInteraction;
        //EventHandler.OnBandageGrabbed += LargeWoundsBandageGrabbed;
        //EventHandler.OnPressureLargeWoundsInteraction += PressureLargeWoundsInteraction;
        //EventHandler.OnLieDownInteraction += LieDownInteraction;
        //EventHandler.OnNewBandageApplied += NewBandageAppliedLargeWounds;
        //EventHandler.OnTourniquetApplied += TourniquetApplied;
        //EventHandler.OnSinkInteraction += SinkInteractionLargeWounds;
    }

    private void OnDestroy()
    {
        EventHandler.OnSinkInteraction -= SinkInteraction;
        EventHandler.OnBandageGrabbed -= BandageGrabbed;
        EventHandler.OnPressureInteraction -= PressureApplied;
        EventHandler.OnAntibioticApplied -= AntibioticApplied;
        EventHandler.OnNewBandageApplied -= NewBandageApplied;
        EventHandler.OnSmallCutsClicked -= SmallCutsWhiteboard;

        EventHandler.OnLargeWoundsClicked -= LargeWoundsWhiteboard;
        EventHandler.OnPhoneInteraction -= PhoneInteraction;
        EventHandler.OnRemoveDebrisInteraction -= RemoveDebrisInteraction;
        EventHandler.OnBandageGrabbed -= LargeWoundsBandageGrabbed;
        EventHandler.OnPressureLargeWoundsInteraction -= PressureLargeWoundsInteraction;
        EventHandler.OnLieDownInteraction -= LieDownInteraction;
        EventHandler.OnNewBandageApplied -= NewBandageAppliedLargeWounds;
        EventHandler.OnTourniquetApplied -= TourniquetApplied;
        EventHandler.OnSinkInteraction -= SinkInteractionLargeWounds;
    }

    private void AddTextComponent(string text, float fontSize, TextAlignmentOptions alignment, Vector3 position, int width)
    {
        GameObject newText = new GameObject();
        newText.AddComponent<TextMeshProUGUI>().text = text;
        newText.GetComponent<TextMeshProUGUI>().fontSize = fontSize * 2.5f;
        newText.GetComponent<TextMeshProUGUI>().color = Color.black;
        newText.GetComponent<TextMeshProUGUI>().alignment = alignment;
        newText.GetComponent<TextMeshProUGUI>().transform.localPosition = position;
        newText.GetComponent<TextMeshProUGUI>().transform.localScale = new Vector3(0.4f, 0.4f, 1.0f);
        newText.GetComponent<TextMeshProUGUI>().GetComponent<RectTransform>().sizeDelta = new Vector2(width, fontSize * 2.5f * 2 + 10);
        newText.transform.SetParent(canvas.transform, false);

    }

    void SinkInteraction()
    {
        canvas.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.grey;
    }

    void BandageGrabbed()
    {
        canvas.transform.GetChild(3).GetComponent<TextMeshProUGUI>().color = Color.grey;
    }

    void PressureApplied()
    {
        canvas.transform.GetChild(4).GetComponent<TextMeshProUGUI>().color = Color.grey;
    }

    void AntibioticApplied()
    {
        canvas.transform.GetChild(5).GetComponent<TextMeshProUGUI>().color = Color.grey;
    }

    void NewBandageApplied()
    {
        canvas.transform.GetChild(6).GetComponent<TextMeshProUGUI>().color = Color.grey;
    }

    void SmallCutsWhiteboard()
    {
        BeginSmallCuts();
    }

    void LargeWoundsWhiteboard()
    {
        BeginLargeWounds();
    }

    void PhoneInteraction()
    {
        canvas.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.grey;

    }


    void RemoveDebrisInteraction()
    {
        canvas.transform.GetChild(3).GetComponent<TextMeshProUGUI>().color = Color.grey;

    }

    void LargeWoundsBandageGrabbed()
    {
        canvas.transform.GetChild(4).GetComponent<TextMeshProUGUI>().color = Color.grey;
    }

    void PressureLargeWoundsInteraction()
    {
        canvas.transform.GetChild(5).GetComponent<TextMeshProUGUI>().color = Color.grey;
    }

    void LieDownInteraction()
    { 
        canvas.transform.GetChild(6).GetComponent<TextMeshProUGUI>().color = Color.grey;

    }
    void NewBandageAppliedLargeWounds()
    {
        canvas.transform.GetChild(7).GetComponent<TextMeshProUGUI>().color = Color.grey;
    }
    void TourniquetApplied()
    {
        canvas.transform.GetChild(8).GetComponent<TextMeshProUGUI>().color = Color.grey;
    }
       
    void SinkInteractionLargeWounds()
    {
        canvas.transform.GetChild(9).GetComponent<TextMeshProUGUI>().color = Color.grey;
    }

}
