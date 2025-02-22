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

            print("기물 삭제 : " + clickedObject.name);

            // 체크메이트 당했을 경우를 대비하여 모든 기물의 이동 가능한 위치를 초기화
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 8; j++)
                {
                    GameObject piece = board.GetPieceAt(i, j);
                    if (piece != null)
                    {
                        piece.GetComponent<Piece>().ResetMoves();
                    }
                }
            }
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
            print("기물 선택 : " + clickedObject.name);
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Board")))
            {
                Vector3 clickedPos = hit.point; // 클릭한 씬 위치
                (int, int) clickedGridIdx = board.BoardPosToGridIdx(clickedPos); // 클릭한 칸의 가로 세로 인덱스
                print("클릭한 위치 : " + clickedGridIdx);

                // 선택했던 기물이 있었고, 보드 안에 있는 인덱스고, valid한 이동 인덱스라면 인덱스에 해당하는 위치로 이동
                if (selectedPiece != null && board.IsIdxInBoard(clickedGridIdx) && validMoves.Contains(clickedGridIdx))
                {
                    (int x, int z) = board.BoardPosToGridIdx(selectedPiece.transform.position); // 위치를 이동하기 전의 인덱스를 저장해두었다가
                    MoveTo(selectedPiece, board.GridIdxToBoardPos(clickedGridIdx));
                    aiManager.SendPlayerMoveToStockfish(x, z, clickedGridIdx.Item1, clickedGridIdx.Item2); // AI에게 움직임을 전달
                }
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            return hit.collider.gameObject;
        }

        return null;
    }
    #endregion

    public void AIMoveTo(int x, int z, int mx, int mz)
    {
        GameObject piece = board.GetPieceAt(x, z);
        Vector3 moveto_position = board.GridIdxToBoardPos((mx, mz));
        MoveTo(piece, moveto_position);
    }


    void MoveTo(GameObject piece, Vector3 moveto_position)
    {
        Piece pieceScript = piece.GetComponent<Piece>();
        (int, int) piece_pre_idx = board.BoardPosToGridIdx(piece.transform.position);
        int x = piece_pre_idx.Item1;
        int z = piece_pre_idx.Item2;
        (int, int) moveto_grid = board.BoardPosToGridIdx(moveto_position);
        int mx = moveto_grid.Item1;
        int mz = moveto_grid.Item2;

        bool destroyed = board.DestroyPieceAt(mx, mz); // 기물이 있다면 제거 (같은 편인지는 piece의 스크립트에서 검증되어 들어옴)

        if (pieceScript is Pawn) TryPawnSpecial(ref piece, ref pieceScript, mx, mz);
        if (pieceScript is King) TryKingSpecial(pieceScript, mx, mz);

        UpdatePieceAndBoard(piece, x, z, moveto_position, mx, mz); // Board 배열에 있는 정보 갱신하고 기물 위치 변경

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
    private void TryPawnSpecial(ref GameObject piece, ref Piece pieceScript, int mx, int mz)
    {
        moveValidator.SetEnPassant(piece, mx, mz);
        if (moveValidator.CheckEnpassant(pieceScript as Pawn, mx, mz))
        {
            moveValidator.DoEnPassant(pieceScript as Pawn, mx, mz);
        }

        piece = TryPromotion(pieceScript as Pawn, mz);
        pieceScript = piece.GetComponent<Piece>();
    }

    /// <summary>
    /// 폰이 마지막 칸에 도착했다면 위치를 옮겨주기 전 게임 오브젝트 바꿔치기
    /// </summary>
    private GameObject TryPromotion(Pawn pawn, int mz)
    {
        (int, int) idx = board.BoardPosToGridIdx(pawn.transform.position);

        if (pawn.isWhite && mz == 8)
        {
            board.DestroyPieceAt(idx.Item1, idx.Item2);
            return Instantiate(board.white_Queen, pawn.transform.position, Quaternion.Euler(-90, 0, 0));
        }
        if (!pawn.isWhite && mz == 1)
        {
            board.DestroyPieceAt(idx.Item1, idx.Item2);
            return Instantiate(board.black_Queen, pawn.transform.position, Quaternion.Euler(-90, 0, 0));
        }
        return pawn.gameObject;
    }

    /// <summary>
    /// 킹이라면 캐슬링 여부 검사
    /// </summary>
    private void TryKingSpecial(Piece pieceScript, int mx, int mz)
    {
        // 캐슬링 검사및 배열 위치 변경
        if (moveValidator.TryCastling(pieceScript as King, mx, mz))
        {
            // 캐슬링 이후 룩의 오브젝트 위치 변경
            if (mx == 3) board.GetPieceAt(4, mz).transform.position = board.GridIdxToBoardPos((4, mz));
            else board.GetPieceAt(6, mz).transform.position = board.GridIdxToBoardPos((6, mz));
        }

        moveValidator.UpdateKingPosition(pieceScript, mx, mz);
    }

    /// <summary>
    /// 기물을 이동시키고 보드에 있는 정보를 갱신하는 함수
    /// </summary>
    private void UpdatePieceAndBoard(GameObject piece, int x, int z, Vector3 moveto_position, int mx, int mz)
    {
        board.SetPieceAt(piece, mx, mz);
        board.SetPieceAt(null, x, z);

        piece.transform.position = moveto_position;
        piece.GetComponent<Piece>().FirstMove = false;
    }
}