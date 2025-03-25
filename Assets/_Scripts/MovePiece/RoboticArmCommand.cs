using UnityEngine;

public class RoboticArmCommand : ICommand
{
    private RoboticArm roboticArm;
    private GameObject piece;
    private Vector3 targetPos;
    private float time;
    private bool willEndTurn;

    public RoboticArmCommand(GameObject piece, Vector3 targetPos, float time, RoboticArm roboticArm, bool willEndTurn)
    {
        this.piece = piece;
        this.targetPos = targetPos;
        this.time = time;
        this.roboticArm = roboticArm;
        this.willEndTurn = willEndTurn;
    }

    public void Execute()
    {
        roboticArm.MovePieceToPos(piece, targetPos, time);
    }

    public bool WillEndTurn()
    {
        return willEndTurn;
    }
}