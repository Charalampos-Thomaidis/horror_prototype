using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TrialEndMenu : MonoBehaviour
{
    public static bool trialEnded = false;

    public GameObject trialCompletedUI;
    public TextMeshProUGUI timeText;

    private PlayerController playerController;
    private Flashlight flashlight;
    private AudioManager audioManager;
    private float elapsedTime;

    void Start()
    {
        playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
        flashlight = GameManager.Instance.FlashlightHolder.GetComponent<Flashlight>();
        audioManager = AudioManager.GetAudioManager();
        elapsedTime = 0f;
        trialEnded = false;
    }

    void Update()
    {
        if (!trialEnded)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trialCompletedUI.SetActive(true);

            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 100) % 100);

            timeText.text = $"Time to Complete: {minutes:00}'{seconds:00}''{milliseconds:00}";

            audioManager.PlayBackgroundMusic();

            Time.timeScale = 0f;
            trialEnded = true;

            playerController.enabled = false;
            flashlight.enabled = false;
            playerController.mouseLook.SetCursorLock(false);
        }
    }
    
    public void Restart()
    {
        SceneManager.LoadScene(1);
        AudioManager.Instance.PlayClickSound();
    }
}