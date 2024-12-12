using UnityEngine;

public class SoundOcclusion : MonoBehaviour
{
    public AudioSource[] audioSources;
    public LayerMask occlusionMask;
    public float maxVolume = 1f;
    public float occludedVolume = 0.1f;
    public static float globalSFXVolume = 1f;

    private Transform listener;

    void Start()
    {
        listener = Camera.main.transform;

        if (audioSources == null || audioSources.Length == 0)
        {
            audioSources = GetComponents<AudioSource>();
        }
    }

    void Update()
    {
        if (listener != null)
        {
            Vector3 directionToListener = (listener.position - transform.position).normalized;
            float distanceToListener = Vector3.Distance(transform.position, listener.position);

            bool isOccluded = Physics.Raycast(transform.position, directionToListener, distanceToListener, occlusionMask);

            foreach (AudioSource source in audioSources)
            {
                float targetVolume = isOccluded ? occludedVolume : maxVolume;
                source.volume = targetVolume * globalSFXVolume;
            }
        }
    }

    public static void UpdateGlobalSFXVolume(float volume)
    {
        globalSFXVolume = volume;
    }
}