using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // Reference to the dropdowns for settings
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown fpsDropdown;
    public TMP_Dropdown fullscreenDropdown;

    public Slider volumeSliderSFX;
    public Slider volumeSliderMusic;

    private Resolution[] resolutions; // Store all available screen resolutions
    private List<Resolution> filteredResolutions; // Store filtered resolutions based on aspect ratio
    private int currentResolutionIndex = 0; // Index to track the current resolution
    private float targetAspectRatio; // Desired aspect ratio

    private void Start()
    {
        // Set up the volume slider and initialize its value from PlayerPrefs
        float savedVolumeSFX = PlayerPrefs.GetFloat("GlobalVolumeSFX", 1.0f);
        volumeSliderSFX.value = savedVolumeSFX;
        AudioManager.Instance.SetGlobalVolumeSFX(savedVolumeSFX);

        float savedVolumeMusic = PlayerPrefs.GetFloat("GlobalVolumeMusic", 1.0f);
        volumeSliderMusic.value = savedVolumeMusic;
        AudioManager.Instance.SetGlobalVolumeMusic(savedVolumeMusic);

        // Add listener to handle volume changes
        volumeSliderSFX.onValueChanged.AddListener(UpdateVolumeSFX);
        volumeSliderMusic.onValueChanged.AddListener(UpdateVolumeMusic);

        // Calculate the target aspect ratio based on the current screen dimensions
        targetAspectRatio = (float)Screen.width / Screen.height;

        InitializeQualitySettings();
        InitializeResolutionSettings();
        InitializeFPSSettings();
        InitializeFullscreenSettings();

        LoadSettings();
        ApplySettings();

        // Add listeners for dropdown value changes with sound effect
        resolutionDropdown.onValueChanged.AddListener(OnResolutionValueChanged);
        qualityDropdown.onValueChanged.AddListener(OnQualityValueChanged);
        fpsDropdown.onValueChanged.AddListener(OnFPSValueChanged);
        fullscreenDropdown.onValueChanged.AddListener(OnFullscreenValueChanged);
    }

    // Method to update the volume based on slider value
    public void UpdateVolumeSFX(float volume)
    {
        // Update global volume in AudioManager
        AudioManager.Instance.SetGlobalVolumeSFX(volume);

        // Save the new volume setting
        PlayerPrefs.SetFloat("GlobalVolumeSFX", volume);
        PlayerPrefs.Save();
    }

    public void UpdateVolumeMusic(float volume)
    {
        AudioManager.Instance.SetGlobalVolumeMusic(volume);

        PlayerPrefs.SetFloat("GlobalVolumeMusic", volume);
        PlayerPrefs.Save();
    }

    // Initialize the quality settings dropdown
    private void InitializeQualitySettings()
    {
        qualityDropdown.ClearOptions();
        List<string> qualityLevels = new List<string>();

        // Populate dropdown with available quality levels
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            qualityLevels.Add(QualitySettings.names[i]);
        }

        qualityDropdown.AddOptions(qualityLevels);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    private void InitializeResolutionSettings()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        UpdateResolutionDropdown();
    }

    // Filter resolutions by aspect ratio and update dropdown
    private void UpdateResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();

        List<string> resolutionOptions = new List<string>();
        HashSet<string> uniqueResolutions = new HashSet<string>();

        // Filter and add unique resolutions to dropdown
        for (int i = 0; i < resolutions.Length; i++)
        {
            float aspectRatio = (float)resolutions[i].width / resolutions[i].height;

            // Check if the aspect ratio matches the target
            if (Mathf.Approximately(aspectRatio, targetAspectRatio))
            {
                string resolutionOption = $"{resolutions[i].width} x {resolutions[i].height}";
                if (uniqueResolutions.Add(resolutionOption))
                {
                    resolutionOptions.Add(resolutionOption);
                    filteredResolutions.Add(resolutions[i]);

                    // Check if this is the current resolution
                    if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                    {
                        currentResolutionIndex = resolutionOptions.Count - 1;
                    }
                }
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // Initialize the FPS settings dropdown
    private void InitializeFPSSettings()
    {
        fpsDropdown.ClearOptions();

        List<string> fpsOptions = new List<string>
        {
            "Unlimited", "360", "240", "165", "144", "120", "60", "30"
        };

        fpsDropdown.AddOptions(fpsOptions);
        fpsDropdown.value = Array.IndexOf(fpsOptions.ToArray(), Application.targetFrameRate == -1 ? "Unlimited" : Application.targetFrameRate.ToString());
        fpsDropdown.RefreshShownValue();
    }

    // Initialize the fullscreen settings dropdown
    private void InitializeFullscreenSettings()
    {
        fullscreenDropdown.ClearOptions();

        List<string> fullscreenOptions = new List<string>
        {
            "Fullscreen", "Windowed", "Exclusive Fullscreen"
        };

        fullscreenDropdown.AddOptions(fullscreenOptions);
        fullscreenDropdown.value = GetFullscreenModeIndex(Screen.fullScreenMode);
        fullscreenDropdown.RefreshShownValue();
    }

    // Get index of the current fullscreen mode
    private int GetFullscreenModeIndex(FullScreenMode mode)
    {
        if (mode == FullScreenMode.ExclusiveFullScreen)
        {
            return 2; // Return index for Exclusive Fullscreen
        }
        else if (mode == FullScreenMode.FullScreenWindow)
        {
            return 0; // Return index for Fullscreen
        }
        else if (mode == FullScreenMode.Windowed)
        {
            return 1; // Return index for Windowed
        }
        else
        {
            return 0; // Default to Fullscreen if no match found
        }
    }

    // Event listener for resolution dropdown value change
    private void OnResolutionValueChanged(int value)
    {
        AudioManager.Instance.PlayClickSound();
        SetResolution(value);
    }

    // Event listener for quality dropdown value change
    private void OnQualityValueChanged(int value)
    {
        AudioManager.Instance.PlayClickSound();
        SetQuality(value);
    }

    // Event listener for fps dropdown value change
    private void OnFPSValueChanged(int value)
    {
        AudioManager.Instance.PlayClickSound();
        SetFPSLimit(value);
    }

    // Event listener for fullscreen dropdown value change
    private void OnFullscreenValueChanged(int value)
    {
        AudioManager.Instance.PlayClickSound();
        SetFullscreenMode(value);
    }

    // Set the screen resolution
    public void SetResolution(int resolutionIndex)
    {
        currentResolutionIndex = resolutionIndex; // Update the current resolution index
        Resolution resolution = filteredResolutions[resolutionIndex]; // Get selected resolution

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        SaveSettings();
    }

    // Set the quality level
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        SaveSettings();
    }

    // Set the FPS limit
    public void SetFPSLimit(int fpsIndex)
    {
        int[] fpsValues = { -1, 360, 240, 165, 144, 120, 60, 30 }; // Corresponding FPS values
        int targetFPS = fpsValues[fpsIndex]; // Get selected FPS

        Application.targetFrameRate = targetFPS;
        SaveSettings();
    }

    // Set the fullscreen mode based on the dropdown index
    public void SetFullscreenMode(int fullscreenIndex)
    {
        // Map dropdown index to fullscreen mode
        FullScreenMode[] modes = { FullScreenMode.FullScreenWindow, FullScreenMode.Windowed, FullScreenMode.ExclusiveFullScreen };

        Screen.fullScreenMode = modes[fullscreenIndex];
        SaveSettings();
    }

    // Save settings using PlayerPrefs
    private void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionIndex", currentResolutionIndex);
        PlayerPrefs.SetInt("QualityIndex", QualitySettings.GetQualityLevel());
        PlayerPrefs.SetInt("FPSIndex", fpsDropdown.value);
        PlayerPrefs.SetInt("FullscreenIndex", fullscreenDropdown.value);
        PlayerPrefs.Save();
    }

    // Load settings using PlayerPrefs
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
        }
        else
        {
            currentResolutionIndex = GetCurrentResolutionIndex();
        }

        if (PlayerPrefs.HasKey("QualityIndex"))
        {
            int qualityIndex = PlayerPrefs.GetInt("QualityIndex");
            qualityDropdown.value = qualityIndex;
            qualityDropdown.RefreshShownValue();
        }

        if (PlayerPrefs.HasKey("FPSIndex"))
        {
            int fpsIndex = PlayerPrefs.GetInt("FPSIndex");
            fpsDropdown.value = fpsIndex;
            fpsDropdown.RefreshShownValue();
        }

        if (PlayerPrefs.HasKey("FullscreenIndex"))
        {
            int fullscreenIndex = PlayerPrefs.GetInt("FullscreenIndex");
            fullscreenDropdown.value = fullscreenIndex;
            fullscreenDropdown.RefreshShownValue();
        }
    }

    // Apply settings to the application
    private void ApplySettings()
    {
        Resolution resolution = filteredResolutions[currentResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);

        int qualityIndex = PlayerPrefs.GetInt("QualityIndex", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(qualityIndex);

        int fpsIndex = PlayerPrefs.GetInt("FPSIndex", fpsDropdown.value);
        SetFPSLimit(fpsIndex);

        int fullscreenIndex = PlayerPrefs.GetInt("FullscreenIndex", fullscreenDropdown.value);
        SetFullscreenMode(fullscreenIndex);
    }

    // Get the current resolution index from the filtered resolutions
    private int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            if (filteredResolutions[i].width == Screen.width &&
                filteredResolutions[i].height == Screen.height)
            {
                return i;
            }
        }
        return 0; // Default to first resolution if not found
    }
}