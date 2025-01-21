using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SceneController : MonoBehaviour
{
    [SerializeField] List<GameObject> gameObjects = new List<GameObject>();
    private int objectIndex = 0;
    // Start is called before the first frame update

    void Start()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.GetComponent<XRGrabInteractable>().enabled = false; 
        }
        gameObjects[objectIndex].GetComponent<XRGrabInteractable>().enabled = true;
        gameObjects[objectIndex].GetComponent<XRGrabInteractable>().selectEntered.AddListener(GrabAction);


    }

    void GrabAction(SelectEnterEventArgs selectEnterEventArgs)
    {
        Debug.Log(gameObjects[objectIndex].name + "interacted with");
        objectIndex++;
        if (objectIndex >= gameObjects.Count) objectIndex = 0;
        gameObjects[objectIndex].GetComponent<XRGrabInteractable>().enabled = true;
        gameObjects[objectIndex].GetComponent<XRGrabInteractable>().selectEntered.AddListener(GrabAction);

    }
}
