using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private string currentState; // for debug purposes

    public static GameManager Instance { get; private set; }

    public Flashlight Flashlight { get; private set; }
    public GameObject Player { get; private set; }
    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
    public bool IsInChase { get; set; }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Chase,
        Win,
        Loss
    }

    public GameState CurrentState { get; private set; }

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

        currentState = CurrentState.ToString();
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
            InitializeReferences();
            CurrentState = GameState.MainMenu;
            AudioManager.Instance.StopAllMusic();
            AudioManager.Instance.PlayMainMenuMusic();
        }
        else
        {
            InitializeReferences();
            CurrentState = GameState.Playing;
            AudioManager.Instance.StopAllMusic();
            AudioManager.Instance.PlayBackgroundMusic();
        } 
    }

    public void InitializeReferences()
    {
        Enemies.Clear();
        Enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy").Select(obj => obj.GetComponent<Enemy>()).Where(enemy => enemy != null));

        Player = GameObject.FindWithTag("Player");
        Flashlight = GameObject.Find("FlashlightHolder")?.GetComponent<Flashlight>();
    }

    public void PauseAllEnemiesSound()
    {
        foreach (Enemy enemy in Enemies)
        {
            enemy.PauseSound();
        }
    }

    public void ResumeAllEnemiesSound()
    {
        foreach (Enemy enemy in Enemies)
        {
            enemy.ResumeSound();
        }
    }

    public void HandleLoss()
    {
        CurrentState = GameState.Loss;

        if (Player.TryGetComponent(out PlayerController playerController))
        {
            playerController.enabled = false;
            playerController.mouseLook.SetCursorLock(false);
        }

        if (Player.TryGetComponent(out PlayerInteract playerInteract))
        {
            playerInteract.enabled = false;
        }

        if (Player.TryGetComponent(out PlayerUI playerUI))
        {
            playerUI.DisableText();
        }

        AudioManager.Instance.StopAllMusic();
    }

    public void HandleWin()
    {
        CurrentState = GameState.Win;

        AudioManager.Instance.PlayBackgroundMusic();
        if (Player.TryGetComponent(out PlayerController playerController))
        {
            playerController.enabled = false;
            playerController.mouseLook.SetCursorLock(false);
        }
    }
    
    public void PauseGame()
    {
        if (CurrentState == GameState.Playing || CurrentState == GameState.Chase)
        {
            CurrentState = GameState.Paused;

            if (Player.TryGetComponent(out PlayerController playerController))
            {
                playerController.enabled = false;
                playerController.mouseLook.SetCursorLock(false);
            }

            if (Player.TryGetComponent(out PlayerInteract playerInteract))
            {
                playerInteract.enabled = false;
            }

            if (Player.TryGetComponent(out PlayerInteractUI playerInteractUI))
            {
                playerInteractUI.enabled = false;
            }

            AudioManager.Instance.SetMusicPitch(0.5f);
            AudioManager.Instance.PauseAllSFX();
            PauseAllEnemiesSound();
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = IsInChase ? GameState.Chase : GameState.Playing;

            if (Player.TryGetComponent(out PlayerController playerController))
            {
                playerController.enabled = true;
                playerController.mouseLook.SetCursorLock(true);
            }

            if (Player.TryGetComponent(out PlayerInteract playerInteract))
            {
                playerInteract.enabled = true;
            }

            if (Player.TryGetComponent(out PlayerInteractUI playerInteractUI))
            {
                playerInteractUI.enabled = true;
            }

            AudioManager.Instance.SetMusicPitch(1f);
            AudioManager.Instance.ResumeAllSFX();
            ResumeAllEnemiesSound();
        }
    }

    public void HandleChase()
    {
        CurrentState = GameState.Chase;

        if (IsInChase)
        {
            AudioManager.Instance.PlayChaseMusic();
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