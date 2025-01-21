using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{

    private void Start()//trigger for step 2 is active on start
    {
        gameObject.SetActive(true);

    }
    public void Check()//no logner active after interaction
    {
        gameObject.SetActive(false);

    }
}
