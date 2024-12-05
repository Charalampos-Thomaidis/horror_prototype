using System.Collections;
using System;
using UnityEngine;

[Serializable]
public class FOVKick 
{
    public Camera cam;
    [HideInInspector]
    public float originalFOV;
    public float FOV_increase = 3f;
    public float timeToIncrease = 1f;
    public float timeToDecrease = 1f;
    public AnimationCurve increaseCurve;

    public void Setup(Camera camera)
    {
        cam = camera;
        originalFOV = cam.fieldOfView;
    }

    public IEnumerator FOVKickUp()
    {
        float t = 0f;
        while (t < timeToIncrease)
        {
            cam.fieldOfView = originalFOV + (increaseCurve.Evaluate(t / timeToIncrease) * FOV_increase);
            t += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = originalFOV + FOV_increase;
    }

    public IEnumerator FOVKickDown()
    {
        float t = 0f;
        float startFOV = cam.fieldOfView;
        while (t < timeToDecrease) 
        {
            cam.fieldOfView = startFOV - (increaseCurve.Evaluate(t / timeToDecrease) * FOV_increase);
            t += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = originalFOV;
    }
}