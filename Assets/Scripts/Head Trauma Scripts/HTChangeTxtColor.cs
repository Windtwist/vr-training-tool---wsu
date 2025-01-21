using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HTChangeTxtColor : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI text;

    // Changes the color of textmeshpro object to green 
    public void changeToGreen()
    {
        text.GetComponent<TextMeshProUGUI>().color = Color.green; 
    }
}
