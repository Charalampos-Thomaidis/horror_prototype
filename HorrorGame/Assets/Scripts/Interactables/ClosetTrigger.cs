using UnityEngine;

public class ClosetTrigger : MonoBehaviour
{
    public Closet closet;

    private void OnTriggerEnter(Collider other)
    {
        closet.OnChildTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        closet.OnChildTriggerExit(other);
    }
}