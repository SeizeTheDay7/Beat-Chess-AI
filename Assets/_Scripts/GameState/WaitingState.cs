public class WaitingState : IGameState
{
    private RoboticArm roboticArm;

    public WaitingState(RoboticArm roboticArm)
    {
        this.roboticArm = roboticArm;
    }

    public void EnterState()
    {
        roboticArm.Reset_Arm();
    }

    public void UpdateState()
    {

    }

    public void ExitState()
    {

    }
}