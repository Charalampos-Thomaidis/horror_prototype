using UnityEngine;

public class FlashlightOffset : MonoBehaviour
{
    [SerializeField]
    private float speed = 3f;

    private Vector3 vectOffset;
    private GameObject followPlayer;

    void Start()
    {
        followPlayer = Camera.main.gameObject;
        vectOffset = transform.position - followPlayer.transform.position;
    }

    void Update()
    {
        transform.position = followPlayer.transform.position + vectOffset;
        transform.rotation = Quaternion.Slerp(transform.rotation, followPlayer.transform.rotation, speed * Time.deltaTime);
    }
}
