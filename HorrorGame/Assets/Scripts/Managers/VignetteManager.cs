using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class VignetteManager : MonoBehaviour
{
    public static VignetteManager Instance {  get; private set; }

    private Vignette vignette;
    private Coroutine currentCoroutine;
    private VignetteState currentState;
    private float buffEndTime;

    public enum VignetteState
    {
        None,
        Damage,
        Heal,
        Buff,
        Death
    }

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
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene loaded events to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Initialize()
    {
        // Re-fetch the PostProcessVolume and set up the vignette reference
        PostProcessVolume volume = GameManager.Instance.PostProcessVolume;
        if (volume != null && volume.profile != null)
        {
            volume.profile.TryGetSettings(out vignette);
            vignette.enabled.Override(false);
            currentState = VignetteState.None;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Initialize();
    }

    private void Start()
    {
        Initialize();
    }

    public void ApplyVignetteEffect(Color color, float intensity, float duration, VignetteState state)
    {
        if (vignette == null) return;
        if (currentState == VignetteState.Death) return;
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);

        currentState = state;
        vignette.color.Override(color);
        vignette.intensity.Override(intensity);
        vignette.enabled.Override(true);

        if (state == VignetteState.Buff)
        {
            buffEndTime = Time.time + duration;
            currentCoroutine = StartCoroutine(BuffEffectCoroutine());
        }
        else
        {
            currentCoroutine = StartCoroutine(EffectCoroutine(duration, state));
        }
    }

    private IEnumerator EffectCoroutine(float duration, VignetteState state)
    {
        yield return new WaitForSeconds(duration);

        if (state == currentState)
        {
            vignette.enabled.Override(false);
            currentState = VignetteState.None;

            if (buffEndTime > Time.time && state != VignetteState.Buff)
            {
                ReapplyBuffEffect();
            }
        }
    }

    private IEnumerator BuffEffectCoroutine()
    {
        while (Time.time < buffEndTime)
        {
            yield return null;
        }

        if (currentState == VignetteState.Buff)
        {
            vignette.enabled.Override(false);
            currentState = VignetteState.None;
        }
    }

    private void ReapplyBuffEffect()
    {
        float remainingDuration = buffEndTime - Time.time;
        ApplyVignetteEffect(Color.yellow, 0.4f, remainingDuration, VignetteState.Buff);
    }

    public void ApplyDeathEffect()
    {
        if (vignette == null) return;
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);

        currentState = VignetteState.Death;
        vignette.color.Override(Color.red);
        vignette.intensity.Override(1f);
        vignette.center.Override(new Vector2(1.5f, 1.5f));
        vignette.enabled.Override(true);
    }

    public void ResetVignette()
    {
        vignette.enabled.Override(false);
        currentState = VignetteState.None;
        buffEndTime = 0f;
    }
}