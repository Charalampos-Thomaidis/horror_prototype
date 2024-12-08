using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera m_Camera;

    [SerializeField]
    private float distance = 3f;
    [SerializeField]
    private LayerMask mask;

    private float holdTime = 0f;
    private PlayerUI playerUI;
    private DialogueManager dialogueManager;
    private Interactable currentInteractable;

    void Start()
    {
        m_Camera = Camera.main;
        playerUI = GetComponent<PlayerUI>();
        playerUI.SetCrosshairImageActive(true);
        dialogueManager = DialogueManager.Instance;
    }

    void Update()
    {
        if (dialogueManager.IsDialogueActive())
        {
            if (Input.GetButtonDown("Interact"))
            {
                dialogueManager.DisplayNextSentence();
            }
            return;
        }

        playerUI.UpdateText(string.Empty);

        Ray ray = new Ray(m_Camera.transform.position, m_Camera.transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessege);
                playerUI.SetCrosshairImageActive(false);

                if (interactable != currentInteractable)
                {
                    holdTime = 0f;
                    playerUI.UpdateSlider(0);
                }

                currentInteractable = interactable;

                if (interactable.holdInteraction)
                {
                    HandleHoldInteraction(interactable, interactable.requiredHoldTime);
                }
                else
                {
                    HandlePressInteraction(interactable);
                }
            }
        }
        else
        {
            if (currentInteractable != null)
            {
                holdTime = 0f;
                playerUI.UpdateSlider(0);
                playerUI.SetSliderActive(false);
                currentInteractable = null;
            }

            playerUI.SetCrosshairImageActive(true);
        }
    }

    private void HandlePressInteraction(Interactable interactable)
    {
        if (Input.GetButtonDown("Interact"))
        {
            interactable.BaseInteract();
        }
    }

    private void HandleHoldInteraction(Interactable interactable, float requiredHoldTime)
    {
        if (Input.GetButton("Interact"))
        {
            holdTime += Time.deltaTime;

            if (holdTime == Time.deltaTime && interactable is Corpse corpse)
            {
                corpse.StartInteraction();
            }

            float progress = Mathf.Clamp01(holdTime / interactable.requiredHoldTime);
            playerUI.SetSliderActive(true);
            playerUI.UpdateSlider(progress);

            if (holdTime >= requiredHoldTime)
            {
                interactable.BaseInteract();
                holdTime = 0f;
                playerUI.UpdateSlider(0);
                playerUI.SetSliderActive(false);

                if (interactable is Corpse interactingCorpse)
                {
                    interactingCorpse.CancelInteraction();
                }
            }
        }
        else if (Input.GetButtonUp("Interact"))
        {
            holdTime = 0f;
            playerUI.UpdateSlider(0);
            playerUI.SetSliderActive(false);

            if (interactable is Corpse corpse)
            {
                corpse.CancelInteraction();
            }
        }
    }
}