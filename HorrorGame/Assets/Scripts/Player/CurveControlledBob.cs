using System;
using UnityEngine;

[Serializable]
public class CurveControlledBob
{
    public float h_BobRange = 0.33f;
    public float v_BobRange = 0.33f;

    public AnimationCurve BobCurve = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(0.5f,1f),
                                                        new Keyframe(1f,0f), new Keyframe(1.5f,-1f),
                                                        new Keyframe(2f,0f));
    public float VerticalHorizontalRatio = 1f;

    private float cyclePostionX;
    private float cyclePostionY;
    private float BobBaseInterval;
    private Vector3 originalCameraPosition;
    private float time;

    public void Setup(Camera camera, float bobBaseInerval)
    {
        BobBaseInterval = bobBaseInerval;
        originalCameraPosition = camera.transform.localPosition;

        time = BobCurve[BobCurve.length - 1].time;
    }

    public Vector3 DoHeadBob(float speed)
    {
        float xPos = originalCameraPosition.x + (BobCurve.Evaluate(cyclePostionX) * h_BobRange);
        float yPos = originalCameraPosition.y + (BobCurve.Evaluate(cyclePostionY) * v_BobRange);

        cyclePostionX += (speed * Time.deltaTime) / BobBaseInterval;
        cyclePostionY += ((speed * Time.deltaTime) / BobBaseInterval) * VerticalHorizontalRatio;

        if (cyclePostionX > time)
        {
            cyclePostionX = cyclePostionX - time;
        }
        if (cyclePostionY > time)
        {
            cyclePostionY = cyclePostionY - time;
        }

        return new Vector3(xPos, yPos, 0f);
    }
}