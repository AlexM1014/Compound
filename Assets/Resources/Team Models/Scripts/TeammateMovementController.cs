using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeammateMovementController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [SerializeField]
    private GameObject[] targets;

    [SerializeField]
    private float movementInterval = 2.5f;
    private float movementTimer;

    private float movementPause = 0f;
    private float animInterval = 1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        Debug.Log(animator);

        movementTimer = 2f;
    }

    void Update()
    {
        HandleDestinations();
        HandleAnims();
    }

    private void HandleAnims()
    {
        movementPause -= Time.deltaTime;
        animator.SetFloat("Velocity", agent.velocity.magnitude);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Kick");
            agent.isStopped = true;
            movementPause = animInterval;
        }

        if (movementPause < 0f)
        {
            agent.isStopped = false;
        }
    }

    private void HandleDestinations()
    {
        movementTimer -= Time.deltaTime;
        if (movementTimer < 0)
        {
            movementTimer = movementInterval;
            SetNewDestination();
        }
    }

    private void SetNewDestination()
    {
        if (targets.Length == 0)
        {
            Debug.LogWarning("Agent has no location to move to");
            return;
        }

        int randSpot = Random.Range(0, targets.Length);
        agent.SetDestination(targets[randSpot].transform.position);
    }
}
