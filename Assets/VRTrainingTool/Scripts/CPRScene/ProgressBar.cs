using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    // Related to HandButton.cs and PressureBar.cs
    // Visualizes user's progress on CPR procedure
    #if UNITY_EDITOR
    [MenuItem("GameObject/UI/Vertical Progress Bar")]
        public static void AddVerticalProgressBar()
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Vertical Progress Bar"));
            obj.transform.SetParent(Selection.activeGameObject.transform, false);
        }
    #endif

    public HandButton hb;

    private int minimum = 0;
    private int maximum;
    public int minPushesRange;
    public int maxPushesRange;
    private float current;
    private float fillAmount = 0.0f;
    private bool cprStarted = false;
    public Image mask;
    public Image fill;
    public Color color;

    // Start is called before the first frame update
    void Start()
    {
        // Number of required pressed are randomly assigned based on range
        // specified in Progress Bar script in Inspector.
        // Search for PressCounter object in Hierarchy to change values
        maximum = Random.Range(minPushesRange, maxPushesRange);

        // explore better way, maybe with Time.deltaTime
        InvokeRepeating(nameof(GraduallyDepleteProgress), 1.0f, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
        FinishTraining();
        TriggerCPR();
    }

    private void GetCurrentFill()
    {
        current = hb.pressCount;
        float currentOffset = current - minimum;
        float maximumOffset = maximum - minimum;
        fillAmount = currentOffset / maximumOffset;
        mask.fillAmount = fillAmount;
        fill.color = color;
    }

    // If user is not continuously moving controllers up and down in rapid pace
    // Progress Bar will slowly deplete until user completely fills it
    private void GraduallyDepleteProgress()
    {
        if (fillAmount > 0 && fillAmount != 1.0f)
        {
            hb.pressCount--;

            if (!cprStarted)
            {
                cprStarted = true;
            }
        }
    }

    private void TriggerCPR()
    {
        if (cprStarted)
        {
            CprEventHandler.TriggerCPRInteraction();
        }
    }

    private void FinishTraining()
    {
        if (fillAmount == 1.0f)
        {
            CprEventHandler.TriggerTrainingCompletion();
            CprEventHandler.TriggerTestingCompletion();
        }
    }
}
