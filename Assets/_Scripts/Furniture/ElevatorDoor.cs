using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class ElevatorDoor : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip openSFX;
    [SerializeField] private AudioClip closeSFX;
    [SerializeField] private AudioClip vacantSFX;
    [SerializeField] private Transform LeftDoor;
    [SerializeField] private Transform RightDoor;
    [SerializeField] private float doorOpenTime = 2f;
    [SerializeField] private float doorCloseTime = 0.5f;
    [SerializeField] private MeshRenderer signRenderer;
    [SerializeField] private Texture2D occupiedTexture;
    [SerializeField] private Texture2D vacantTexture;
    private MaterialPropertyBlock mpb;
    private Vector3 leftDoorStartPos;
    private Vector3 rightDoorStartPos;

    void Start()
    {
        mpb = new MaterialPropertyBlock();
        audioSource = GetComponent<AudioSource>();
        leftDoorStartPos = LeftDoor.localPosition;
        rightDoorStartPos = RightDoor.localPosition;
    }

    public void ElevatorDoorOpen()
    {
        LeftDoor.DOLocalMoveX(6.68f, doorOpenTime);
        RightDoor.DOLocalMoveX(-5.95f, doorOpenTime);
        audioSource.clip = openSFX;
        audioSource.Play();
    }

    public void ElevatorDoorClose()
    {
        LeftDoor.DOLocalMove(leftDoorStartPos, doorCloseTime);
        RightDoor.DOLocalMove(rightDoorStartPos, doorCloseTime);
        audioSource.clip = closeSFX;
        audioSource.Play();
    }

    public void SetOcccupied()
    {
        signRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_MainTex", occupiedTexture);
        signRenderer.SetPropertyBlock(mpb);
    }

    public void SetVacant()
    {
        signRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_MainTex", vacantTexture);
        signRenderer.SetPropertyBlock(mpb);
        audioSource.clip = vacantSFX;
        audioSource.Play();
    }
}
