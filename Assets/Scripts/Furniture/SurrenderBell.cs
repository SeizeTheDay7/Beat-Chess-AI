using UnityEngine;
using DG.Tweening;

public class SurrenderBell : MonoBehaviour
{
    [SerializeField] private GameObject handle;
    [SerializeField] private float rotateAngle = 30f;
    [SerializeField] private float rotateDuration = 0.3f;
    [SerializeField] private float shootStartOffset = 2f;
    [SerializeField] private Shooter shooter;
    private AudioSource audioSource;
    private float initialAngle;
    private bool isBellRinged = false;


    void Start()
    {
        initialAngle = handle.transform.localEulerAngles.x;
        audioSource = GetComponent<AudioSource>();
    }

    public void Ring()
    {
        print("Ring the bell!");

        // if (isBellRinged) return;
        // isBellRinged = true;
        audioSource.Play();
        DOVirtual.DelayedCall(audioSource.clip.length - shootStartOffset, () => shooter.PlayShooterFootstep());

        // 회전 -> 원래 위치로 되돌아오는 트윈 애니메이션
        handle.transform.DOLocalRotate(
            new Vector3(initialAngle + rotateAngle, 0, 0), // 목표 회전 각도
            rotateDuration, // 지속 시간
            RotateMode.Fast // 가장 빠른 회전 경로
        )
        .SetEase(Ease.OutQuad) // 자연스럽게 움직이도록 설정
        .OnComplete(() =>
        {
            // 원래 위치로 돌아옴
            handle.transform.DOLocalRotate(
                new Vector3(initialAngle, 0, 0),
                rotateDuration,
                RotateMode.Fast
            ).SetEase(Ease.InQuad);
        });
    }
}
