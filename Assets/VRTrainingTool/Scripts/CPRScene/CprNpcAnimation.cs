using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CprNpcAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        CprEventHandler.onCorrectAction += TriggerCorrectActionAnim;
        CprEventHandler.onIncorrectAction += TriggerIncorrectActionAnim;

    }

    private void TriggerCorrectActionAnim()
    {
        animator.SetTrigger("CorrectAction");
    }

    private void TriggerIncorrectActionAnim()
    {
        animator.SetTrigger("IncorrectAction");
    }

    private void OnDestroy()
    {
        CprEventHandler.onCorrectAction -= TriggerCorrectActionAnim;
        CprEventHandler.onIncorrectAction -= TriggerIncorrectActionAnim;
    }
}
