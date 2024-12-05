using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsBackground;
    public GameObject menuBackground;
    public GameObject controlsBackground;

    private bool isSettingsActive = false;
    private bool isControlsActive = false;

    private void Start()
    {
        AudioManager.Instance.PlayBackgroundMusic();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isControlsActive)
            {
                ReturnToSettings();
            }
            else if (isSettingsActive)
            {
                ReturnToMenu();
            }
        }
    }

    public void SettingsMenu()
    {
        isSettingsActive = true;
        isControlsActive = false;
        settingsBackground.SetActive(true);
        controlsBackground.SetActive(false);
        menuBackground.SetActive(false);
        AudioManager.Instance.PlayClickSound();
    }

    public void ControllsMenu()
    {
        isControlsActive = true;
        isSettingsActive = false;
        controlsBackground.SetActive(true);
        settingsBackground.SetActive(false);
        menuBackground.SetActive(false);
        AudioManager.Instance.PlayClickSound();
    }

    public void ReturnToMenu()
    {
        menuBackground.SetActive(true);
        settingsBackground.SetActive(false);
        controlsBackground.SetActive(false);
        isSettingsActive = false;
        isControlsActive = false;
        AudioManager.Instance.PlayClickSound();
    }

    public void ReturnToSettings()
    {
        controlsBackground.SetActive(false);
        settingsBackground.SetActive(true);
        isSettingsActive = true;
        isControlsActive = false;
        AudioManager.Instance.PlayClickSound();
    }

    public void QuitGame()
    {
        AudioManager.Instance.PlayClickSound();
        Application.Quit();
    }
}
