using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestTimerController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countdownText;

    private Coroutine coroutine;

    private int countdownTimer = 30;
    private IEnumerator ScenarioCountdown()
    {
        string textColor = "";

        while (countdownTimer > 0)
        {
            if (countdownTimer >= 15)
            {
                textColor = "green";
            }
            else if (countdownTimer <= 15 && countdownTimer >= 5)
            {
                textColor = "yellow";
            }
            else
            {
                textColor = "red";
            }

            if (countdownTimer <= 30)
            {
                countdownText.text = String.Format("Timer: <color=\"{0}\">{1} </color>", textColor, countdownTimer.ToString());
            }

            countdownTimer--;

            yield return new WaitForSeconds(1);
        }

        countdownText.text = "";
        EventHandler.TriggerTimeout();
    }

    public void StartTimer()
    {
        coroutine = StartCoroutine(ScenarioCountdown());
    }

    public void StopTimer()
    {
        Debug.Log("Timer stopped");
        StopCoroutine(coroutine);
    }
}
