using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public List<(int, int)> moves = new List<(int, int)>();
    public bool cached = false;
    public bool isWhite;
    public bool FirstMove = true;
    protected GameManager gameManager;
    protected MoveCountHelper moveCountHelper;
    protected Board board;
    protected MoveValidator moveValidator;
    void Awake()
    {
        GameObject serviceLocator = GameObject.FindGameObjectWithTag("ServiceLocator");
        board = serviceLocator.GetComponentInChildren<Board>();
        moveCountHelper = serviceLocator.GetComponentInChildren<MoveCountHelper>();
        moveValidator = serviceLocator.GetComponentInChildren<MoveValidator>();
    }
    public abstract string GetFENchar();
    public abstract List<(int, int)> PossibleMove(CheckFrom checkFrom);


    /// <summary>
    /// 이동 가능한 경로 중 적에게 체크가 되는 경로를 제거한다.
    /// </summary>
    public void ExcludeCheckMoves(int x, int z)
    {
        moves.RemoveAll(move => moveValidator.SimulateEnemyCheck(x, z, move.Item1, move.Item2));
    }

    public void ResetMoves()
    {
        moves.Clear();
        cached = false;
    }
}
