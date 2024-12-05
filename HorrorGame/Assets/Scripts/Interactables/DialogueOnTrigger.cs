using UnityEngine;

public class DialogueOnTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player") && !DialogueManager.Instance.IsTutorialActive())
        {
            DialogueManager.Instance.StartTutorial(dialogue);
            hasTriggered = true;
        }
    }
}