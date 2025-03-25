using UnityEngine;
using DG.Tweening;

public class InvisibleHand : MonoBehaviour
{
    PieceCommandManager pieceCommandManager;
    private AudioSource player_drop_sfx;
    private Sequence current_seq;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pieceCommandManager = GameObject.FindGameObjectWithTag("ServiceLocator").GetComponentInChildren<PieceCommandManager>();
        player_drop_sfx = GetComponent<AudioSource>();
    }

    public void MovePieceToPos(GameObject piece, Vector3 targetPos, float time)
    {
        float jumpPower = 0.25f; // 원하는 점프 높이를 설정
        int numJumps = 1;       // 점프 횟수를 설정 (1회이면 한 번의 포물선)

        piece.transform.DOJump(targetPos, jumpPower, numJumps, time)
             .OnComplete(() => { pieceCommandManager.CompleteCommand(); player_drop_sfx.Play(); });
    }

    // public void OnlyMovePieceToPos(GameObject piece, Vector3 targetPos, float time)
    // {
    //     piece.transform.DOMove(targetPos, time)
    //          .OnComplete(() => { player_drop_sfx.Play(); });
    // }
}
