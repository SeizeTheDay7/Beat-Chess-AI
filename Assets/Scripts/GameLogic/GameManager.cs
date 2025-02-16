using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Board board;
    private MoveValidator moveValidator;
    private InputHandler inputHandler;

    private IGameState currentState; // 현재 활성 상태
    public bool whiteTurn = true; // 턴을 나타내는 변수. whiteTurn이 true면 백(플레이어), false면 흑(AI)


    // 상태 스크립트 인스턴스들
    [HideInInspector] public WhiteTurnState whiteTurnState;
    [HideInInspector] public BlackTurnState blackTurnState;

    [HideInInspector] public PlayerTurnState playerTurnState;
    [HideInInspector] public AITurnState aiTurnState;

    void Start()
    {
        SetComponents();
        board.SetBoard();

        // whiteTurnState = new WhiteTurnState(this);
        // blackTurnState = new BlackTurnState(this);

        playerTurnState = new PlayerTurnState(this);
        aiTurnState = new AITurnState(this);

        currentState = playerTurnState;
    }

    void SetComponents()
    {
        GameObject serviceLocator = GameObject.FindGameObjectWithTag("ServiceLocator");
        board = serviceLocator.GetComponentInChildren<Board>();
        moveValidator = serviceLocator.GetComponentInChildren<MoveValidator>();
        inputHandler = serviceLocator.GetComponentInChildren<InputHandler>();
    }

    void Update()
    {
        currentState.UpdateState();
    }

    public void EndTurn()
    {
        // print("FEN string : " + board.GetFENstring());
        moveValidator.ResetEnPassantCandidates(whiteTurn);
        ChangeState();
    }

    void ChangeState()
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }

        currentState = whiteTurn ? aiTurnState : playerTurnState;
        whiteTurn = !whiteTurn;

        currentState.EnterState();
    }
}