using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI tutorialText;
    public Animator dialogueAnimator;
    public Animator tutorialAnimator;

    private Queue<string> sentences;
    private bool isDialogueActive = false;
    private bool isTutorialActive = false;

    void Awake()
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
        if (scene.name == "MainMenu")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            Initialize();
        }
    }
    public void Initialize()
    {
        nameText = GameObject.Find("Name").GetComponent<TextMeshProUGUI>();
        dialogueText = GameObject.Find("Dialogue").GetComponent<TextMeshProUGUI>();
        tutorialText = GameObject.Find("TutorialText").GetComponent<TextMeshProUGUI>();
        dialogueAnimator = GameObject.Find("DialogueBox").GetComponent<Animator>();
        tutorialAnimator = GameObject.Find("TutorialBox").GetComponent<Animator>();

        sentences = new Queue<string>();
        isDialogueActive = false;
        isTutorialActive = false;
    }

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (isDialogueActive)
        {
            return;
        }

        if (isTutorialActive)
        {
            EndTutorial();
        }

        isDialogueActive = true;
        dialogueAnimator.SetBool("IsOpen", true);
        GameManager.Instance.Player.GetComponent<PlayerController>().enabled = false;

        nameText.text = dialogue.name;
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            while (Time.timeScale == 0)
            {
                yield return null;
            }
            dialogueText.text += letter;
            yield return null;
        }
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialogueAnimator.SetBool("IsOpen", false);
        GameManager.Instance.Player.GetComponent<PlayerController>().enabled = true;
        GameManager.Instance.Player.GetComponent<PlayerController>().mouseLook.SetCursorLock(true);
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    public void StartTutorial(Dialogue dialogue)
    {
        if (isTutorialActive || isDialogueActive)
        {
            return;
        }

        isTutorialActive = true;
        tutorialAnimator.SetBool("isOpen", true);
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        StartCoroutine(DisplayTutorial());
    }

    private IEnumerator DisplayTutorial()
    {
        while (sentences.Count > 0)
        {
            string sentence = sentences.Dequeue();
            StopCoroutine("TypeSentence");
            StartCoroutine(TypeSentenceTutorial(sentence));
            yield return new WaitForSeconds(5f);
        }

        EndTutorial();
    }

    IEnumerator TypeSentenceTutorial(string sentence)
    {
        tutorialText.text = "";
        tutorialText.text += sentence;
        yield return null;
    }

    public void EndTutorial()
    {
        tutorialAnimator.SetBool("isOpen", false);
        isTutorialActive = false;
    }

    public bool IsTutorialActive()
    {
        return isTutorialActive;
    }
}