using UnityEngine;

public class DeskLight : MonoBehaviour
{
    private Light deskLight;
    private AudioSource audioSource;

    void Start()
    {
        deskLight = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();
    }

    public void TurnOnLight()
    {
        deskLight.intensity = 30;
        audioSource.Play();
    }

    public void TurnOffLight()
    {
        deskLight.intensity = 0;
        audioSource.Play();
    }
}