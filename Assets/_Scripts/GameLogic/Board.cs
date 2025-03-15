using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("판떼기")]
    [SerializeField] GameObject board_object;
    [SerializeField] GameObject board_pivot;
    [SerializeField] PieceGrave piece_grave;

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
    private GameManager gameManager;
    private PieceCommandManager pieceCommandManager;
    private InvisibleHand invisibleHand;

    public const int BOARD_SIZE = 8;
    private float TILE_Z_ORIGIN;
    private float TILE_Y_ORIGIN;
    private float TILE_X_ORIGIN;
    private float TILE_DX;
    private float TILE_DZ;
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
        gameManager = serviceLocator.GetComponentInChildren<GameManager>();
        pieceCommandManager = serviceLocator.GetComponentInChildren<PieceCommandManager>();
        invisibleHand = serviceLocator.GetComponentInChildren<InvisibleHand>();
    }

    public void SetBoard()
    {
        TILE_X_ORIGIN = board_pivot.transform.position.x;
        TILE_Y_ORIGIN = board_pivot.transform.position.y + 0.01f;
        TILE_Z_ORIGIN = board_pivot.transform.position.z;
        MeshRenderer board_mr = board_object.GetComponent<MeshRenderer>();
        TILE_DX = board_mr.bounds.size.x / 16;
        TILE_DZ = board_mr.bounds.size.z / 16;

        piece_grave.InitPieceGrave(2 * TILE_DX, 2 * TILE_DZ);

        if (highlights[1, 1] == null)
        {
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 8; j++)
                {
                    highlights[i, j] = Instantiate(highlight, GridIdxToBoardPos((i, j)), Quaternion.Euler(90, 0, 0));
                    highlights[i, j].SetActive(false);
                }
            }
        }

        for (int i = 1; i <= 8; i++)
        {
            pieces[i, 2] = Instantiate(white_Pawn, GridIdxToBoardPos((i, 2)), Quaternion.Euler(-90, 0, 0));
            pieces[i, 7] = Instantiate(black_Pawn, GridIdxToBoardPos((i, 7)), Quaternion.Euler(-90, 0, 0));
        }

        pieces[1, 1] = Instantiate(white_Rook, GridIdxToBoardPos((1, 1)), Quaternion.Euler(-90, 0, 0));
        pieces[2, 1] = Instantiate(white_Knight, GridIdxToBoardPos((2, 1)), Quaternion.Euler(-90, 0, 0));
        pieces[3, 1] = Instantiate(white_Bishop, GridIdxToBoardPos((3, 1)), Quaternion.Euler(-90, 0, 0));
        pieces[4, 1] = Instantiate(white_Queen, GridIdxToBoardPos((4, 1)), Quaternion.Euler(-90, 0, 0));
        pieces[5, 1] = Instantiate(white_King, GridIdxToBoardPos((5, 1)), Quaternion.Euler(-90, 0, 0));
        pieces[6, 1] = Instantiate(white_Bishop, GridIdxToBoardPos((6, 1)), Quaternion.Euler(-90, 0, 0));
        pieces[7, 1] = Instantiate(white_Knight, GridIdxToBoardPos((7, 1)), Quaternion.Euler(-90, 0, 0));
        pieces[8, 1] = Instantiate(white_Rook, GridIdxToBoardPos((8, 1)), Quaternion.Euler(-90, 0, 0));

        pieces[1, 8] = Instantiate(black_Rook, GridIdxToBoardPos((1, 8)), Quaternion.Euler(-90, 0, 0));
        pieces[2, 8] = Instantiate(black_Knight, GridIdxToBoardPos((2, 8)), Quaternion.Euler(-90, 0, 0));
        pieces[3, 8] = Instantiate(black_Bishop, GridIdxToBoardPos((3, 8)), Quaternion.Euler(-90, 0, 0));
        pieces[4, 8] = Instantiate(black_Queen, GridIdxToBoardPos((4, 8)), Quaternion.Euler(-90, 0, 0));
        pieces[5, 8] = Instantiate(black_King, GridIdxToBoardPos((5, 8)), Quaternion.Euler(-90, 0, 0));
        pieces[6, 8] = Instantiate(black_Bishop, GridIdxToBoardPos((6, 8)), Quaternion.Euler(-90, 0, 0));
        pieces[7, 8] = Instantiate(black_Knight, GridIdxToBoardPos((7, 8)), Quaternion.Euler(-90, 0, 0));
        pieces[8, 8] = Instantiate(black_Rook, GridIdxToBoardPos((8, 8)), Quaternion.Euler(-90, 0, 0));
    }

    public void ResetBoard()
    {
        for (int i = 1; i <= 8; i++)
        {
            for (int j = 1; j <= 8; j++)
            {
                if (pieces[i, j] != null)
                {
                    Destroy(pieces[i, j]);
                }
                highlights[i, j].SetActive(false);
            }
        }
        moveValidator.ResetBoardInfo();
        piece_grave.ResetGrave();
        SetBoard();
    }

    /// <summary>
    /// 가로 인덱스 i, 세로 인덱스 j에 해당하는 씬 위치 반환
    /// </summary>
    public Vector3 GridIdxToBoardPos((int, int) idx)
    {
        int i = idx.Item1;
        int j = idx.Item2;
        float x_pos = TILE_X_ORIGIN + (2 * i - 1) * TILE_DX;
        float y_pos = TILE_Y_ORIGIN;
        float z_pos = TILE_Z_ORIGIN + (2 * j - 1) * TILE_DZ;
        return new Vector3(x_pos, y_pos, z_pos);
    }

    /// <summary>
    /// 씬에서의 위치 좌표에 해당하는 가로 인덱스 x, 세로 인덱스 y 반환
    /// </summary>
    public (int, int) BoardPosToGridIdx(Vector3 pos)
    {
        int x = (int)((pos.x - TILE_X_ORIGIN) / (2 * TILE_DX) + 1);
        int z = (int)((pos.z - TILE_Z_ORIGIN) / (2 * TILE_DZ) + 1);
        return (x, z);
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

    public void MovePieceAt(GameObject piece, int x, int z)
    {
        if (gameManager.whiteTurn)
        {
            pieceCommandManager.EnQueuePlayerMove(piece, GridIdxToBoardPos((x, z)), 0.25f);
        }
        else
        {
            pieceCommandManager.EnQueueRoboticArmMove(piece, GridIdxToBoardPos((x, z)), 0.4f);
        }
    }

    public bool DestroyPieceAt(int x, int z, bool isTurnEnd = true)
    {
        if (pieces[x, z] == null) { return false; }

        bool isWhiteNow = gameManager.whiteTurn; // 백이 플레이어

        if (!isTurnEnd) // 버튼을 누른 거라면 턴을 종료하지 않고 무덤에만 보내기
        {
            invisibleHand.OnlyMovePieceToPos(pieces[x, z], piece_grave.GetPlayerGravePos(pieces[x, z]), 0.25f);
        }
        else
        {
            if (isWhiteNow)
            {
                pieceCommandManager.EnQueuePlayerMove(pieces[x, z], piece_grave.GetPlayerGravePos(pieces[x, z]), 0.25f);
            }
            else
            {
                pieceCommandManager.EnQueueRoboticArmMove(pieces[x, z], piece_grave.GetAIGravePos(pieces[x, z]), 0.4f);
            }
        }


        pieces[x, z] = null;

        return true;
    }

    public GameObject GetPieceAt(int x, int z)
    {
        if (x < 1 || x > 8 || z < 1 || z > 8)
        {
            return null;
        }

        if (pieces[x, z] == null)
        {
            return null;
        }

        return pieces[x, z];
    }

    public Piece GetPieceScriptAt(int x, int z)
    {
        if (x < 1 || x > 8 || z < 1 || z > 8)
        {
            return null;
        }

        if (pieces[x, z] == null)
        {
            return null;
        }

        return pieces[x, z].GetComponent<Piece>();
    }

    public void SetHighlights(List<(int, int)> moves)
    {
        foreach (var move in moves)
        {
            highlights[move.Item1, move.Item2].SetActive(true);
        }
    }

    public void RemoveHighlight(int x, int z)
    {
        highlights[x, z].SetActive(false);
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

    public int GetHalfMoveCount()
    {
        return halfMoveCount;
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