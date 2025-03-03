using UnityEngine;

public class RoboticArmCommand : ICommand
{
    private RoboticArm roboticArm;
    private GameObject piece;
    private Vector3 targetPos;
    private float time;

    public RoboticArmCommand(GameObject piece, Vector3 targetPos, float time, RoboticArm roboticArm)
    {
        this.piece = piece;
        this.targetPos = targetPos;
        this.time = time;
        this.roboticArm = roboticArm;
    }

    public void Execute()
    {
        roboticArm.MovePieceToPos(piece, targetPos, time);
    }
}