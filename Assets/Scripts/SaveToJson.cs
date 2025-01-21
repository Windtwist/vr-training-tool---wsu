using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class PlayerData
{
    public int recordID;
    public int playerScore;
    public bool state; //bool of state 
    public string state_desc;
    public string date;
}

public class EventData
{
    public List<string> events = new List<string>(); //issue when reloading scene where 1 event has been passed, the other event takes the place of that event and overwrites the previous
}

public class SaveToJson : MonoBehaviour
{
    public static EventData eventData = new EventData();

    private string eventsFilePath;

    public Button button;
    private int recordID;
    private int playerScore;
    public bool state;
    public string state_desc;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(HandleButtonClick);

        System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/json saves");
        System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/json loader");
    }

    private static int recordCount = 0;

    public void HandleButtonClick() //demo for tmr
    {
        PlayerData playerData = new PlayerData
        {
            playerScore = playerScore,
            state = state,
            recordID = recordCount,
            date = System.DateTime.Now.ToString(),
            state_desc = state_desc
        };

        string json = JsonUtility.ToJson(playerData);
        string fileName = string.Format("{0}_{1}.json", Application.persistentDataPath + "/json saves/player-score", playerData.recordID);
        File.WriteAllText(fileName, json);
        Debug.Log("Record:" + fileName + " saved");
        recordCount++;
    }
    public void UpdatePlayerScore(int newScore)
    {
        playerScore = newScore;
    }
    public void Description(string state)
    {
        state_desc = state;
    }

    public void AddEvent(string eventName) //save states - training + testing
    {
        eventData.events.Add(eventName);
        Debug.Log("Saving as: " + eventName);
        string json = JsonUtility.ToJson(eventData);
        File.WriteAllText(Application.persistentDataPath + "/json loader/events.json", json);
    }





}