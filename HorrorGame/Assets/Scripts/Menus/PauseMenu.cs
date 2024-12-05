using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject inventoryUI;
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public GameObject controllsMenuUI;
    public UnityEngine.UI.Button[] pauseMenuButtons;

    private AudioManager audioManager;
    private PlayerController playerController;
    private PlayerInteract playerInteract;
    private PlayerInteractUI playerInteractUI;
    private Flashlight flashlight;
    private GameObject flashlightHolder;
    private GameObject inventory;

    private void Start()
    {
        audioManager = AudioManager.Instance;
        playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
        playerInteract = GameManager.Instance.Player.GetComponent <PlayerInteract>();
        playerInteractUI = GameManager.Instance.Player.GetComponent<PlayerInteractUI>();
        flashlightHolder = GameManager.Instance.FlashlightHolder;
        flashlight = GameManager.Instance.FlashlightHolder.GetComponent<Flashlight>();
        inventory = GameManager.Instance.Inventory;

        // Ensure the game is not paused initially
        Resume();

        // Register for sceneLoaded event to handle pause state when switching scenes
        SceneManager.sceneLoaded += OnSceneLoaded;

        foreach (var button in pauseMenuButtons)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void OnDestroy()
    {
        // Unregister to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset pause state when a new scene is loaded
        Resume();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !GameManager.Instance.DialogueManager.IsDialogueActive() && !TrialEndMenu.trialEnded && !PlayerHealth.PlayerDied)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (flashlight.on && flashlight.batteryDrainCoroutine == null)
        {
            flashlight.batteryDrainCoroutine = StartCoroutine(flashlight.DrainBattery());
        }
        flashlightHolder.SetActive(true);
        inventoryUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        controllsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        playerController.enabled = true;
        playerController.mouseLook.SetCursorLock(true);
        playerInteract.enabled = true;
        playerInteractUI.enabled = true;
        audioManager.SetMusicPitch(1f);
        audioManager.ResumeAllSFX();
        GameManager.Instance.ResumeAllEnemies();
        inventory.SetActive(true);
    }

    void Pause()
    {
        if (flashlight.batteryDrainCoroutine != null)
        {
            StopCoroutine(flashlight.batteryDrainCoroutine);
            flashlight.batteryDrainCoroutine = null;
        }
        flashlightHolder.SetActive(false);
        inventoryUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        playerController.enabled = false;
        playerController.mouseLook.SetCursorLock(false);
        playerInteract.enabled = false;
        playerInteractUI.enabled = false;
        audioManager.SetMusicPitch(0.5f);
        audioManager.PauseAllSFX();
        GameManager.Instance.PauseAllEnemies();
        inventory.SetActive(false);
    }

    public void LoadMenu()
    {
        audioManager.SetMusicPitch(1f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayClickSound()
    {
        audioManager.PlayClickSound();
    }
}