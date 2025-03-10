using UnityEngine;
using System.Collections.Generic;

public class MoveCountHelper : MonoBehaviour
{
    private GameManager gameManager;
    private Board board;

    private void Awake()
    {
        board = GameObject.FindGameObjectWithTag("ServiceLocator").GetComponentInChildren<Board>();
    }

    public List<(int, int)> MoveUp(GameManager gameManager, bool isWhite, int x, int y)
    {
        List<(int, int)> moves = new List<(int, int)>();

        for (int i = y + 1; i <= 8; i++)
        {
            Piece ps = board.GetPieceScriptAt(x, i);
            if (ps == null)
            {
                moves.Add((x, i));
            }
            else
            {
                if (ps.isWhite != isWhite)
                {
                    moves.Add((x, i));
                }
                break;
            }
        }

        return moves;
    }

    public List<(int, int)> MoveDown(GameManager gameManager, bool isWhite, int x, int y)
    {
        List<(int, int)> moves = new List<(int, int)>();

        for (int i = y - 1; i >= 1; i--)
        {
            Piece ps = board.GetPieceScriptAt(x, i);
            if (ps == null)
            {
                moves.Add((x, i));
            }
            else
            {
                if (ps.isWhite != isWhite)
                {
                    moves.Add((x, i));
                }
                break;
            }
        }

        return moves;
    }

    public List<(int, int)> MoveLeft(GameManager gameManager, bool isWhite, int x, int y)
    {
        List<(int, int)> moves = new List<(int, int)>();

        for (int i = x - 1; i >= 1; i--)
        {
            Piece ps = board.GetPieceScriptAt(i, y);
            if (ps == null)
            {
                moves.Add((i, y));
            }
            else
            {
                if (ps.isWhite != isWhite)
                {
                    moves.Add((i, y));
                }
                break;
            }
        }

        return moves;
    }

    public List<(int, int)> MoveRight(GameManager gameManager, bool isWhite, int x, int y)
    {
        List<(int, int)> moves = new List<(int, int)>();

        for (int i = x + 1; i <= 8; i++)
        {
            Piece ps = board.GetPieceScriptAt(i, y);
            if (ps == null)
            {
                moves.Add((i, y));
            }
            else
            {
                if (ps.isWhite != isWhite)
                {
                    moves.Add((i, y));
                }
                break;
            }
        }

        return moves;
    }

    public List<(int, int)> MoveUpRight(GameManager gameManager, bool isWhite, int x, int y)
    {
        List<(int, int)> moves = new List<(int, int)>();

        for (int i = 1; x + i <= 8 && y + i <= 8; i++)
        {
            Piece ps = board.GetPieceScriptAt(x + i, y + i);
            if (ps == null)
            {
                moves.Add((x + i, y + i));
            }
            else
            {
                if (ps.isWhite != isWhite)
                {
                    moves.Add((x + i, y + i));
                }
                break;
            }
        }

        return moves;
    }

    public List<(int, int)> MoveUpLeft(GameManager gameManager, bool isWhite, int x, int y)
    {
        List<(int, int)> moves = new List<(int, int)>();

        for (int i = 1; x - i >= 1 && y + i <= 8; i++)
        {
            Piece ps = board.GetPieceScriptAt(x - i, y + i);
            if (ps == null)
            {
                moves.Add((x - i, y + i));
            }
            else
            {
                if (ps.isWhite != isWhite)
                {
                    moves.Add((x - i, y + i));
                }
                break;
            }
        }

        return moves;
    }


    public List<(int, int)> MoveDownRight(GameManager gameManager, bool isWhite, int x, int y)
    {
        List<(int, int)> moves = new List<(int, int)>();

        for (int i = 1; x + i <= 8 && y - i >= 1; i++)
        {
            Piece ps = board.GetPieceScriptAt(x + i, y - i);
            if (ps == null)
            {
                moves.Add((x + i, y - i));
            }
            else
            {
                if (ps.isWhite != isWhite)
                {
                    moves.Add((x + i, y - i));
                }
                break;
            }
        }

        return moves;
    }

    public List<(int, int)> MoveDownLeft(GameManager gameManager, bool isWhite, int x, int y)
    {
        List<(int, int)> moves = new List<(int, int)>();

        for (int i = 1; x - i >= 1 && y - i >= 1; i++)
        {
            Piece p = board.GetPieceScriptAt(x - i, y - i);
            if (p == null)
            {
                moves.Add((x - i, y - i));
            }
            else
            {
                if (p.isWhite != isWhite)
                {
                    moves.Add((x - i, y - i));
                }
                break;
            }
        }

        return moves;
    }

}
