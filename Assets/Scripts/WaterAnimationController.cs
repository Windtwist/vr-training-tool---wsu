using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAnimationController : MonoBehaviour
{
    [SerializeField] GameObject handle1;
    [SerializeField] GameObject handle2;
    [SerializeField] GameObject waterParticleSystem;
    private bool handle1active = false;
    private bool handle2active = false;

    public void ToggleHandle1()
    {
        handle1active = !handle1active;
    }

    public void ToggleHandle2()
    {
        handle2active = !handle2active;
    }

    public void UpdateWater()
    {
        if (!handle1active && !handle2active)
        {
            waterParticleSystem.SetActive(false);
        }
        else
        {
            waterParticleSystem.SetActive(true);
        }
    }
}
