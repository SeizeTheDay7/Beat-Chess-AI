using System.Collections.Generic;

public class Rook : Piece
{
    public override string GetFENchar()
    {
        return isWhite ? "R" : "r";
    }

    public override List<(int, int)> PossibleMove(CheckFrom checkFrom)
    {
        // 내 기물을 조회하고 있는거고, 캐싱된 값이 있다면 그 값을 반환한다
        if (checkFrom == CheckFrom.MySide && cached) { return moves; }
        // 남의 기물을 조회하고 있는 거라면 킹에 의한 변수를 제거하기 위해 매번 재계산한다
        if (checkFrom == CheckFrom.OtherSide) { moves.Clear(); }

        int x = board.PieceToGridX(gameObject);
        int y = board.PieceToGridY(gameObject);

        moves.AddRange(moveCountHelper.MoveUp(gameManager, isWhite, x, y)); // Move up
        moves.AddRange(moveCountHelper.MoveDown(gameManager, isWhite, x, y)); // Move down
        moves.AddRange(moveCountHelper.MoveRight(gameManager, isWhite, x, y)); // Move right
        moves.AddRange(moveCountHelper.MoveLeft(gameManager, isWhite, x, y)); // Move left

        if (checkFrom == CheckFrom.MySide) { ExcludeCheckMoves(x, y); }

        cached = true;
        return moves;
    }
}