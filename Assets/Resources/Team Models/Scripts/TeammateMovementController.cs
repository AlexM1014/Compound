using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeammateMovementController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [SerializeField]
    private BaseDoor[] targets;

    private Coroutine movementRoutine;

    [SerializeField]
    private float movementInterval = 2.5f;
    private float movementTimer;

    private float movementPause = 0f;
    private float animInterval = 1f;

    [SerializeField]
    private float rotateSpeed = 90f;

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
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            animator.SetTrigger("Kick");
            movementPause = animInterval;
        }
    }

    private void HandleDestinations()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveToDoor(ReturnRandomDoor());
        }

/*        movementTimer -= Time.deltaTime;
        if (movementTimer < 0)
        {
            movementTimer = movementInterval;
            MoveToDoor(ReturnRandomDoor());
        }*/
    }

    private BaseDoor ReturnRandomDoor()
    {
        if (targets.Length == 0) throw new UnassignedReferenceException("No Doors assigned to teammember");

        return targets[Random.Range(0, targets.Length)];
    }

    private void MoveToDoor(BaseDoor baseDoor)
    {
        BaseDoor doorComponent = baseDoor.GetComponent<BaseDoor>();
        GameObject targetTransform = doorComponent.movementTarget;
        MoveToDestination(targetTransform);
    } 

    private Coroutine MoveToDestination(GameObject target)
    {
        if (movementRoutine != null)
        {
            StopCoroutine(movementRoutine);
        }

        return movementRoutine = StartCoroutine(PerformMoveToTransform(target));
    }

    private IEnumerator PerformMoveToTransform(GameObject target)
    {
        agent.isStopped = false;
        Transform targetTransform = target.transform;
        agent.SetDestination(targetTransform.position);

        //Wait a frame for remaningDistance to be generated
        yield return null;

        // Wait until at destination
        Debug.Log("Walking towards destination");
        while (agent.remainingDistance > 0.1f) { yield return null; }
        agent.isStopped = true;

        // Rotate towards direction of movementTarget
        Debug.Log("Arrived at destination, rotating...");
        float angle;
        Quaternion toRotation;
        do
        {
            toRotation = targetTransform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotateSpeed * Time.deltaTime);
            yield return null;

            angle = Quaternion.Angle(toRotation, transform.rotation);
        } while (System.Math.Abs(angle) > 0.2);
        Debug.Log("Rotating stopped!");
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
