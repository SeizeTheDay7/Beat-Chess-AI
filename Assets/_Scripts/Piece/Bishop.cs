using System.Collections.Generic;

public class Bishop : Piece
{
    public override string GetName()
    {
        return "Bishop";
    }

    public override string GetFENchar()
    {
        return isWhite ? "B" : "b";
    }

    public override List<(int, int)> PossibleMove(CheckFrom checkFrom)
    {
        // 내 기물을 조회하고 있는거고, 캐싱된 값이 있다면 그 값을 반환한다
        if (checkFrom == CheckFrom.MySide && cached) { return moves; }
        // 남의 기물을 조회하고 있는 거라면 킹에 의한 변수를 제거하기 위해 매번 재계산한다
        if (checkFrom == CheckFrom.OtherSide) { moves.Clear(); }

        int x = board.PieceToGridX(gameObject);
        int y = board.PieceToGridY(gameObject);

        moves.AddRange(moveCountHelper.MoveUpRight(gameManager, isWhite, x, y)); // Move up right
        moves.AddRange(moveCountHelper.MoveUpLeft(gameManager, isWhite, x, y)); // Move up left
        moves.AddRange(moveCountHelper.MoveDownRight(gameManager, isWhite, x, y)); // Move down right
        moves.AddRange(moveCountHelper.MoveDownLeft(gameManager, isWhite, x, y)); // Move down left

        if (checkFrom == CheckFrom.MySide) { ExcludeCheckMoves(x, y); }

        cached = true;
        return moves;
    }
}