using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance { get; private set; }

    [SerializeField]
    private FOVKick fovKick = new FOVKick();

    private bool isBuffActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene loaded events to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Initialize()
    {
        // Re-fetch the camera and re-setup FOVKick if needed
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            fovKick.Setup(mainCamera);
        }

        // Reset buff state as necessary
        isBuffActive = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Initialize();
    }

    private void Start()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            fovKick.Setup(mainCamera);
        }
    }

    public void ApplyBuff(PlayerController playerController, float speedBuff, float duration, Color color)
    {
        if (playerController == null || isBuffActive) return;

        StartCoroutine(ApplyBuffCoroutine(playerController, speedBuff, duration, color));
    }

    private IEnumerator ApplyBuffCoroutine(PlayerController playerController, float speedBuff, float duration, Color color)
    {
        isBuffActive = true;

        if (!playerController.IsPlayerInCloset)
        {
            playerController.walkSpeed += speedBuff;
            playerController.runSpeed += speedBuff;
        }

        if (fovKick != null && fovKick.cam != null)
        {
            StartCoroutine(fovKick.FOVKickUp());
        }

        VignetteManager.Instance.ApplyVignetteEffect(color, 0.4f, duration, VignetteManager.VignetteState.Buff);

        yield return new WaitForSeconds(duration);

        if (fovKick != null && fovKick.cam != null)
        {
            StartCoroutine(fovKick.FOVKickDown());
        }

        if (!playerController.IsPlayerInCloset)
        {
            playerController.walkSpeed -= speedBuff;
            playerController.runSpeed -= speedBuff;
        }

        ResetSpeedValues(playerController);

        isBuffActive = false;
    }

    private void ResetSpeedValues(PlayerController playerController)
    {
        // Reset to original values based on initial player state
        playerController.walkSpeed = playerController.originalWalkSpeed;
        playerController.runSpeed = playerController.originalRunSpeed;
    }

    public bool IsBuffActive()
    {
        return isBuffActive;
    }
}