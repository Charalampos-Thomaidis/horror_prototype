using System;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public Camera cam;
    public CurveControlledBob motionBob = new CurveControlledBob();
    public float StrideInterval;
    [Range(0f, 1f)]
    public float RuningStrideLengthen;

    private Vector3 originalCameraPosition;

    private void Start()
    {
        motionBob.Setup(cam, StrideInterval);
        originalCameraPosition = cam.transform.localPosition;
    }
}