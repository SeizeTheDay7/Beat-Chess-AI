using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override string GetFENchar()
    {
        return isWhite ? "P" : "p";
    }

    public override List<(int, int)> PossibleMove(CheckFrom checkFrom)
    {
        // 내 기물을 조회하고 있는거고, 캐싱된 값이 있다면 그 값을 반환한다
        if (checkFrom == CheckFrom.MySide && cached)
        {
            // Debug.Log("Cached moves");
            return moves;
        }
        // 남의 기물을 조회하고 있는 거라면 킹에 의한 변수를 제거하기 위해 매번 재계산한다
        if (checkFrom == CheckFrom.OtherSide) { moves.Clear(); }

        int x = board.PieceToGridX(gameObject);
        int y = board.PieceToGridY(gameObject);

        if (isWhite)
        {
            if (x != 1) CheckLeftUpDiagonal(checkFrom, x, y);
            if (x != 8) CheckRightUpDiagonal(checkFrom, x, y);

            Piece p = board.GetPieceScriptAt(x, y + 1);
            if (p == null && checkFrom == CheckFrom.MySide) moves.Add((x, y + 1));

            if (FirstMove)
            {
                Piece p2 = board.GetPieceScriptAt(x, y + 2);
                if (p == null && p2 == null && checkFrom == CheckFrom.MySide) moves.Add((x, y + 2));
            }
        }
        else
        {
            if (x != 1) CheckLeftDownDiagonal(checkFrom, x, y);
            if (x != 8) CheckRightDownDiagonal(checkFrom, x, y);

            Piece p = board.GetPieceScriptAt(x, y - 1);
            if (p == null && checkFrom == CheckFrom.MySide) moves.Add((x, y - 1));

            if (FirstMove)
            {
                Piece p2 = board.GetPieceScriptAt(x, y - 2);
                if (p == null && p2 == null && checkFrom == CheckFrom.MySide) moves.Add((x, y - 2));
            }
        }

        if (checkFrom == CheckFrom.MySide) { ExcludeCheckMoves(x, y); }

        cached = true;
        return moves;
    }

    private void CheckLeftUpDiagonal(CheckFrom checkFrom, int x, int y)
    {
        Piece p = board.GetPieceScriptAt(x - 1, y + 1);

        if ((p != null && p.isWhite != isWhite)
            || moveValidator.whiteEnPassantCandidate == (x - 1, y + 1)
            || checkFrom == CheckFrom.OtherSide) moves.Add((x - 1, y + 1));
    }

    private void CheckRightUpDiagonal(CheckFrom checkFrom, int x, int y)
    {
        Piece p = board.GetPieceScriptAt(x + 1, y + 1);

        if ((p != null && p.isWhite != isWhite) || moveValidator.whiteEnPassantCandidate == (x + 1, y + 1)
            || checkFrom == CheckFrom.OtherSide) moves.Add((x + 1, y + 1));
    }

    private void CheckLeftDownDiagonal(CheckFrom checkFrom, int x, int y)
    {
        Piece p = board.GetPieceScriptAt(x - 1, y - 1);

        if ((p != null && p.isWhite != isWhite) || moveValidator.blackEnPassantCandidate == (x - 1, y - 1)
            || checkFrom == CheckFrom.OtherSide) moves.Add((x - 1, y - 1));
    }

    private void CheckRightDownDiagonal(CheckFrom checkFrom, int x, int y)
    {
        Piece p = board.GetPieceScriptAt(x + 1, y - 1);

        if ((p != null && p.isWhite != isWhite) || moveValidator.blackEnPassantCandidate == (x + 1, y - 1)
            || checkFrom == CheckFrom.OtherSide) moves.Add((x + 1, y - 1));
    }
}

