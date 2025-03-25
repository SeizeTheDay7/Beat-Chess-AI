using UnityEngine;

public class InvisibleHandCommand : ICommand
{
    private InvisibleHand invisibleHand;
    private GameObject piece;
    private Vector3 targetPos;
    private float time;
    private bool willEndTurn;

    public InvisibleHandCommand(GameObject piece, Vector3 targetPos, float time, InvisibleHand invisibleHand, bool willEndTurn)
    {
        this.piece = piece;
        this.targetPos = targetPos;
        this.time = time;
        this.invisibleHand = invisibleHand;
        this.willEndTurn = willEndTurn;
    }

    public void Execute()
    {
        invisibleHand.MovePieceToPos(piece, targetPos, time);
    }

    public bool WillEndTurn()
    {
        return willEndTurn;
    }
}