using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;
    // Start is called before the first frame update

    private void Start()
    {
        EventHandler.OnCorrectAction += TriggerHeadNod;
        EventHandler.OnIncorrectAction += TriggerHeadShake;
    }

    private void OnDestroy()
    {
        EventHandler.OnCorrectAction -= TriggerHeadNod;
        EventHandler.OnIncorrectAction -= TriggerHeadShake;
    }

    public void TriggerHeadNod()
    {
        animator.SetTrigger("ActivateHeadNod");
    }

    public void TriggerHeadShake()
    {
        animator.SetTrigger("ActivateHeadShake");
    }
}
