using System.Collections.Generic;

public class Knight : Piece
{
    public override string GetName()
    {
        return "Knight";
    }

    public override string GetFENchar()
    {
        return isWhite ? "N" : "n";
    }

    public override List<(int, int)> PossibleMove(CheckFrom checkFrom)
    {
        // 내 기물을 조회하고 있는거고, 캐싱된 값이 있다면 그 값을 반환한다
        if (checkFrom == CheckFrom.MySide && cached) { return moves; }
        // 남의 기물을 조회하고 있는 거라면 킹에 의한 변수를 제거하기 위해 매번 재계산한다
        if (checkFrom == CheckFrom.OtherSide) { moves.Clear(); }

        int x = board.PieceToGridX(gameObject);
        int y = board.PieceToGridY(gameObject);

        int[,] offsets = new int[,]
        {
            { 1, 2 }, { -1, 2 }, { 1, -2 }, { -1, -2 },
            { 2, 1 }, { 2, -1 }, { -2, 1 }, { -2, -1 }
        };

        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            int newX = x + offsets[i, 0];
            int newY = y + offsets[i, 1];

            if (newX >= 1 && newX <= 8 && newY >= 1 && newY <= 8)
            {
                Piece ps = board.GetPieceScriptAt(newX, newY);
                if (ps == null || ps.isWhite != isWhite)
                {
                    moves.Add((newX, newY));
                }
            }
        }

        if (checkFrom == CheckFrom.MySide) { ExcludeCheckMoves(x, y); }

        cached = true;
        return moves;
    }
}