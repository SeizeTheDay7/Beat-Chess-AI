using UnityEngine;
using DG.Tweening;

public class WoodenDoor : MonoBehaviour, IRaycastNonGame
{
    [SerializeField] GameObject door;
    [SerializeField] GameObject handle;
    [SerializeField] float handleAngle = 30f;
    [SerializeField] float doorAngle = -1f;
    [SerializeField] float doorTime = 0.05f;
    [SerializeField] AudioClip doorStuckSound;
    [SerializeField] AudioClip doorOpenSound;
    [SerializeField] AudioClip doorCloseSound;
    AudioSource audioSource;
    bool isMoving = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnClicked(Transform player)
    {
        if (!isMoving)
        {
            isMoving = true;
            PlaySound(doorStuckSound);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(
                    door.transform.DOLocalRotate(new Vector3(0, -2, 0), doorTime))
            .Join(handle.transform.DOLocalRotate(new Vector3(0, 0, 35), doorTime))
            .Append(door.transform.DOLocalRotate(Vector3.zero, doorTime))
            .Join(handle.transform.DOLocalRotate(Vector3.zero, doorTime))
            .OnComplete(() => isMoving = false);
        }
    }

    public void OpenDoor()
    {
        door.transform.DOLocalRotate(new Vector3(0, -120f, 0), 2f).SetEase(Ease.Linear);
        PlaySound(doorOpenSound);
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void OnStartLooking() { }
    public void OnEndLooking() { }
}
