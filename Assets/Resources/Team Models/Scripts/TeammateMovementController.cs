using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class TeammateMovementController : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private Animator animator;

    [SerializeField]
    private BaseDoor[] targets;

    private Coroutine movementRoutine;

    private BaseDoor currentDoor;
    private bool isStandbyDoor = false;

    [SerializeField]
    private float movementInterval = 2.5f;

    [SerializeField]
    private float rotateSpeed = 90f;

    [SerializeField]
    private bool allowTestingControls = false;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        Debug.Log(animator);
    }

    void Update()
    {
        HandleAnims();
        if (allowTestingControls) HandleTestingControls();
    }

    /// <summary>
    /// Function to update animation variables
    /// </summary>
    private void HandleAnims()
    {
        animator.SetFloat("Velocity", navAgent.velocity.magnitude);
    }

    /// <summary>
    /// Test function to listen for input to call certain actions
    /// </summary>
    private void HandleTestingControls()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveToDoor(ReturnRandomDoor());
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OpenDoor(currentDoor);
        }
    }

    /// <summary>
    /// Test function for returning a random door from a test list
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UnassignedReferenceException"></exception>
    private BaseDoor ReturnRandomDoor()
    {
        if (targets.Length == 0) throw new UnassignedReferenceException("No Doors assigned to teammember");

        return targets[UnityEngine.Random.Range(0, targets.Length)];
    }

    /// <summary>
    /// Opens door passed in, will only open if matches current door
    /// </summary>
    /// <param name="baseDoor"></param>
    public void OpenDoor(BaseDoor baseDoor)
    {
        // TODO Add animation trigger to tell door to open when animation plays
        if (CanOpenDoor(baseDoor))
        {
            animator.SetTrigger("Kick");
        }
    }

    private bool CanOpenDoor(BaseDoor baseDoor)
    {
        if (baseDoor == null)
        {
            Debug.LogError("Not at a door, skipping kick animation");
            return false;
        }

        if (baseDoor != currentDoor)
        {
            Debug.LogError("Door does not match current door, skipping kick animation");
            return false;
        }

        if (movementRoutine != null)
        {
            Debug.LogError("Teammate is moving, skipping kick animation");
            return false;
        }

        if (!IsStandbyDoor())
        {
            Debug.LogError("Teammate is not in position, skipping kick animation");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Animation event function to play door animation when trigger hits
    /// </summary>
    public void PlayDoorOpen()
    {
        // currentDoor.PlayAnim()
        Debug.Log("Door anim plays here");
    }

    /// <summary>
    /// Moves the teammate towards passed in door
    /// </summary>
    /// <param name="baseDoor"></param>
    public void MoveToDoor(BaseDoor baseDoor)
    {
        BaseDoor doorComponent = baseDoor.GetComponent<BaseDoor>();
        currentDoor = doorComponent;
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
        navAgent.isStopped = false;
        Transform targetTransform = target.transform;
        navAgent.SetDestination(targetTransform.position);

        //Wait a frame for remaningDistance to be generated
        yield return null;

        // Wait until at destination
        Debug.Log("Walking towards destination");
        while (navAgent.remainingDistance > 0.1f) { yield return null; }
        navAgent.isStopped = true;

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

        // Remove coroutine as its over
        movementRoutine = null;
    }

    private bool IsStandbyDoor()
    {
        //Check if agent is at target location for door
        //Debug.Log(navAgent.remainingDistance);
        return (navAgent.remainingDistance < 0.2f);
    }
}
