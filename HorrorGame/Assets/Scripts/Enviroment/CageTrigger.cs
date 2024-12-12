using UnityEngine;

public class CageTrigger : MonoBehaviour
{
    private Animator cageAnim;

    void Start()
    {
        cageAnim = GetComponent<Animator>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cageAnim.SetBool("isOpen", true);
        }
    }

    // Called by animation event
    public void PlayCageSound()
    {
        AudioManager.Instance.PlayImpactMetalSound();
    }
}