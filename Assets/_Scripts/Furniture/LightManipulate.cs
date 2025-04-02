using UnityEngine;

public class LightManipulate : MonoBehaviour
{
    private Light deskLight;
    private AudioSource audioSource;

    void Awake()
    {
        deskLight = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();
    }

    public void TurnOnLight()
    {
        deskLight.enabled = true;
        audioSource.Play();
    }

    public void TurnOffLight()
    {
        deskLight.enabled = false;
        audioSource.Play();
    }
}