using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

public class InputHandler : MonoBehaviour
{
    GameObject selectedPiece;
    bool deleteMode;
    int targetLayer;

    private GameManager gameManager;
    private Board board;
    private MoveValidator moveValidator;
    private AIManager aiManager;

    private List<(int, int)> validMoves = new List<(int, int)>();

    void Awake()
    {
        GameObject serviceLocator = GameObject.FindGameObjectWithTag("ServiceLocator");
        gameManager = serviceLocator.GetComponentInChildren<GameManager>();
        board = serviceLocator.GetComponentInChildren<Board>();
        moveValidator = serviceLocator.GetComponentInChildren<MoveValidator>();
        aiManager = serviceLocator.GetComponentInChildren<AIManager>();

        targetLayer = LayerMask.GetMask("Piece");
    }

    /// <summary>
    /// 플레이어가 플레이어 턴에 마우스 좌클하면 호출되어 기물 선택, 이동, 삭제를 처리하는 함수
    /// </summary>
    public void HandleClick(bool whiteTurn)
    {
        GameObject clickedObject = ClickObject();

        if (deleteMode)
        {
            HandleEnemyPieces(whiteTurn, clickedObject);
            return;
        }

        HandleMyPieces(whiteTurn, clickedObject);
    }

    private void HandleEnemyPieces(bool whiteTurn, GameObject clickedObject)
    {
        // 선택한 오브젝트가 없고, 클릭한 오브젝트가 적 기물이라면 삭제
        if (clickedObject != null && clickedObject.GetComponent<Piece>().isWhite != whiteTurn)
        {
            (int, int) clickedGridIdx = board.BoardPosToGridIdx(clickedObject.transform.position);
            board.DestroyPieceAt(clickedGridIdx.Item1, clickedGridIdx.Item2);
        }
        DisableDeleteMode();
    }

    public void EnableDeleteMode()
    {
        deleteMode = true;
    }

    public void DisableDeleteMode()
    {
        deleteMode = false;
    }

    private void HandleMyPieces(bool whiteTurn, GameObject clickedObject)
    {
        // 선택한 오브젝트가 아직 없고, 클릭한 오브젝트가 현재 턴에 맞는 기물이라면 선택
        if (selectedPiece == null && clickedObject != null && clickedObject.GetComponent<Piece>().isWhite == whiteTurn)
        {
            SelectPiece(clickedObject);
        }
        else
        {
            Vector2 clickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 클릭한 씬 위치
            (int, int) clickedGridIdx = board.BoardPosToGridIdx(clickedPos); // 클릭한 칸의 가로 세로 인덱스

            // 선택했던 기물이 있었고, 보드 안에 있는 인덱스고, valid한 이동 인덱스라면 인덱스에 해당하는 위치로 이동
            if (selectedPiece != null && board.IsIdxInBoard(clickedGridIdx) && validMoves.Contains(clickedGridIdx))
            {
                (int x, int y) = board.BoardPosToGridIdx(selectedPiece.transform.position); // 위치를 이동하기 전의 인덱스를 저장해두었다가
                MoveTo(selectedPiece, board.GridIdxToBoardPos(clickedGridIdx));
                aiManager.SendPlayerMoveToStockfish(x, y, clickedGridIdx.Item1, clickedGridIdx.Item2); // AI에게 움직임을 전달
            }

            DeselectPiece(); // 선택을 실패했거나 이동을 해버렸다면 선택 기물 null
        }
    }

    #region 기물 선택
    /// <summary>
    /// 클릭한 기물을 선택하는 함수
    /// </summary>
    void SelectPiece(GameObject clickedObject)
    {
        selectedPiece = clickedObject;
        GetValidMoves(clickedObject);
        board.SetHighlights(validMoves);
    }

    private void GetValidMoves(GameObject selectedPiece)
    {
        Piece pieceScript = selectedPiece.GetComponent<Piece>();
        validMoves = pieceScript.PossibleMove(CheckFrom.MySide);
    }

    /// <summary>
    /// 기물 선택을 취소하는 함수
    /// </summary>
    public void DeselectPiece()
    {
        selectedPiece = null;
        board.RemoveAllHighlights();
    }

    GameObject ClickObject()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f, targetLayer);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }

        return null;
    }
    #endregion

    public void AIMoveTo(int x, int y, int mx, int my)
    {
        GameObject piece = board.GetPieceAt(x, y);
        Vector2 moveto_position = board.GridIdxToBoardPos((mx, my));
        MoveTo(piece, moveto_position);
    }


    void MoveTo(GameObject piece, Vector2 moveto_position)
    {
        Piece pieceScript = piece.GetComponent<Piece>();
        (int, int) piece_pre_idx = board.BoardPosToGridIdx(piece.transform.position);
        int x = piece_pre_idx.Item1;
        int y = piece_pre_idx.Item2;
        (int, int) moveto_grid = board.BoardPosToGridIdx(moveto_position);
        int mx = moveto_grid.Item1;
        int my = moveto_grid.Item2;

        bool destroyed = board.DestroyPieceAt(mx, my); // 기물이 있다면 제거 (같은 편인지는 piece의 스크립트에서 검증되어 들어옴)

        if (pieceScript is Pawn) TryPawnSpecial(ref piece, ref pieceScript, mx, my);
        if (pieceScript is King) TryKingSpecial(pieceScript, mx, my);

        UpdatePieceAndBoard(piece, x, y, moveto_position, mx, my); // Board 배열에 있는 정보 갱신하고 기물 위치 변경

        selectedPiece = null;
        moveValidator.MoveCheckCheck(pieceScript); // 자신의 움직임의 결과가 체크를 발생시켰는지 확인

        // 50수 규칙을 위한 카운트
        if (destroyed || pieceScript is Pawn) board.ResetHalfMoveCount();
        else board.IncreaseHalfMoveCount();

        if (!gameManager.whiteTurn) board.IncreaseFullMoveCount(); // 흑 끝나면 전체 턴 수 증가

        gameManager.EndTurn();
    }


    /// <summary>
    /// 폰이라면 앙파상 설정 혹은 앙파상 행동 여부 검사. 마지막 칸에 도착했다면 승진
    /// </summary>
    private void TryPawnSpecial(ref GameObject piece, ref Piece pieceScript, int mx, int my)
    {
        moveValidator.SetEnPassant(piece, mx, my);
        if (moveValidator.CheckEnpassant(pieceScript as Pawn, mx, my))
        {
            moveValidator.DoEnPassant(pieceScript as Pawn, mx, my);
        }

        piece = TryPromotion(pieceScript as Pawn, my);
        pieceScript = piece.GetComponent<Piece>();
    }

    /// <summary>
    /// 폰이 마지막 칸에 도착했다면 위치를 옮겨주기 전 게임 오브젝트 바꿔치기
    /// </summary>
    private GameObject TryPromotion(Pawn pawn, int my)
    {
        (int, int) idx = board.BoardPosToGridIdx(pawn.transform.position);

        if (pawn.isWhite && my == 8)
        {
            board.DestroyPieceAt(idx.Item1, idx.Item2);
            return Instantiate(board.white_Queen, pawn.transform.position, Quaternion.identity);
        }
        if (!pawn.isWhite && my == 1)
        {
            board.DestroyPieceAt(idx.Item1, idx.Item2);
            return Instantiate(board.black_Queen, pawn.transform.position, Quaternion.identity);
        }
        return pawn.gameObject;
    }

    /// <summary>
    /// 킹이라면 캐슬링 여부 검사
    /// </summary>
    private void TryKingSpecial(Piece pieceScript, int mx, int my)
    {
        // 캐슬링 검사및 배열 위치 변경
        if (moveValidator.TryCastling(pieceScript as King, mx, my))
        {
            // 캐슬링 이후 룩의 오브젝트 위치 변경
            if (mx == 3) board.GetPieceAt(4, my).transform.position = board.GridIdxToBoardPos((4, my));
            else board.GetPieceAt(6, my).transform.position = board.GridIdxToBoardPos((6, my));
        }

        moveValidator.UpdateKingPosition(pieceScript, mx, my);
    }

    /// <summary>
    /// 기물을 이동시키고 보드에 있는 정보를 갱신하는 함수
    /// </summary>
    private void UpdatePieceAndBoard(GameObject piece, int x, int y, Vector2 moveto_position, int mx, int my)
    {
        board.SetPieceAt(piece, mx, my);
        board.SetPieceAt(null, x, y);

        piece.transform.position = moveto_position;
        piece.GetComponent<Piece>().FirstMove = false;
    }
}