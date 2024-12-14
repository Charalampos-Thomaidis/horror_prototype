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
    private PauseMenu pauseMenu;

    public void Start()
    {
        player = GameManager.Instance.Player;
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
        AudioManager.Instance.PlayNotePickSound();
        textDisplay.SetActive(true);
        isInteracting = true;
        player.GetComponent<PlayerController>().enabled = false;
        pauseMenu.enabled = false;
        noteMesh.enabled = false;
    }

    public void HideText()
    {
        textDisplay.SetActive(false);
        isInteracting = false;
        player.GetComponent<PlayerController>().enabled = true;
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