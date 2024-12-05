using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera m_Camera;

    [SerializeField]
    private float distance = 3f;
    [SerializeField]
    private LayerMask mask;

    private PlayerUI playerUI;
    private DialogueManager dialogueManager;

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

                if (Input.GetButtonDown("Interact"))
                {
                    interactable.BaseInteract();
                }
            }
        }
        else
        {
            playerUI.SetCrosshairImageActive(true);
        }
    }
}