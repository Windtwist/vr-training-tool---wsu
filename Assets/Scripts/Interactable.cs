using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    public string EventName;
    private bool hasInteracted;
    private List<string> events;

    private SaveToJson saveToJson;
    private MeshRenderer meshRenderer;
    private Collider collider; //make object non-interactable now it just changes color

    private void Start()
    {
        saveToJson = new SaveToJson();
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
        events = new List<string>();

        // Read the JSON file and add all events to the list
        string path = Application.persistentDataPath + "/json loader/events.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            EventData eventData = JsonUtility.FromJson<EventData>(json);
            foreach (string eventName in eventData.events)
            {
                Debug.Log(eventName);
                // Check if the current event has happened
                if (eventName == EventName)
                {
                    Debug.Log(EventName + " is not interactable and matches " + eventName);
                    GetComponent<Renderer>().material.color = Color.red;
                    hasInteracted = true;
                }
            }
        }

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("hand") && !hasInteracted)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;

            string eventName = EventName;
            Debug.Log("Interacted with " + eventName);

            saveToJson.AddEvent(eventName);

            // Change the color of the object
            meshRenderer.material.color = Color.red;

        }
    }
}