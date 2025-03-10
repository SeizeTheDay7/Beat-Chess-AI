using System.Collections.Generic;
using UnityEngine;

public class PieceCommandManager : MonoBehaviour
{
    private Queue<ICommand> moveQueue = new Queue<ICommand>();
    [SerializeField] private RoboticArm roboticArm;
    private InvisibleHand invisibleHand;
    private GameManager gameManager;
    private bool isWorking = false;

    void Start()
    {
        GameObject serviceLocator = GameObject.FindGameObjectWithTag("ServiceLocator");
        gameManager = serviceLocator.GetComponentInChildren<GameManager>();
        invisibleHand = serviceLocator.GetComponentInChildren<InvisibleHand>();
    }

    /// <summary>
    /// 각 명령이 끝나면 호출되어 다음 명령을 실행하는 함수
    /// </summary>
    public void ExecuteNextCommand()
    {
        if (isWorking) return;

        if (moveQueue.Count > 0)
        {
            moveQueue.Dequeue().Execute();
            isWorking = true;
        }
        else
        {
            gameManager.EndTurn();
        }
    }

    public void CompleteCommand()
    {
        isWorking = false;
        ExecuteNextCommand();
    }

    public void EnQueueRoboticArmMove(GameObject piece, Vector3 targetPos, float time)
    {
        print("EnQueueRoboticArmMove");
        moveQueue.Enqueue(new RoboticArmCommand(piece, targetPos, time, roboticArm));
        ExecuteNextCommand();
    }

    public void EnQueuePlayerMove(GameObject piece, Vector3 targetPos, float time)
    {
        print("EnQueuePlayerMove");
        moveQueue.Enqueue(new InvisibleHandCommand(piece, targetPos, time, invisibleHand));
        ExecuteNextCommand();
    }
}
