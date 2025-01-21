using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonVR_Unused : MonoBehaviour
{
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private float deadzone = 0.025f;

    public UnityEvent OnPress;
    public UnityEvent OnRelease;
    private bool isPress;
    private Vector3 start_pos;
    private ConfigurableJoint _joint;
    private int count = 0;

    void Start()
    {
        start_pos = transform.localPosition;
        _joint = GetComponent<ConfigurableJoint>();
    }


    private void Update()
    {
        if (!isPress && GetValue() + threshold >= 1)
            Press();
        if (!isPress && GetValue() + threshold <= 0)
            Release();
    }

    private float GetValue()
    {
        var value = Vector3.Distance(start_pos, transform.localPosition) / _joint.linearLimit.limit;

        if (Mathf.Abs(value) < deadzone)
            value = 0;

        return Mathf.Clamp(value, -1f, 1f);
    }

    private void Press()
    {
        isPress = true;
        OnPress.Invoke();
        Debug.Log("press");
        count += 1;
        Debug.Log(count);
    }

    private void Release()
    {
        isPress = false;
        OnRelease.Invoke();
        Debug.Log("Release");
        
    }

    public void spawnSphere()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        sphere.transform.localPosition = new Vector3(0, 1, -0.5f);
        sphere.AddComponent<Rigidbody>();

    }

}
