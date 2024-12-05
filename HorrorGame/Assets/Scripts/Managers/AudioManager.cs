using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private float globalVolumeSFX = 1.0f;
    private float globalVolumeMusic = 1.0f;

    private AudioSource BackgroundMusic;
    private AudioSource ClickSound;
    private AudioSource ChaseMusic;
    private AudioSource EnemyAttackSound;
    private AudioSource EnemyFlinchSound;
    private AudioSource UnlockedSound;
    private AudioSource LockedSound;
    private AudioSource RightAnswerSound;
    private AudioSource WrongAnswerSound;
    private AudioSource BuffSound;
    private AudioSource HealSound;
    private AudioSource HurtSound;
    private AudioSource OpenDrawerSound;
    private AudioSource CloseDrawerSound;
    private AudioSource LightswitchSound;
    private AudioSource PickupSound;
    private AudioSource ChargingFlashlightSound;

    public AudioSource[] audioSources;

    public static AudioManager Instance = null;

    public static AudioManager GetAudioManager()
    {
        return Instance;
    }

    public void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSources = GetComponentsInChildren<AudioSource>();

            BackgroundMusic = audioSources[0];
            BackgroundMusic.loop = true;

            ChaseMusic = audioSources[1];
            ChaseMusic.loop = true;

            ClickSound = audioSources[2];
            ClickSound.loop = false;

            EnemyAttackSound = audioSources[3];
            EnemyAttackSound.loop = false;

            EnemyFlinchSound = audioSources[4];
            EnemyFlinchSound.loop = false;

            UnlockedSound = audioSources[5];
            UnlockedSound.loop = false;

            LockedSound = audioSources[6];
            LockedSound.loop = false;

            RightAnswerSound = audioSources[7];
            RightAnswerSound.loop = false;

            WrongAnswerSound = audioSources[8];
            WrongAnswerSound.loop = false;

            BuffSound = audioSources[9];
            BuffSound.loop = false;

            HealSound = audioSources[10];
            HealSound.loop = false;

            HurtSound = audioSources[11];
            HurtSound.loop = false;

            OpenDrawerSound = audioSources[12];
            OpenDrawerSound.loop = false;

            CloseDrawerSound = audioSources[13];
            CloseDrawerSound.loop = false;

            LightswitchSound = audioSources[14];
            LightswitchSound.loop = false;

            PickupSound = audioSources[15];
            PickupSound.loop = false;

            ChargingFlashlightSound = audioSources[16];
            ChargingFlashlightSound.loop = false;

            SetGlobalVolumeSFX(globalVolumeSFX);
            SetGlobalVolumeMusic(globalVolumeMusic);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetGlobalVolumeSFX(float volume)
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource != Instance.BackgroundMusic && audioSource != Instance.ChaseMusic)
            {
                audioSource.volume = volume;
            }
        }

        SoundOcclusion.UpdateGlobalSFXVolume(volume);
    }

    public void SetGlobalVolumeMusic(float volume) 
    {
        globalVolumeMusic = volume;

        if (BackgroundMusic != null)
        {
            BackgroundMusic.volume = globalVolumeMusic;
        }

        if (ChaseMusic != null)
        {
            ChaseMusic.volume = globalVolumeMusic;
        }
    }

    public float GetGlobalVolumeSFX()
    {
        return globalVolumeSFX;
    }

    public float GetGlobalVolumeMusic()
    {
        return globalVolumeMusic;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PlayerHealth.PlayerDied)
        {
            // Reset chase and play background music only if player died
            StopAllMusic();
            PlayBackgroundMusic();
            PlayerHealth.PlayerDied = false;  // Reset the flag
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;  // Subscribe to scene loaded event
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;  // Unsubscribe to prevent memory leaks
    }


    public void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (!BackgroundMusic.isPlaying)
        {
            BackgroundMusic.Play();
            ChaseMusic.Stop();
        }

        if (PauseMenu.GameIsPaused)
        {
            BackgroundMusic.pitch = 0.5f;
        }
    }

    public void PlayChaseMusic()
    {
        if (!ChaseMusic.isPlaying)
        {
            BackgroundMusic.Stop();
            ChaseMusic.Play();
        }

        if (PauseMenu.GameIsPaused)
        {
            ChaseMusic.pitch = 0.5f;
        }
    }

    public void PlayClickSound()
    {
        ClickSound.Play();
    }

    public void PlayEnemyAttackSound()
    {
        EnemyAttackSound.Play();
    }

    public void PlayEnemyFlinchSound()
    {
        EnemyFlinchSound.Play();
    }
    
    public void PlayUnlockedSound()
    {
        UnlockedSound.Play();
    }

    public void PlayLockedSound()
    {
        LockedSound.Play();
    }

    public void PlayRightAnswerSound()
    {
        RightAnswerSound.Play();
    }

    public void PlayWrongAnswerSound()
    {
        WrongAnswerSound.Play();
    }

    public void PlayBuffSound()
    {
        BuffSound.Play();
    }

    public void PlayHealSound()
    {
        HealSound.Play();
    }

    public void PlayHurtSound()
    {
        HurtSound.Play();
    }

    public void PlayOpenDrawerSound()
    {
        OpenDrawerSound.Play();
    }

    public void PlayCloseDrawerSound()
    {
        CloseDrawerSound.Play();
    }

    public void PlayLightswitchSound()
    {
        LightswitchSound.Play();
    }

    public void PlayPickupSound()
    {
        PickupSound.Play();
    }

    public void PlayChargingFlashlightSound()
    {
        ChargingFlashlightSound.Play();
    }

    public void SetMusicPitch(float pitch)
    {
        if (BackgroundMusic != null)
        {
            BackgroundMusic.pitch = pitch;
        }

        if (ChaseMusic != null)
        {
            ChaseMusic.pitch = pitch;
        }
    }

    public void PauseAllSFX()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource!=BackgroundMusic && audioSource != ChaseMusic)
            {
                audioSource.Pause();
            }
        }
    }

    public void ResumeAllSFX()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource != BackgroundMusic && audioSource != ChaseMusic)
            {
                audioSource.UnPause();
            }
        }
    }

    public void StopAllMusic()
    {
        if (BackgroundMusic.isPlaying) BackgroundMusic.Stop();
        if (ChaseMusic.isPlaying) ChaseMusic.Stop();
    }
}