using UnityEngine;

public class Note : Interactable
{
    [SerializeField]
    private GameObject note;
    [SerializeField]
    private GameObject textDisplay;
    [SerializeField]
    private Renderer noteMesh;

    private bool isInteracting = false;
    private GameObject player;
    private Inventory inventory;
    private PauseMenu pauseMenu;

    public void Start()
    {
        player = GameManager.Instance.Player;
        inventory = Inventory.Instance;
        pauseMenu = PauseMenu.Instance;
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && isInteracting)
        {
            HideText();
            pauseMenu.enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && !isInteracting)
        {
            pauseMenu.enabled = true;
        }
    }

    public void ShowText()
    {
        textDisplay.SetActive(true);
        isInteracting = true;
        player.GetComponent<PlayerController>().enabled = false;
        inventory.gameObject.SetActive(false);
        pauseMenu.enabled = false;
        noteMesh.enabled = false;
    }

    public void HideText()
    {
        textDisplay.SetActive(false);
        isInteracting = false;
        player.GetComponent<PlayerController>().enabled = true;
        inventory.gameObject.SetActive(true);
        noteMesh.enabled = true;
    }

    protected override void Interact()
    {
        if (!isInteracting)
        {
            ShowText();
        }
        else
        {
            HideText();
        }
    }
}