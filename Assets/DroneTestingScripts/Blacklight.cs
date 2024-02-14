using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Blacklight : MonoBehaviour
{
    public Shader colliderMat;
    public GameObject TestObject;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ChangeAlpha();
        }
    }

    void ChangeAlpha()
    {
        Material testMaterial = TestObject.GetComponent<MeshRenderer>().material;
        testMaterial.SetFloat("_TestColorAlpha", Mathf.Lerp(1, 100, Time.deltaTime));
        testMaterial.SetFloat("_TestObjectAlpha", Mathf.Lerp(1, 100, Time.deltaTime));
        Debug.Log("Change Alpha");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("TestHiddenTexture"))
            {
                colliderMat.GetComponent<MeshRenderer>().material.SetFloat("TestAlpha", Mathf.Lerp(1, 0, Time.deltaTime * 2));
            }
        }
    }
}
