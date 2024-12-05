using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Keypad : MonoBehaviour
{
    public Door currentDoor;
    public TextMeshProUGUI keypadText;
    public Image backgroundText;
    public UnityEngine.UI.Button[] keypadButtons;

    private int maxInputLength = 4;

    void Start()
    {
        keypadText.text = string.Empty;

        if (keypadButtons.Length == 0)
        {
            keypadButtons = GetComponentsInChildren<UnityEngine.UI.Button>();
        }

        foreach (var button in keypadButtons)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    public void Number (int number)
    {
        if (keypadText.text.Length < maxInputLength)
        {
            keypadText.text += number.ToString();
        }
    }

    public void Enter()
    {
        if (keypadText.text == currentDoor.code)
        {
            AudioManager.Instance.PlayRightAnswerSound();
            keypadText.text = "Right";
            currentDoor.UnlockKeypadDoor();
            backgroundText.color = Color.green;
            DisableAllButtons();
        }
        else
        {
            AudioManager.Instance.PlayWrongAnswerSound();
            keypadText.text = "Wrong";
            backgroundText.color = Color.red;
            DisableAllButtons();
            StartCoroutine(ClearTextAfterDelay(1f));
        }
    }

    private IEnumerator ClearTextAfterDelay (float delay)
    {
        yield return new WaitForSeconds(delay);
        keypadText.text = "";
        backgroundText.color = Color.white;
        EnableAllButtons();
    }

    public void Clear()
    {
        if (keypadText.text.Length > 0 && char.IsDigit(keypadText.text[keypadText.text.Length - 1]))
        {
            keypadText.text = keypadText.text.Substring(0,keypadText.text.Length - 1);
        }
    }

    private void DisableAllButtons()
    {
        foreach (UnityEngine.UI.Button button in keypadButtons)
        {
            button.interactable = false;
        }
    }

    private void EnableAllButtons()
    {
        foreach (UnityEngine.UI.Button button in keypadButtons)
        {
            button.interactable = true;
        }
    }

    private void PlayClickSound()
    {
        AudioManager.Instance.PlayClickSound();
    }
}