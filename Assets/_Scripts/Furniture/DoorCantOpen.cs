using UnityEngine;
using DG.Tweening;

public class DoorCantOpen : MonoBehaviour, IRaycastNonGame
{
    [SerializeField] GameObject door;
    [SerializeField] GameObject handle;
    [SerializeField] float handleAngle = 35f;
    [SerializeField] float doorAngle = -2f;
    [SerializeField] float doorTime = 1f;
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
            audioSource.Play();
            Sequence sequence = DOTween.Sequence();
            sequence.Append(
                    door.transform.DOLocalRotate(new Vector3(0, -2, 0), doorTime))
            .Join(handle.transform.DOLocalRotate(new Vector3(0, 0, 35), doorTime))
            .Append(door.transform.DOLocalRotate(Vector3.zero, doorTime))
            .Join(handle.transform.DOLocalRotate(Vector3.zero, doorTime))
            .OnComplete(() => isMoving = false);
        }
    }

    public void OnStartLooking() { }
    public void OnEndLooking() { }
}
