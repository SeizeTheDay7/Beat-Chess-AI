using UnityEngine;
using DG.Tweening;
using System.Collections;

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
        StartCoroutine(DoorOpenSound());
        transform.DOLocalMoveX(4.3f, doorSlideTime);
        return doorSlideTime;
    }

    private IEnumerator DoorOpenSound()
    {
        audioSource.clip = metalDoorSounds[0];
        audioSource.Play();
        yield return new WaitForSeconds(metalDoorSounds[0].length);
        audioSource.clip = metalDoorSounds[1];
        audioSource.Play();
    }

    public float MetalDoorClose()
    {
        audioSource.clip = metalDoorSounds[1];
        audioSource.Play();
        transform.DOLocalMoveX(0f, doorSlideTime);
        return doorSlideTime;
    }
}