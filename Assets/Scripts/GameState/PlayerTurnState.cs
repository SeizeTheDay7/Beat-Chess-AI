using UnityEngine;

public class PlayerTurnState : IGameState
{
    private GameManager gameManager;
    private MoveValidator moveValidator;
    private InputHandler inputHandler;

    public PlayerTurnState(GameManager gameManager)
    {
        this.gameManager = gameManager;
        moveValidator = gameManager.transform.parent.GetComponentInChildren<MoveValidator>();
        inputHandler = gameManager.transform.parent.GetComponentInChildren<InputHandler>();
    }

    public void EnterState()
    {
        Debug.Log("Entering White Turn");
        moveValidator.ResetAllPieceMoves(); // 캐싱했던 데이터를 전부 리셋하고
        moveValidator.CalculateMyMoves(gameManager.whiteTurn); // 미리 경로를 전부 계산해놓는다

        if (!moveValidator.anyValidMove)
        {
            if (moveValidator.isWhiteKingChecked)
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
        // Debug.Log("Player Turn Update");

        if (Input.GetMouseButtonDown(0))
        {
            inputHandler.HandleClick(gameManager.whiteTurn);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            inputHandler.EnableDeleteMode();
        }
    }

    public void ExitState()
    {
        // Debug.Log("Exit Player Turn");
    }
}