using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light flickeringLight;
    public float flickerInterval = 0.1f;
    public float flickerDuration = 5f;
    private bool isFlickering = false;
    private float flickerTimer = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            if (!isFlickering)
            {
                isFlickering = true;
                flickerTimer = flickerDuration;
                StartCoroutine(FlickerLight());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            if (isFlickering)
            {
                isFlickering = false;
                flickeringLight.enabled = true;
                StopCoroutine(FlickerLight());
            }
        }
    }

    private IEnumerator FlickerLight()
    {
        while (isFlickering)
        {
            flickeringLight.enabled = !flickeringLight.enabled;
            yield return new WaitForSeconds(flickerInterval);

            flickerTimer -= flickerInterval;
            if (flickerTimer <= 0f)
            {
                isFlickering = false;
                flickeringLight.enabled = true;
            }
        }
    }
     
    // Called by animation event
    public void PlayElevatorSound()
    {
        AudioManager.Instance.PlayElevatorSound();
    }
}