using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public DialogueManager DialogueManager { get; private set; }
    public TextMeshProUGUI NameText { get; private set; }
    public TextMeshProUGUI DialogueText { get; private set; }
    public TextMeshProUGUI TutorialText { get; private set; }
    public Animator DialogueAnimator { get; private set; }
    public Animator TutorialAnimator { get; private set; }
    public GameObject Player { get; private set; }
    public Inventory Inventory { get; private set; }
    public GameObject FlashlightHolder { get; private set; }
    public PostProcessVolume PostProcessVolume { get; private set; }
    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
    public bool IsInChase { get; set; }

    private bool isChaseMusicPlaying = false;
    private Flashlight flashlight;

    public enum GameState
    {
        Playing,
        Paused,
        Chase,
        Win,
        Loss
    }

    public GameState CurrentState { get; private set; } = GameState.Playing;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        CheckGameStatus();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            InitializeReferences();
            CurrentState = GameState.Playing;
        }
    }

    public void InitializeReferences()
    {
        Player = GameObject.FindWithTag("Player");
        FlashlightHolder = GameObject.Find("FlashlightHolder");
        flashlight = FlashlightHolder.GetComponent<Flashlight>();
        NameText = GameObject.Find("Name")?.GetComponent<TextMeshProUGUI>();
        DialogueText = GameObject.Find("Dialogue")?.GetComponent<TextMeshProUGUI>();
        TutorialText = GameObject.Find("TutorialText")?.GetComponent<TextMeshProUGUI>();
        DialogueAnimator = GameObject.Find("DialogueBox")?.GetComponent<Animator>();
        TutorialAnimator = GameObject.Find("TutorialBox")?.GetComponent<Animator>();
        PostProcessVolume = GameObject.Find("player")?.GetComponent<PostProcessVolume>();

        Inventory = Inventory.Instance;
        DialogueManager = DialogueManager.Instance;

        Enemies.Clear();
        Enemies.AddRange(FindObjectsOfType<Enemy>());

        DialogueManager?.Initialize();
    }

    public void PauseAllEnemiesSound()
    {
        foreach (Enemy enemy in Enemies)
        {
            enemy.PauseEatSound();
        }
    }

    public void ResumeAllEnemiesSound()
    {
        foreach (Enemy enemy in Enemies)
        {
            enemy.ResumeEatSound();
        }
    }

    public void HandleChase()
    {
        CurrentState = GameState.Chase;

        if (IsInChase)
        {
            AudioManager.Instance.PlayChaseMusic();
            isChaseMusicPlaying = true;
        }
    }

    public void HandleLoss()
    {
        CurrentState = GameState.Loss;

        Player.GetComponent<PlayerController>().enabled = false;
        Player.GetComponent<PlayerInteract>().enabled = false;
        Player.GetComponent<PlayerUI>().DisableText();
        FlashlightHolder.GetComponent<Flashlight>().enabled = false;
        Player.GetComponent<PlayerController>().mouseLook.SetCursorLock(false);
        Inventory.gameObject.SetActive(false);
    }

    public void HandleWin()
    {
        CurrentState = GameState.Win;

        AudioManager.Instance.PlayBackgroundMusic();
        Player.GetComponent<PlayerController>().enabled = false;
        FlashlightHolder.GetComponent<Flashlight>().enabled = false;
        Player.GetComponent<PlayerController>().mouseLook.SetCursorLock(false);
    }
    
    public void PauseGame()
    {
        if (CurrentState == GameState.Playing || CurrentState == GameState.Chase)
        {
            CurrentState = GameState.Paused;

            if (flashlight.batteryDrainCoroutine != null)
            {
                StopCoroutine(flashlight.batteryDrainCoroutine);
                flashlight.batteryDrainCoroutine = null;
            }
            FlashlightHolder.SetActive(false);
            Player.GetComponent<PlayerController>().enabled = false;
            Player.GetComponent<PlayerController>().mouseLook.SetCursorLock(false);
            Player.GetComponent<PlayerInteract>().enabled = false;
            Player.GetComponent<PlayerInteractUI>().enabled = false;
            AudioManager.Instance.SetMusicPitch(0.5f);
            AudioManager.Instance.PauseAllSFX();
            PauseAllEnemiesSound();
            Inventory.gameObject.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = IsInChase ? GameState.Chase : GameState.Playing;

            if (flashlight.on && flashlight.batteryDrainCoroutine == null)
            {
                flashlight.batteryDrainCoroutine = StartCoroutine(flashlight.DrainBattery());
            }
            FlashlightHolder.SetActive(true);
            Player.GetComponent<PlayerController>().enabled = true;
            Player.GetComponent<PlayerController>().mouseLook.SetCursorLock(true);
            Player.GetComponent<PlayerInteract>().enabled = true;
            Player.GetComponent<PlayerInteractUI>().enabled = true;
            AudioManager.Instance.SetMusicPitch(1f);
            AudioManager.Instance.ResumeAllSFX();
            ResumeAllEnemiesSound();
            Inventory.gameObject.SetActive(true);
        }
    }
    
    public void CheckGameStatus()
    {
        if (PlayerHealth.PlayerDied)
        {
            HandleLoss();
        }
        else if (TrialEndMenu.trialEnded)
        {
            HandleWin();
        }

        if (PauseMenu.GameIsPaused)
        {
            if (CurrentState != GameState.Paused)
            {
                PauseGame();
            }
        }
        else
        {
            if (CurrentState == GameState.Paused)
            {
                ResumeGame();
            }
        }

        if (CurrentState == GameState.Playing)
        {
            bool isEnemyChasing = Enemies.Exists(enemy => enemy.IsChasing);

            if (isEnemyChasing)
            {
                HandleChase();
            }
        }
        else if (CurrentState == GameState.Chase)
        {
            bool isEnemyChasing = Enemies.Exists(enemy => enemy.IsChasing);

            if (!isEnemyChasing)
            {
                CurrentState = GameState.Playing;
                AudioManager.Instance.PlayBackgroundMusic();
            }
        }
    }
}