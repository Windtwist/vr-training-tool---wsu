using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CprCharAnimation : MonoBehaviour
{
    
    public Animator animator;
    public PressureBar pb;
    public HandButton hb;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        StartAnimation();
        //StopAnimation();
    }

    public void StartAnimation()
    {
        if (pb.fillAmount >= 0.90f && hb.pressCount > 0)
        {
            animator.SetTrigger("ActivateCPRAnim");
            //animator.SetBool("ActivateCPRAnim", true);
        }
    }
}