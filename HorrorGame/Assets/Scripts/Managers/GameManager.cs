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
    public GameObject Inventory { get; private set; }
    public GameObject FlashlightHolder { get; private set; }
    public PauseMenu PauseMenu { get; private set; }
    public PostProcessVolume PostProcessVolume { get; private set; }
    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();

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
        InitializeReferences();
    }

    public void InitializeReferences()
    {
        Player = GameObject.FindWithTag("Player");
        Inventory = GameObject.Find("Inventory");
        FlashlightHolder = GameObject.Find("FlashlightHolder");
        NameText = GameObject.Find("Name")?.GetComponent<TextMeshProUGUI>();
        DialogueText = GameObject.Find("Dialogue")?.GetComponent<TextMeshProUGUI>();
        TutorialText = GameObject.Find("TutorialText")?.GetComponent<TextMeshProUGUI>();
        DialogueAnimator = GameObject.Find("DialogueBox")?.GetComponent<Animator>();
        TutorialAnimator = GameObject.Find("TutorialBox")?.GetComponent<Animator>();
        DialogueManager = FindObjectOfType<DialogueManager>();
        PauseMenu = FindObjectOfType<PauseMenu>();
        PostProcessVolume = FindObjectOfType<PostProcessVolume>();

        Enemies.Clear();
        Enemies.AddRange(FindObjectsOfType<Enemy>());

        DialogueManager?.Initialize();
    }
    public IEnumerable<Enemy> GetEnemies() => Enemies;

    public void PauseAllEnemies()
    {
        foreach (Enemy enemy in Enemies)
        {
            enemy.PauseEatSound();
        }
    }

    public void ResumeAllEnemies()
    {
        foreach (Enemy enemy in Enemies)
        {
            enemy.ResumeEatSound();
        }
    }
}