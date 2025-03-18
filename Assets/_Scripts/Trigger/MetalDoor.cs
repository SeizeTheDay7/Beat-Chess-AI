using UnityEngine;
using DG.Tweening;

public class MetalDoor : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] metalDoorSounds;
    [SerializeField] private float doorSlideTime = 1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public float MetalDoorOpen()
    {
        PlayDoorSound();
        transform.DOLocalMoveX(4.3f, doorSlideTime);
        return doorSlideTime;
    }

    public float MetalDoorClose()
    {
        PlayDoorSound();
        transform.DOLocalMoveX(0f, doorSlideTime);
        return doorSlideTime;
    }

    public void PlayDoorSound()
    {
        audioSource.clip = metalDoorSounds[1];
        audioSource.Play();
    }

    public float PlayBuzzer()
    {
        audioSource.clip = metalDoorSounds[0];
        audioSource.Play();
        return metalDoorSounds[0].length;
    }
}