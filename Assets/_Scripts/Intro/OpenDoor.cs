using UnityEngine;
using DG.Tweening;
using System.Collections;
public class OpenDoor : MonoBehaviour
{
    [SerializeField] private Transform door;
    [SerializeField] private float doorOpenTime = 1f;
    private float buzzDelayTime;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] doorOpenSounds;

    void OnTriggerEnter(Collider other)
    {
        buzzDelayTime = doorOpenSounds[0].length;
        Sequence doorSequence = DOTween.Sequence();

        doorSequence.AppendCallback(() =>
        {
            audioSource.clip = doorOpenSounds[0];
            audioSource.Play();
        });

        doorSequence.AppendInterval(buzzDelayTime + 0.1f);

        doorSequence.AppendCallback(() =>
        {
            door.DOLocalMoveX(4.3f, doorOpenTime);
            audioSource.clip = doorOpenSounds[1];
            audioSource.Play();
        });

        Destroy(this);
    }
}
