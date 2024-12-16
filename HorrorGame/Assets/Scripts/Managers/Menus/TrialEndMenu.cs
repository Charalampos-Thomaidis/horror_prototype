using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TrialEndMenu : MonoBehaviour
{
    public static bool trialEnded = false;

    public GameObject trialCompletedUI;
    public TextMeshProUGUI timeText;

    private float elapsedTime;
    


    void Start()
    {
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
    
    //Called by animation event at the end of animation of elevator
    public void OnElevatorClosed()
    {
        trialCompletedUI.SetActive(true);

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100) % 100);

        timeText.text = $"Time to Complete: {minutes:00}'{seconds:00}''{milliseconds:00}";

        Time.timeScale = 0f;
        trialEnded = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
        AudioManager.Instance.PlayClickSound();
    }
}