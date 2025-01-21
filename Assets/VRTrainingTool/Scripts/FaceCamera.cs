using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera camera;
    private Transform cameraTransform;

    private void Start()
    {
        camera = Camera.main;
        cameraTransform = camera.transform;
    }

    private void LateUpdate()
    {
        if (camera.orthographic) return;

        //good for 2d
        //transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up);
        var lookAt = new Vector3(camera.transform.position.x / 50, transform.position.y, camera.transform.position.z * 180);
        transform.LookAt(lookAt);
    }


}
