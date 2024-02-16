using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Blacklight : MonoBehaviour
{
    MeshRenderer meshRenderer;

    bool isFadingIn;
    bool isFadingOut;

    public float timeElapsed;
    public float lerpDuration = 0.5f;

    IEnumerator Reveal(Collider other)
    {
        isFadingIn = true;
        if (timeElapsed > lerpDuration) timeElapsed = 0;

        meshRenderer = other.gameObject.GetComponent<MeshRenderer>();
        meshRenderer.enabled = true;

        while (timeElapsed < lerpDuration && isFadingIn)
        {
            timeElapsed += Time.deltaTime;
            meshRenderer.material.SetFloat("_TestAlphaLerp", Mathf.Lerp(1, 0, timeElapsed / lerpDuration));

            Debug.Log("Fading in - " + timeElapsed);

            yield return null;
        }

        isFadingIn = false;
    }

    IEnumerator Fade(Collider other)
    {
        isFadingOut = true;
        if (timeElapsed > lerpDuration) timeElapsed = 0;

        meshRenderer = other.gameObject.GetComponent<MeshRenderer>();

        while (timeElapsed < lerpDuration && isFadingOut)
        {
            timeElapsed += Time.deltaTime;
            meshRenderer.material.SetFloat("_TestAlphaLerp", Mathf.Lerp(0, 1, timeElapsed / lerpDuration));

            Debug.Log("Fading out - " + timeElapsed);

            yield return null;
        }

        isFadingOut = false;
        meshRenderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            if (other.gameObject.CompareTag("TestHiddenTexture"))
            {
                isFadingOut = false;
                StartCoroutine(Reveal(other));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            if (other.gameObject.CompareTag("TestHiddenTexture"))
            {
                isFadingIn = false;
                StartCoroutine(Fade(other));
            }
        }
    }
}
