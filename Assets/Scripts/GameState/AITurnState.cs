using UnityEngine;

public class AITurnState : IGameState
{
    private GameManager gameManager;
    private MoveValidator moveValidator;

    public AITurnState(GameManager gameManager)
    {
        this.gameManager = gameManager;
        moveValidator = gameManager.transform.parent.GetComponentInChildren<MoveValidator>();
    }

    public void EnterState()
    {
        Debug.Log("Entering Black Turn");
        moveValidator.ResetAllPieceMoves(); // 캐싱했던 데이터를 전부 리셋하고
        moveValidator.CalculateMyMoves(gameManager.whiteTurn); // 미리 경로를 전부 계산해놓는다

        if (!moveValidator.anyValidMove)
        {
            if (moveValidator.isBlackKingChecked)
            {
                Debug.Log("체크메이트!");
            }
            else
            {
                Debug.Log("스테일메이트!");
            }
        }
    }

    public void UpdateState()
    {
        // Debug.Log("White Turn Update");

        // if (Input.GetMouseButtonDown(0))
        // {
        //     bool moved = gameManager.HandleClick();

        //     if (moved)
        //     {
        //         gameManager.whiteTurn = false;
        //         gameManager.ChangeState(gameManager.blackTurnState);
        //     }
        // }
    }

    public void ExitState()
    {
        // Debug.Log("Exit White Turn");
    }
}