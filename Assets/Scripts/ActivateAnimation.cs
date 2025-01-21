using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform currentRotation;
    // Start is called before the first frame update
    public void ObjectHover()
    {
        animator.SetTrigger("Active");
    }

    public void ObjectStopHover()
    {
        animator.SetTrigger("Idle");

    }
}
