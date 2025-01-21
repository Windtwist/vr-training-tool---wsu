using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode()]
public class PressureBar : MonoBehaviour
{
    // Extension class for HandButton.cs functionality.
    // It visualizes button displacement in Pressure Bar
    #if UNITY_EDITOR
    [MenuItem("GameObject/UI/Vertical Pressure Bar")]
        public static void AddVerticalPressureBar()
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Vertical Pressure Bar"));
            obj.transform.SetParent(Selection.activeGameObject.transform, false);
        }
    #endif

    public HandButton hb;
    public float maximumBarValue;
    public float minimumBarValue;
    public float fillAmount;
    private float btnTopDisplacement;
    private float btnBottomDisplacement;
    private float current;
    public Image mask;
    public Image fill;


    // Start is called before the first frame update
    void Start()
    {
        //btnTopDisplacement = hb.YMin;
        //btnBottomDisplacement = hb.YMax;
        btnTopDisplacement = 0.0141f; //0.02557503f; //8.383685f;
        btnBottomDisplacement = -0.0077f; //8.331266f;
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();

    }

    private void GetCurrentFill()
    {
        // Displacement values from HandButton are used to fill the progress bar that user can use for reference
        float pressureAmount = Mathf.InverseLerp(btnTopDisplacement, btnBottomDisplacement, hb.pressureAmount);
        current = Mathf.Lerp(minimumBarValue, maximumBarValue, pressureAmount);
        fillAmount = current / maximumBarValue;
        mask.fillAmount = fillAmount;
        ChangePressureColor(fillAmount);
    }

    private void ChangePressureColor(float fillAmount)
    {
        if (fillAmount < 0.90f)
        {
            fill.color = Color.red;
        }
        if (fillAmount >= 0.90f)
        {
            fill.color = Color.green;
        }
    }
}
