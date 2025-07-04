using UnityEngine;

public class AITurnState : IGameState
{
    private GameManager gameManager;
    private MoveValidator moveValidator;
    private Board board;
    private TerminalText terminalText;
    private AIManager aiManager;

    public AITurnState(GameManager gameManager)
    {
        this.gameManager = gameManager;
        moveValidator = gameManager.transform.parent.GetComponentInChildren<MoveValidator>();
        board = gameManager.transform.parent.GetComponentInChildren<Board>();
        terminalText = gameManager.transform.parent.GetComponentInChildren<TerminalText>();
        aiManager = gameManager.transform.parent.GetComponentInChildren<AIManager>();
    }

    public void EnterState()
    {
        terminalText.SetTerminalText("AI Turn...");

        Debug.Log("Entering Black Turn");
        moveValidator.ResetAllPieceMoves(); // 캐싱했던 데이터를 전부 리셋하고
        moveValidator.CalculateMyMoves(gameManager.whiteTurn); // 미리 경로를 전부 계산해놓는다

        if (board.GetHalfMoveCount() >= 50)
        {
            Debug.Log("50수 룰로 인한 패배");
        }

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

            gameManager.NextStage();
            return;
        }

        if (moveValidator.NoPieceExceptMyKing(gameManager.whiteTurn))
        {
            Debug.Log("킹만 남았음");
            gameManager.NextStage();
            return;
        }

        aiManager.SendPlayerMoveToStockfish();
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