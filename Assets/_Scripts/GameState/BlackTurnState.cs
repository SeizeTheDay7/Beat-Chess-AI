using UnityEngine;

public class BlackTurnState : IGameState
{
    private GameManager gameManager;
    private MoveValidator moveValidator;

    public BlackTurnState(GameManager gameManager)
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
        // Debug.Log("Black Turn Update");

        // if (Input.GetMouseButtonDown(0))
        // {
        //     bool moved = gameManager.HandleClick();

        //     if (moved)
        //     {
        //         gameManager.whiteTurn = true;
        //         gameManager.ChangeState(gameManager.whiteTurnState);
        //     }
        // }
    }

    public void ExitState()
    {
        // Debug.Log("Exit Black Turn");
    }
}