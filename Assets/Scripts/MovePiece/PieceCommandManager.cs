using System.Collections.Generic;
using UnityEngine;

public class PieceCommandManager : MonoBehaviour
{
    private Queue<PieceMoveCommand> moveQueue = new Queue<PieceMoveCommand>();
    [SerializeField] private RoboticArm roboticArm;
    private GameManager gameManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject serviceLocator = GameObject.FindGameObjectWithTag("ServiceLocator");
        gameManager = serviceLocator.GetComponentInChildren<GameManager>();
    }

    /// <summary>
    /// 각 명령이 끝나면 호출되어 다음 명령을 실행하는 함수
    /// </summary>
    public void ExecuteNextCommand()
    {
        if (moveQueue.Count > 0)
        {
            moveQueue.Dequeue().Execute();
        }
        else
        {
            gameManager.EndTurn();
        }
    }

    public void EnQueueMoveCommand(GameObject piece, Vector3 targetPos, float time)
    {
        PieceMoveCommand pieceMoveCommand = new PieceMoveCommand(piece, targetPos, time, roboticArm);
        moveQueue.Enqueue(pieceMoveCommand);
        if (moveQueue.Count == 1) ExecuteNextCommand();
    }
}
