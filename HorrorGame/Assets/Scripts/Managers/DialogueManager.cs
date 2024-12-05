using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private Queue<string> sentences;
    private bool isDialogueActive = false;
    private bool isTutorialActive = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GameManager.Instance.InitializeReferences();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        // Initialize sentence queue and other state variables
        sentences = new Queue<string>();
        isDialogueActive = false;
        isTutorialActive = false;
    }

    void Start()
    {
        sentences = new Queue<string>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsDialogueActive())
        {
            DisplayNextSentence();
        }
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
        GameManager.Instance.DialogueAnimator.SetBool("IsOpen", true);
        GameManager.Instance.Player.GetComponent<PlayerController>().enabled = false;
        GameManager.Instance.Player.GetComponent<PlayerUI>().enabled = false;
        GameManager.Instance.Inventory.SetActive(false);

        GameManager.Instance.NameText.text = dialogue.name;
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
        GameManager.Instance.DialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            GameManager.Instance.DialogueText.text += letter;
            yield return null;
        }
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        GameManager.Instance.DialogueAnimator.SetBool("IsOpen", false);
        GameManager.Instance.Player.GetComponent<PlayerController>().enabled = true;
        GameManager.Instance.Player.GetComponent<PlayerUI>().enabled = true;
        GameManager.Instance.Inventory.SetActive(true);
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
        GameManager.Instance.TutorialAnimator.SetBool("isOpen", true);
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
        GameManager.Instance.TutorialText.text = "";
        GameManager.Instance.TutorialText.text += sentence;
        yield return null;
    }

    public void EndTutorial()
    {
        GameManager.Instance.TutorialAnimator.SetBool("isOpen", false);
        isTutorialActive = false;
    }

    public bool IsTutorialActive()
    {
        return isTutorialActive;
    }
}