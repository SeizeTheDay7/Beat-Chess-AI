using UnityEngine;
using DG.Tweening;

public class MetalCover : MonoBehaviour
{
    public void OpenCover()
    {
        transform.DOLocalMoveY(3f, 1f);
    }

    public void CloseCover()
    {
        transform.DOLocalMoveY(0f, 1f);
    }
}
