using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    [Header("판떼기")]
    [SerializeField] GameObject board_object;

    [Header("스폰할 오브젝트들")]
    [SerializeField] GameObject highlight;
    [SerializeField] GameObject black_Pawn;
    [SerializeField] GameObject black_Rook;
    [SerializeField] GameObject black_Knight;
    [SerializeField] GameObject black_Bishop;
    public GameObject black_Queen;
    [SerializeField] GameObject black_King;
    [SerializeField] GameObject white_Pawn;
    [SerializeField] GameObject white_Rook;
    [SerializeField] GameObject white_Knight;
    [SerializeField] GameObject white_Bishop;
    public GameObject white_Queen;
    [SerializeField] GameObject white_King;

    private MoveValidator moveValidator;
    private AIManager aiManager;

    public const int BOARD_SIZE = 8;
    private float TILE_WIDTH_ORIGIN;
    private float TILE_HEIGHT_ORIGIN;
    private float TILE_DW;
    private float TILE_DH;
    private GameObject[,] pieces = new GameObject[BOARD_SIZE + 1, BOARD_SIZE + 1];
    private GameObject[,] highlights = new GameObject[BOARD_SIZE + 1, BOARD_SIZE + 1];

    private int halfMoveCount = 0;
    private int fullMoveCount = 1;


    // 혼동을 방지하기 위해, 모든 그리드는 (int, int)로 표현하고, 씬 좌표는 Vector2로 표현한다

    void Awake()
    {
        GameObject serviceLocator = GameObject.FindGameObjectWithTag("ServiceLocator");
        moveValidator = serviceLocator.GetComponentInChildren<MoveValidator>();
        aiManager = serviceLocator.GetComponentInChildren<AIManager>();
    }

    public void SetBoard()
    {
        TILE_WIDTH_ORIGIN = board_object.transform.position.x;
        TILE_HEIGHT_ORIGIN = board_object.transform.position.y;
        SpriteRenderer board_sr = board_object.GetComponent<SpriteRenderer>();
        TILE_DW = board_sr.bounds.size.x / 16;
        TILE_DH = board_sr.bounds.size.y / 16;

        for (int i = 1; i <= 8; i++)
        {
            for (int j = 1; j <= 8; j++)
            {
                highlights[i, j] = Instantiate(highlight, GridIdxToBoardPos((i, j)), Quaternion.identity);
                highlights[i, j].SetActive(false);
            }
        }

        for (int i = 1; i <= 8; i++)
        {
            pieces[i, 2] = Instantiate(white_Pawn, GridIdxToBoardPos((i, 2)), Quaternion.identity);
            pieces[i, 7] = Instantiate(black_Pawn, GridIdxToBoardPos((i, 7)), Quaternion.identity);
        }

        pieces[1, 1] = Instantiate(white_Rook, GridIdxToBoardPos((1, 1)), Quaternion.identity);
        pieces[2, 1] = Instantiate(white_Knight, GridIdxToBoardPos((2, 1)), Quaternion.identity);
        pieces[3, 1] = Instantiate(white_Bishop, GridIdxToBoardPos((3, 1)), Quaternion.identity);
        pieces[4, 1] = Instantiate(white_Queen, GridIdxToBoardPos((4, 1)), Quaternion.identity);
        pieces[5, 1] = Instantiate(white_King, GridIdxToBoardPos((5, 1)), Quaternion.identity);
        pieces[6, 1] = Instantiate(white_Bishop, GridIdxToBoardPos((6, 1)), Quaternion.identity);
        pieces[7, 1] = Instantiate(white_Knight, GridIdxToBoardPos((7, 1)), Quaternion.identity);
        pieces[8, 1] = Instantiate(white_Rook, GridIdxToBoardPos((8, 1)), Quaternion.identity);

        pieces[1, 8] = Instantiate(black_Rook, GridIdxToBoardPos((1, 8)), Quaternion.identity);
        pieces[2, 8] = Instantiate(black_Knight, GridIdxToBoardPos((2, 8)), Quaternion.identity);
        pieces[3, 8] = Instantiate(black_Bishop, GridIdxToBoardPos((3, 8)), Quaternion.identity);
        pieces[4, 8] = Instantiate(black_Queen, GridIdxToBoardPos((4, 8)), Quaternion.identity);
        pieces[5, 8] = Instantiate(black_King, GridIdxToBoardPos((5, 8)), Quaternion.identity);
        pieces[6, 8] = Instantiate(black_Bishop, GridIdxToBoardPos((6, 8)), Quaternion.identity);
        pieces[7, 8] = Instantiate(black_Knight, GridIdxToBoardPos((7, 8)), Quaternion.identity);
        pieces[8, 8] = Instantiate(black_Rook, GridIdxToBoardPos((8, 8)), Quaternion.identity);
    }

    /// <summary>
    /// 가로 인덱스 x, 세로 인덱스 y에 해당하는 씬 위치 반환
    /// </summary>
    public Vector2 GridIdxToBoardPos((int, int) idx)
    {
        int x = idx.Item1;
        int y = idx.Item2;
        float x_pos = TILE_WIDTH_ORIGIN + (2 * x - 1) * TILE_DW;
        float y_pos = TILE_HEIGHT_ORIGIN + (2 * y - 1) * TILE_DH;
        return new Vector2(x_pos, y_pos);
    }

    /// <summary>
    /// 씬에서의 위치 좌표에 해당하는 가로 인덱스 x, 세로 인덱스 y 반환
    /// </summary>
    public (int, int) BoardPosToGridIdx(Vector2 pos)
    {
        int x = (int)((pos.x - TILE_WIDTH_ORIGIN) / (2 * TILE_DW) + 1);
        int y = (int)((pos.y - TILE_HEIGHT_ORIGIN) / (2 * TILE_DH) + 1);
        return (x, y);
    }

    /// <summary>
    /// 해당 오브젝트의 x 인덱스 반환
    /// </summary>
    public int PieceToGridX(GameObject piece)
    {
        return BoardPosToGridIdx(piece.transform.position).Item1;
    }

    /// <summary>
    /// 해당 오브젝트의 y 인덱스 반환
    /// </summary>
    public int PieceToGridY(GameObject piece)
    {
        return BoardPosToGridIdx(piece.transform.position).Item2;
    }

    /// ------------------------------------------------------------------------------------------------------------------------------
    // 이 아래부터는 전부 그리드 인덱스 기반으로 작성된 함수들
    /// ------------------------------------------------------------------------------------------------------------------------------

    public bool IsIdxInBoard((int, int) idx)
    {
        return idx.Item1 >= 1 && idx.Item1 <= 8 && idx.Item2 >= 1 && idx.Item2 <= 8;
    }

    public void SetPieceAt(GameObject piece, int x, int y)
    {
        pieces[x, y] = piece;
    }

    public bool DestroyPieceAt(int x, int y)
    {
        if (pieces[x, y] == null) { return false; }
        Destroy(pieces[x, y]);
        return true;
    }

    public GameObject GetPieceAt(int x, int y)
    {
        if (x < 1 || x > 8 || y < 1 || y > 8)
        {
            return null;
        }

        if (pieces[x, y] == null)
        {
            return null;
        }

        return pieces[x, y];
    }

    public Piece GetPieceScriptAt(int x, int y)
    {
        if (x < 1 || x > 8 || y < 1 || y > 8)
        {
            return null;
        }

        if (pieces[x, y] == null)
        {
            return null;
        }

        return pieces[x, y].GetComponent<Piece>();
    }

    public void SetHighlights(List<(int, int)> moves)
    {
        foreach (var move in moves)
        {
            highlights[move.Item1, move.Item2].SetActive(true);
        }
    }

    public void RemoveHighlight(int x, int y)
    {
        highlights[x, y].SetActive(false);
    }

    public void RemoveAllHighlights()
    {
        for (int i = 1; i <= 8; i++)
        {
            for (int j = 1; j <= 8; j++)
            {
                highlights[i, j].SetActive(false);
            }
        }
    }


    /// ------------------------------------------------------------------------------------------------------------------------------
    /// FEN 관련 함수들
    /// ------------------------------------------------------------------------------------------------------------------------------

    public string GetFENstring()
    {
        string fen = "";
        fen += FEN_Pieces() + " ";
        fen += "b "; // FEN string은 AI만 사용하므로, 무조건 b로 기록
        fen += FEN_Castling() + " ";
        fen += FEN_EnPassant() + " ";
        fen += FEN_HalfMoveClock() + " ";
        fen += FEN_FullMoveNumber();
        return fen;
    }

    private string FEN_Pieces()
    {
        string fen_pieces = "";

        for (int y = 8; y >= 1; y--)
        {
            int empty = 0;
            for (int x = 1; x <= 8; x++)
            {
                if (GetPieceAt(x, y) == null)
                {
                    empty++;
                }
                else
                {
                    if (empty != 0)
                    {
                        fen_pieces += empty.ToString();
                        empty = 0;
                    }
                    fen_pieces += GetPieceAt(x, y).GetComponent<Piece>().GetFENchar();
                }
            }
            if (empty != 0)
            {
                fen_pieces += empty.ToString();
            }
            if (y != 1)
            {
                fen_pieces += "/";
            }
        }

        return fen_pieces;
    }

    private string FEN_Castling()
    {
        string fen_castling = "";
        if (BothFirstMove((5, 1), (8, 1))) fen_castling += "K";
        if (BothFirstMove((5, 1), (1, 1))) fen_castling += "Q";
        if (BothFirstMove((5, 8), (8, 8))) fen_castling += "k";
        if (BothFirstMove((5, 8), (1, 8))) fen_castling += "q";
        if (fen_castling == "") fen_castling = "-";
        return fen_castling;
    }

    private bool BothFirstMove((int, int) p1, (int, int) p2)
    {
        Piece p1_script = GetPieceScriptAt(p1.Item1, p1.Item2);
        Piece p2_script = GetPieceScriptAt(p2.Item1, p2.Item2);
        if (p1_script == null || p2_script == null) return false;
        return p1_script.FirstMove && p2_script.FirstMove;
    }

    private string FEN_EnPassant()
    {
        string fen_enpassant = "";
        (int ep_x, int ep_y) = moveValidator.blackEnPassantCandidate;
        if ((ep_x, ep_y) != (-1, -1)) fen_enpassant = aiManager.GridToUCI(ep_x, ep_y);
        else fen_enpassant = "-";
        return fen_enpassant;
    }

    private string FEN_HalfMoveClock()
    {
        return halfMoveCount.ToString();
    }

    public void ResetHalfMoveCount()
    {
        halfMoveCount = 0;
    }

    public void IncreaseHalfMoveCount()
    {
        halfMoveCount++;
    }

    private string FEN_FullMoveNumber()
    {
        return fullMoveCount.ToString();
    }

    public void ResetFullMoveCount()
    {
        fullMoveCount = 1;
    }

    public void IncreaseFullMoveCount()
    {
        fullMoveCount++;
    }
}