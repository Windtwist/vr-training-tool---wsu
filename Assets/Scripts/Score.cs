using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Score : MonoBehaviour
{
    public int playerScore = 0;
    public string state_desc = "Button has been pressed";
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(HandleScoreButtonClick);
    }

    public void HandleScoreButtonClick()
    {
        
            Debug.Log("Before updating score: " + playerScore);
            playerScore++;
            Debug.Log("After updating score: " + playerScore);
        
        GameObject saveButton = GameObject.Find("button");
        if (saveButton != null)
        {
            SaveToJson saveButtonListener = saveButton.GetComponent<SaveToJson>();
            if (saveButtonListener != null)
            {
                saveButtonListener.UpdatePlayerScore(playerScore);
                saveButtonListener.Description(state_desc);

            }
        }
    }
}
