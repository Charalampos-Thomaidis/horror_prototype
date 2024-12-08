using UnityEngine;

public class npcDialogue : Interactable
{
    public Transform npc;
    public Transform player;
    public Dialogue dialogue;

    private float rotationSpeed = 5f;

    private void Update()
    {
        if (DialogueManager.Instance.IsDialogueActive())
        {
            TurnTowardsPlayer();
        }
        else
        {
            npc.rotation = Quaternion.Euler(0f, 70f, 0f);
        }
    }
    
    protected override void Interact()
    {
        if (!DialogueManager.Instance.IsDialogueActive())
        {
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }

    private void TurnTowardsPlayer()
    {
        Vector3 direction = player.position - npc.position;

        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}