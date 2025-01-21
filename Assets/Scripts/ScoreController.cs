using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public static int correctActions = 0;
    public static int incorrectActions = 0;

    // Start is called before the first frame update
    void Start()
    {
        EventHandler.OnCorrectAction += IncrementCorrect;
        EventHandler.OnIncorrectAction += IncrementIncorrect;
    }

    private void OnDestroy()
    {
        EventHandler.OnCorrectAction -= IncrementCorrect;
        EventHandler.OnIncorrectAction -= IncrementIncorrect;
    }

    private void IncrementCorrect()
    {
        correctActions++;
    }

    private void IncrementIncorrect()
    {
        incorrectActions++;
        Debug.Log("Incorrect: " + incorrectActions);
    }

    public void Print()
    {
        Debug.Log("Correct: " +  correctActions + "\nIncorrect: " + incorrectActions);
    }
}
