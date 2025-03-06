using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class King : Piece
{
    public override string GetFENchar()
    {
        return isWhite ? "K" : "k";
    }

    public override List<(int, int)> PossibleMove(CheckFrom checkFrom)
    {
        // 내 기물을 조회하고 있는거고, 캐싱된 값이 있다면 그 값을 반환한다
        if (checkFrom == CheckFrom.MySide && cached) { return moves; }

        int x = board.PieceToGridX(gameObject);
        int y = board.PieceToGridY(gameObject);

        int[,] directions = new int[,]
        {
            { 0, 1 },  // Up
            { 0, -1 }, // Down
            { 1, 0 },  // Right
            { -1, 0 }, // Left
            { 1, 1 },  // Up Right
            { -1, 1 }, // Up Left
            { 1, -1 }, // Down Right
            { -1, -1 } // Down Left
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int newX = x + directions[i, 0];
            int newY = y + directions[i, 1];

            if (newX >= 1 && newX <= 8 && newY >= 1 && newY <= 8)
            {
                Piece ps = board.GetPieceScriptAt(newX, newY);
                // 기물이 없거나 다른 편의 기물이라면 이동 가능
                if (ps == null || ps.isWhite != isWhite)
                {
                    moves.Add((newX, newY));
                }
            }
        }

        // 상대방의 기물에 의해 체크되는 경로들을 배제
        if (checkFrom == CheckFrom.MySide) { ExcludeCheckMoves(x, y); }

        // 캐슬링 확인하기 위해 우선 킹이 움직인 적 없는지 확인하고, 내 기물을 조회하는게 맞는지 확인한다
        if (FirstMove && checkFrom == CheckFrom.MySide)
        {
            CheckCastling(moves, x, y);
        }

        cached = true;
        return moves;
    }

    private void CheckCastling(List<(int, int)> moves, int x, int y)
    {
        // 한 번도 움직이지 않은 것이니, 왼쪽으로 4칸에 룩이 있거나 오른쪽으로 3칸에 룩이 있는지 바로 확인
        // 그리고 룩이 한 번도 움직이지 않았는지 확인
        // 그리고 룩과 킹 사이에 기물이 있는지 확인하고, 체크가 되는지 확인
        // 모든 조건을 통과하면 캐슬링 가능
        if (moveValidator.SimulateEnemyCheck(x, y, x, y)) return;

        Rook leftRook = board.GetPieceScriptAt(x - 4, y) as Rook;
        if (leftRook != null && leftRook.FirstMove)
        {
            bool canLeftCastling = true;
            if (board.GetPieceAt(x - 3, y) != null)
            {
                canLeftCastling = false;
            }
            else
            {
                for (int i = 3; i <= 4; i++)
                {
                    if (board.GetPieceAt(i, y) != null || moveValidator.SimulateEnemyCheck(x, y, i, y))
                    {
                        canLeftCastling = false;
                        break;
                    }
                }
            }
            if (canLeftCastling) moves.Add((x - 2, y));
        }

        Rook rightRook = board.GetPieceScriptAt(x + 3, y) as Rook;
        if (rightRook != null && rightRook.FirstMove)
        {
            bool canRightCastling = true;
            for (int i = 6; i <= 7; i++)
            {
                if (board.GetPieceAt(i, y) != null || moveValidator.SimulateEnemyCheck(x, y, i, y))
                {
                    canRightCastling = false;
                    break;
                }
            }
            if (canRightCastling) moves.Add((x + 2, y));
        }
    }
}
