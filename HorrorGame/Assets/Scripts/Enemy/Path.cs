using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    [SerializeField]
    public bool alwaysDrawPath;
    [SerializeField]
    public bool drawAsLoop;
    public Color debugColour = Color.white;

    public void OnDrawGizmos()
    {
        if (alwaysDrawPath)
        {
            DrawPath();
        }
    }

    public void DrawPath()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (i >= 1)
            {
                Gizmos.color = debugColour;
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);

                if (drawAsLoop)
                    Gizmos.DrawLine(waypoints[waypoints.Count - 1].position, waypoints[0].position);
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (!alwaysDrawPath)
        {
            DrawPath();
        }
    }
}