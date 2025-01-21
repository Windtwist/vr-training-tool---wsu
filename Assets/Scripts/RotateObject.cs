using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] GameObject objectToRotate;
    [SerializeField] Transform aroundPoint;
    [SerializeField] float angle;
    [SerializeField] float time;
    bool isActive = false;
    Vector3 originalPosition;
    Quaternion originalRotation;

    void Start() 
    {
        originalPosition = objectToRotate.transform.position;
        originalRotation = objectToRotate.transform.rotation;
    }

    public void ActivateRotate()
    {
        StartCoroutine(_rotate(angle));
    }

    public void ReturnToOriginal()
    {
        objectToRotate.transform.position = originalPosition;
        objectToRotate.transform.rotation = originalRotation;
        isActive = false;
    }

    public void ConditionalRotate()
    {
        if (isActive)
        {
            angle = -angle;
            ActivateRotate();
        } else
        {
            ActivateRotate();
        }
    }
    
    private IEnumerator _rotate(float angle)
    {
        float start = Time.time;
        float end = start + time;
        isActive = true;
        while (Time.time < end)
        {
            objectToRotate.transform.RotateAround(aroundPoint.position, Vector3.up, angle * Time.deltaTime / time);
            yield return null;
        }
    }
}
