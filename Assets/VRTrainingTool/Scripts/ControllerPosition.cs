using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerPosition : MonoBehaviour
{
    public InputActionProperty positionProperty;
    public Vector3 Position { get; private set; } = Vector3.zero;

    private void Update()
    {
        Position = positionProperty.action.ReadValue<Vector3>();
        Debug.Log("Y position: " + Position.y);
    }
}
