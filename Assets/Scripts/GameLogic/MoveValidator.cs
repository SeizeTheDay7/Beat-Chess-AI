using UnityEngine;
using System;
using System.Collections.Generic;

public class MoveValidator : MonoBehaviour
{
    private Board board;

    public (int, int) whiteEnPassantCandidate = (-1, -1); // 백에서 앙파상 가능한 위치
    public (int, int) blackEnPassantCandidate = (-1, -1); // 흑에서 앙파상 가능한 위치
    public bool anyValidMove { get; private set; } = false;
    public (int, int) blackKingPosition = (5, 8);
    public (int, int) whiteKingPosition = (5, 1);
    public bool isBlackKingChecked = false;
    public bool isWhiteKingChecked = false;


    void Awake()
    {
        GameObject serviceLocator = GameObject.FindGameObjectWithTag("ServiceLocator");
        board = serviceLocator.GetComponentInChildren<Board>();
    }

    /// <summary>
    /// 캐슬링 이동이라면 캐슬링에 맞게 배열을 수정하는 함수
    /// </summary>
    public bool TryCastling(King king, int mx, int my)
    {
        if (!king.FirstMove) return false; // 첫 이동이 아니라면 캐슬링 불가능

        // 처음 움직인 것인데 특정한 x축 좌표로 이동했다면 캐슬링
        // 캐슬링을 검증할 때 TryCastling이 호출될 때도 있으므로, 혹시 기물이 이미 있지는 않은지 확인한다
        // 킹은 이후 로직에서 이동할 예정이므로, 룩의 좌표만 이동시켜준다 (룩 오브젝트의 위치는 MoveTo로 돌아가서 바꿈)
        if (mx == 3)
        {
            if (board.GetPieceAt(4, my) != null) return false; // 퀸이 있다면 캐슬링 시뮬 불가
            board.SetPieceAt(board.GetPieceAt(1, my), 4, my);
            board.SetPieceAt(null, 1, my);
            return true;
        }
        if (mx == 7)
        {
            board.SetPieceAt(board.GetPieceAt(8, my), 6, my);
            board.SetPieceAt(null, 8, my);
            return true;
        }

        return false;
    }

    /// <summary>
    /// pawn이 움직여 앙파상을 당할 수 있는 상황을 기록
    /// </summary>
    public void SetEnPassant(GameObject pawn, int mx, int my)
    {
        Pawn pawnScript = pawn.GetComponent<Pawn>();

        // 첫 이동이고, 두 칸을 움직일 예정이라면 앙파상이 가능할 위치를 설정
        if (pawnScript.FirstMove && Math.Abs(my - board.PieceToGridY(pawn)) == 2)
        {
            if (pawnScript.isWhite) blackEnPassantCandidate = (mx, my - 1);
            else whiteEnPassantCandidate = (mx, my + 1);
        }
    }

    /// <summary>
    /// pawn의 움직임으로 앙파상이 가능한지 체크
    /// </summary>
    public bool CheckEnpassant(Pawn pawn, int mx, int my)
    {
        if (pawn.isWhite && whiteEnPassantCandidate == (mx, my)) return true;
        if (!pawn.isWhite && blackEnPassantCandidate == (mx, my)) return true;
        return false;
    }

    /// <summary>
    /// 앙파상 실행하여 폰 파괴
    /// </summary>
    public void DoEnPassant(Pawn my_pawn, int mx, int my)
    {
        if (my_pawn.isWhite) { board.DestroyPieceAt(mx, my - 1); }
        else { board.DestroyPieceAt(mx, my + 1); }
        print("Destroyed with EnPassant");
    }

    /// <summary>
    /// 자신의 모든 기물들의 움직임을 미리 계산하는 함수
    /// </summary>
    public void CalculateMyMoves(bool whiteTurn)
    {
        anyValidMove = false;
        for (int i = 1; i <= 8; i++)
        {
            for (int j = 1; j <= 8; j++)
            {
                Piece pieceScript = board.GetPieceScriptAt(i, j);
                if (pieceScript != null && pieceScript.isWhite == whiteTurn)
                {
                    // print("CaculateMyMoves :: PossibleMove 호출");
                    if (pieceScript.PossibleMove(CheckFrom.MySide).Count != 0)
                    {
                        anyValidMove = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 특정 위치에 있는 기물이 새 위치로 이동하면 체크될 것인지 확인해보는 함수
    /// </summary>
    /// <param name="x">기존 x좌표</param>
    /// <param name="z">기존 z좌표</param>
    /// <param name="newx">이동해볼 x좌표</param>
    /// <param name="newz">이동해볼 z좌표</param>
    /// <returns>킹이 체크된다면 true 반환</returns>
    public bool SimulateEnemyCheck(int x, int z, int newx, int newz)
    {
        bool isCheckedMove = false; // 체크되는지 여부
        bool didCastling = false; // 캐슬링 여부
        bool didEnPassant = false; // 앙파상 여부

        GameObject piece = board.GetPieceAt(x, z); // 원래 위치의 기물
        Piece pieceScript = board.GetPieceScriptAt(x, z); // 원래 위치의 기물의 스크립트
        bool isWhite = pieceScript.isWhite; // 원래 위치의 기물의 편
        GameObject temp = board.GetPieceAt(newx, newz); // 이동할 위치에 기물이 있다면 임시로 저장해놓는다
        GameObject temp_pawn = null;

        // 앙파상이라면 앙파상 처리, 캐슬링이라면 캐슬링으로 처리해줘야 함. 그 외의 경우는 그냥 이동
        if (pieceScript is Pawn) didEnPassant = CheckEnpassant(pieceScript as Pawn, newx, newz);
        if (pieceScript is King) didCastling = TryCastling(pieceScript as King, newx, newz);
        if (didEnPassant)
        {
            temp_pawn = board.GetPieceAt(newx, newz - 1);
            board.SetPieceAt(null, newx, newz - 1);
        }

        // 원래 위치의 기물을 없애고, 이동할 위치에 기물을 놓는다
        board.SetPieceAt(null, x, z);
        board.SetPieceAt(piece, newx, newz);

        (int, int) kingPosition = isWhite ? whiteKingPosition : blackKingPosition; // 현재 킹의 위치를 가져온다
        if (pieceScript is King) kingPosition = (newx, newz); // 킹이 자신의 이동을 점검하려는 거면 킹의 위치를 수정


        // 모든 적 기물들이 킹의 위치로 이동할 수 있는지 확인
        for (int i = 1; i <= 8; i++)
        {
            for (int j = 1; j <= 8; j++)
            {
                Piece enemyScript = board.GetPieceScriptAt(i, j);
                if (enemyScript != null && enemyScript.isWhite != isWhite)
                {
                    List<(int, int)> moves = enemyScript.PossibleMove(CheckFrom.OtherSide);
                    if (moves.Contains(kingPosition))
                    {
                        isCheckedMove = true;
                        break;
                    }
                }
            }
        }

        // 시뮬레이션을 마쳤다면 원래대로 되돌린다
        board.SetPieceAt(temp, newx, newz);
        board.SetPieceAt(piece, x, z);

        // 앙파상 했었다면 폰을 되살린다
        if (didEnPassant)
        {
            board.SetPieceAt(temp_pawn, newx, newz - 1);
        }
        // 캐슬링 했었다면 룩을 되돌려놓는다
        if (didCastling)
        {
            if (newx == 3)
            {
                board.SetPieceAt(board.GetPieceAt(4, newz), 1, newz);
                board.SetPieceAt(null, 4, newz);
            }
            if (newx == 7)
            {
                board.SetPieceAt(board.GetPieceAt(6, newz), 8, newz);
                board.SetPieceAt(null, 6, newz);
            }
        }

        return isCheckedMove;
    }

    /// <summary>
    /// 자신의 움직임이 체크를 발생시키는지 확인하는 함수
    /// </summary>
    /// <param name="pieceScript"></param>
    public void MoveCheckCheck(Piece pieceScript)
    {
        var kingPosition = pieceScript.isWhite ? blackKingPosition : whiteKingPosition;
        if (pieceScript.PossibleMove(CheckFrom.OtherSide).Contains(kingPosition))
        {
            if (pieceScript.isWhite)
                isBlackKingChecked = true;
            else
                isWhiteKingChecked = true;
            print("Check!");
        }
        else
        {
            if (pieceScript.isWhite)
                isBlackKingChecked = false;
            else
                isWhiteKingChecked = false;
            print("Not Check!");
        }
    }


    /// <summary>
    /// 킹의 현재 위치를 업데이트하는 함수
    /// </summary>
    public void UpdateKingPosition(Piece pieceScript, int mx, int mz)
    {
        if (pieceScript.isWhite)
        {
            whiteKingPosition = (mx, mz);
        }
        else
        {
            blackKingPosition = (mx, mz);
        }
    }

    /// <summary>
    /// 모든 기물의 이동 가능한 위치를 초기화하는 함수
    /// </summary>
    public void ResetAllPieceMoves()
    {
        for (int i = 1; i <= 8; i++)
        {
            for (int j = 1; j <= 8; j++)
            {
                Piece pieceScript = board.GetPieceScriptAt(i, j);
                if (pieceScript != null)
                {
                    pieceScript.ResetMoves();
                }
            }
        }
    }

    public void ResetEnPassantCandidates(bool whiteTurn)
    {
        if (whiteTurn) whiteEnPassantCandidate = (-1, -1);
        else blackEnPassantCandidate = (-1, -1);
    }
}