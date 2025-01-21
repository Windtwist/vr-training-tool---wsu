using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    public CubeSpawner cubeSpawner; //spawner for our throwing up simulation

    private bool moved;
    private bool back;
    private bool finished;
    private bool dead;
    [SerializeField] private AudioClip death;
    public AudioSource audioSource; // audio source

    // Start is called before the first frame update
    void Start()//called on scene start
    {
        dead = false;
        animator = GetComponent<Animator>();
        animator.SetBool("Sitting", true); //setting first animation parameter to sitting
        //subscribing to all events
        HeimlichEvents.onPatientMoved += Walking;
        HeimlichEvents.onHeimlichStarted += TriggerBack;
        HeimlichEvents.onTestingTimeout += TriggerDeath;
    }
    void PlayClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
    private void Update()
    {
        if (finished)
        {
            //unsub from all
        }
        if (moved)
        {
            HeimlichEvents.onPatientMoved -= Walking;
        }
        if (back)
            HeimlichEvents.onHeimlichStarted -= TriggerBack;
        if(dead)
        {
            HeimlichEvents.onTestingTimeout -= TriggerDeath;
            HeimlichEvents.onPatientMoved -= Walking;
            HeimlichEvents.onHeimlichStarted -= TriggerBack;
        }
    }

        public void Walking()//sets second animation which makes the npc stand up and walk
        {
        animator.SetBool("Sitting", false);
        animator.SetBool("Walking", true);
        moved = true;
        }

    
    public void Unsub()
    {
        HeimlichEvents.onPatientMoved -= Walking;
        HeimlichEvents.onHeimlichStarted -= TriggerBack;
        HeimlichEvents.onTestingTimeout -= TriggerDeath;
    }

    public void TriggerBack()
    {
        animator.SetTrigger("Heimlich");

        SpawnDelayedCube();//after hemlich is done we call to spawn a prefab
        back = true;

    }

   
    public void TriggerDeath() //if countdown reaches 0
    {
        animator.SetTrigger("Die");
        dead = true;
        PlayClip(death);
    }

    private void SpawnDelayedCube()
    {
        Vector3 spawnPosition = new Vector3(-2f, 1.15f, 0f);
        cubeSpawner.SpawnCube(spawnPosition);
        cubeSpawner.SpawnCube(spawnPosition);
        cubeSpawner.SpawnCube(spawnPosition);
        cubeSpawner.SpawnCube(spawnPosition);
        cubeSpawner.SpawnCube(spawnPosition);
        cubeSpawner.SpawnCube(spawnPosition);
        cubeSpawner.SpawnCube(spawnPosition);


        HeimlichEvents.TriggerTrainingCompletion();//this will trigger training completion
        //unsub from all events
        HeimlichEvents.onPatientMoved -= Walking;
        HeimlichEvents.onHeimlichStarted -= TriggerBack;
        HeimlichEvents.onTestingTimeout -= TriggerDeath;
        finished = true;

    }


}
