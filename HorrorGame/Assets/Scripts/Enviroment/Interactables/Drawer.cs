using UnityEngine;

public class Drawer : Interactable
{
    private bool interactingWithDrawer;
    private float interactionCooldown = 0.5f;
    private float lastInteractionTime = -1f;
    private Animator animDrawer;

    private void Start()
    {
        animDrawer = GetComponent<Animator>();
    }

    protected override void Interact()
    {
        if (Time.time < lastInteractionTime + interactionCooldown)
        {
            return;
        }

        lastInteractionTime = Time.time;
        interactingWithDrawer = !interactingWithDrawer;
        animDrawer.SetBool("openShelf", interactingWithDrawer);

        if (animDrawer.GetBool("openShelf"))
        {
            AudioManager.Instance.PlayOpenDrawerSound();
        }
        else
        {
            AudioManager.Instance.PlayCloseDrawerSound();
        }
    }
}