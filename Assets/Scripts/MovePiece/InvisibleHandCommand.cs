using UnityEngine;

public class InvisibleHandCommand : ICommand
{
    private InvisibleHand invisibleHand;
    private GameObject piece;
    private Vector3 targetPos;
    private float time;

    public InvisibleHandCommand(GameObject piece, Vector3 targetPos, float time, InvisibleHand invisibleHand)
    {
        this.piece = piece;
        this.targetPos = targetPos;
        this.time = time;
        this.invisibleHand = invisibleHand;
    }

    public void Execute()
    {
        invisibleHand.MovePieceToPos(piece, targetPos, time);
    }
}