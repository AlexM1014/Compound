using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDoor : MonoBehaviour
{
    [SerializeField]
    public GameObject movementTarget;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayOpenDoor()
    {
        //animator?.SetTrigger("Open");
    }


}
