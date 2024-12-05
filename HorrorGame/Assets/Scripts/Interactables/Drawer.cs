using UnityEngine;

public class Drawer : Interactable
{
    [SerializeField]
    private GameObject drawershelf;

    private bool interactingWithDrawer;
    private float interactionCooldown = 0.5f;
    private float lastInteractionTime = -1f;


    protected override void Interact()
    {
        if (Time.time < lastInteractionTime + interactionCooldown)
        {
            return;
        }

        lastInteractionTime = Time.time;
        interactingWithDrawer = !interactingWithDrawer;
        drawershelf.GetComponent<Animator>().SetBool("openShelf", interactingWithDrawer);

        if (drawershelf.GetComponent<Animator>().GetBool("openShelf"))
        {
            AudioManager.Instance.PlayOpenDrawerSound();
        }
        else
        {
            AudioManager.Instance.PlayCloseDrawerSound();
        }
    }
}
