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

    public void MetalDoorOpen()
    {
        StartCoroutine(DoorOpenCoroutine());
    }

    private IEnumerator DoorOpenCoroutine()
    {
        audioSource.clip = metalDoorSounds[0];
        audioSource.Play();
        yield return new WaitForSeconds(metalDoorSounds[0].length);
        transform.DOLocalMoveX(4.5f, doorSlideTime);
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