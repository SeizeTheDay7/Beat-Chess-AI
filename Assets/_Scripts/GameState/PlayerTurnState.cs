using UnityEngine;

public class PlayerTurnState : IGameState
{
    private GameManager gameManager;
    private MoveValidator moveValidator;
    private InputHandler inputHandler;
    private Board board;
    private TerminalText terminalText;

    public PlayerTurnState(GameManager gameManager)
    {
        this.gameManager = gameManager;
        moveValidator = gameManager.transform.parent.GetComponentInChildren<MoveValidator>();
        inputHandler = gameManager.transform.parent.GetComponentInChildren<InputHandler>();
        board = gameManager.transform.parent.GetComponentInChildren<Board>();
        terminalText = gameManager.transform.parent.GetComponentInChildren<TerminalText>();
    }

    public void EnterState()
    {
        terminalText.BackToOriginalText();

        Debug.Log("Entering White Turn");
        moveValidator.ResetAllPieceMoves(); // 캐싱했던 데이터를 전부 리셋하고
        moveValidator.CalculateMyMoves(gameManager.whiteTurn); // 미리 경로를 전부 계산해놓는다

        if (board.GetHalfMoveCount() >= 50)
        {
            Debug.Log("50 수 룰로 인한 패배");
        }

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

            gameManager.GameOver();
        }

        if (moveValidator.NoPieceExceptMyKing(gameManager.whiteTurn))
        {
            Debug.Log("킹만 남았음");
            gameManager.GameOver();
        }
    }

    public void UpdateState()
    {
        // Debug.Log("Player Turn Update");

        if (Input.GetMouseButtonDown(0))
        {
            inputHandler.HandleClick(gameManager.whiteTurn);
        }
    }

    public void ExitState()
    {
        // Debug.Log("Exit Player Turn");
    }
}