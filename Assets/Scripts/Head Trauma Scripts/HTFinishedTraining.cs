using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTFinishedTraining : MonoBehaviour
{
 public GameObject trainingCompleteCanvas;
     
    //This function delays the ending of training mode by 18 seconds 
    //the purpose is to give the audio instruction for step 4 time to play

    public void endTraining()
    {
        StartCoroutine(timeDelay());

    }

    IEnumerator timeDelay()
    {
        yield return new WaitForSeconds(18);
        trainingCompleteCanvas.SetActive(true);

    }


}
