using UnityEngine;

public class LightSwitch : Interactable
{
    [SerializeField]
    private GameObject lightswitch;
    [SerializeField]
    private GameObject spotlight;
    [SerializeField]
    private bool Lighting;

    void Start()
    {
        lightswitch.GetComponent<Animator>().SetBool("Lighting", Lighting);
    }

    protected override void Interact()
    {
        Lighting = !Lighting;
        lightswitch.GetComponent<Animator>().SetBool("Lighting", Lighting);
        AudioManager.Instance.PlayLightswitchSound();
        spotlight.GetComponent<Light>().enabled = Lighting;
    }
}