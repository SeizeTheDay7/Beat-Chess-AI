using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Concurrent;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private Process stockfish;
    private StreamWriter input;
    // 별도의 큐를 이용해 이벤트 핸들러에서 받은 메시지를 메인 스레드로 전달
    private ConcurrentQueue<string> outputQueue = new ConcurrentQueue<string>();

    private TerminalText terminalText;
    private InputHandler inputHandler;
    private GameManager gameManager;
    private Board board;
    private string moves = " moves "; // 플레이어의 수를 기록하는 변수
    private Piece nextDeleteTarget; // 다음에 삭제할 기물
    private int nextDeleteTurn; // 다음 삭제 턴
    private int deleteCount = 3; // 삭제 가능 횟수
    [SerializeField] private Material pieceEmissionMaterial;

    void Start()
    {
        GameObject serviceLocator = GameObject.FindGameObjectWithTag("ServiceLocator");
        inputHandler = serviceLocator.GetComponentInChildren<InputHandler>();
        board = serviceLocator.GetComponentInChildren<Board>();
        terminalText = serviceLocator.GetComponentInChildren<TerminalText>();
        gameManager = serviceLocator.GetComponentInChildren<GameManager>();
        StartStockfish();
    }

    public void ResetAIManager()
    {
        ResetDeleteCond();
    }

    void StartStockfish()
    {
        stockfish = new Process();
        stockfish.StartInfo.FileName = Path.Combine(Application.streamingAssetsPath, "stockfish/stockfish-windows-x86-64-avx2.exe");
        stockfish.StartInfo.RedirectStandardInput = true;
        stockfish.StartInfo.RedirectStandardOutput = true;
        stockfish.StartInfo.RedirectStandardError = true;
        stockfish.StartInfo.UseShellExecute = false;
        stockfish.StartInfo.CreateNoWindow = true;

        // 이벤트 핸들러 등록
        stockfish.OutputDataReceived += StockfishOutputHandler;
        stockfish.ErrorDataReceived += StockfishErrorHandler;

        stockfish.Start();

        input = stockfish.StandardInput;

        // 비동기 읽기 시작
        stockfish.BeginOutputReadLine();
        stockfish.BeginErrorReadLine();

        // UCI 프로토콜 초기화
        input.WriteLine("uci");
        input.Flush();
    }

    // StandardOutput에서 데이터가 들어올 때마다 호출됨
    private void StockfishOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (!string.IsNullOrEmpty(outLine.Data))
        {
            outputQueue.Enqueue(outLine.Data);
        }
    }

    // StandardError에서 데이터가 들어올 때마다 호출됨
    private void StockfishErrorHandler(object sendingProcess, DataReceivedEventArgs errLine)
    {
        if (!string.IsNullOrEmpty(errLine.Data))
        {
            outputQueue.Enqueue(errLine.Data);
        }
    }

    void Update()
    {
        // 메인 스레드에서 큐를 확인하고 처리
        while (outputQueue.TryDequeue(out string response))
        {
            // UnityEngine.Debug.Log("Stockfish 응답: " + response);
            ProcessResponse(response);
        }
    }

    /// <summary>
    /// stockfish의 응답에 따라 다르게 처리
    /// </summary>
    void ProcessResponse(string response)
    {
        if (response.Contains("uciok"))
        {
            input.WriteLine("ucinewgame");
            input.Flush();
            UnityEngine.Debug.Log("uciok를 받았고, ucinewgame를 stockfish에게 보냈다.");
        }
        else if (response.StartsWith("bestmove"))
        {
            string bestMove = response.Split(' ')[1];
            moves += " " + bestMove;
            UnityEngine.Debug.Log("Stockfish가 선택한 수: " + bestMove);
            SendAIMoveToGameManager(bestMove);
        }
    }

    public void DebugPlayerMove(int x, int y, int mx, int my)
    {
        string playermove = GridMoveToUCI(x, y, mx, my);
        UnityEngine.Debug.Log("플레이어의 수 : " + playermove);
    }

    /// <summary>
    /// 플레이어의 착수를 Stockfish에게 전달
    /// </summary>
    public void SendPlayerMoveToStockfish()
    {
        // if (gameManager.phase == 2) TryDelete();

        UnityEngine.Debug.Log("플레이어가 둔 후의 FEN : " + board.GetFENstring());

        input.WriteLine("position fen " + board.GetFENstring());
        input.Flush();

        if (gameManager.phase == 1) input.WriteLine("go depth 18");
        else if (gameManager.phase == 2) input.WriteLine("go depth 20");
        else if (gameManager.phase == 3) input.WriteLine("go depth 25");
        input.Flush();
    }

    /// <summary>
    /// 삭제할 턴이 왔다면, 플레이어의 기물을 삭제하고 삭제 횟수를 감소시킴
    /// </summary>
    private void TryDelete()
    {
        if (deleteCount == 0) return;
        if (nextDeleteTurn != board.GetFullMoveCount()) return;
        // if (nextDeleteTarget == null) return;

        board.DestroyPiece(nextDeleteTarget, false);
        DecreaseDeleteCount();
        UpdateNextDeleteCond();
    }

    /// <summary>
    /// Stockfish의 착수를 게임 매니저에게 전달
    /// </summary>
    void SendAIMoveToGameManager(string move)
    {
        if (move == "(none)")
        {
            UnityEngine.Debug.Log("Stockfish가 수를 선택하지 않았습니다.");
            return;
        }
        (int x, int y) = StringToGrid(move.Substring(0, 2));
        (int mx, int my) = StringToGrid(move.Substring(2, 2));
        inputHandler.AIMoveTo(x, y, mx, my);
        UnityEngine.Debug.Log("AI가 둔 후의 FEN : " + board.GetFENstring());
    }

    /// <summary>
    /// 1. 게임이 시작될 때
    /// 2. 목표로 하던 기물이 게임 중 죽었을 때
    /// 3. 예고한 턴에 목표 기물을 제거한 후
    /// 삭제할 기물을 업데이트함
    /// 만약 AI가 삭제 카운트를 모두 소모했다면, 더 이상 삭제하지 못하도록 설정
    /// Cond는 Target과 Trun 모두를 의미
    /// </summary>
    public void UpdateNextDeleteCond()
    {
        if (deleteCount == 0)
        {
            terminalText.SetOriginalText("It's your turn.");
            return;
        }
        SetNextDeleteTurn();
        SetNextDeleteTarget();
        terminalText.SetOriginalText("Next target : " + GetNextDeleteTarget().GetName() + " at Turn " + GetNextDeleteTurn() + "\n" + "It's your turn.");
    }

    private void ResetDeleteCond()
    {
        nextDeleteTurn = 0;
        deleteCount = 3;
        UpdateNextDeleteCond();
    }

    public Piece GetNextDeleteTarget() { return nextDeleteTarget; }
    public int GetNextDeleteTurn() { return nextDeleteTurn; }
    public void DecreaseDeleteCount() { deleteCount--; }

    public void SetNextDeleteTarget()
    {
        nextDeleteTarget = board.GetRandomPlayerPieceScript();
        nextDeleteTarget.GetComponent<Renderer>().material = pieceEmissionMaterial; // 다음 삭제할 기물의 색깔을 바꿈
    }

    private void SetNextDeleteTurn()
    {
        int baseTurn = board.GetFullMoveCount();
        nextDeleteTurn = baseTurn + UnityEngine.Random.Range(5, 7);
    }

    /// <summary>
    /// UCI 프로토콜을 게임 내 그리드 좌표로 변환
    /// </summary>
    (int, int) StringToGrid(string move)
    {
        int x = move[0] - 'a' + 1;
        int y = move[1] - '1' + 1;
        return (x, y);
    }

    /// <summary>
    /// 게임 내 그리드 이동 좌표를 UCI 프로토콜로 변환
    /// </summary>
    string GridMoveToUCI(int x, int y, int mx, int my)
    {
        return GridToUCI(x, y) + GridToUCI(mx, my);
    }

    /// <summary>
    /// 게임 내 그리드 좌표를 UCI 프로토콜 문자열로 변환
    /// </summary>
    public string GridToUCI(int x, int y)
    {
        char xChar = (char)('a' + x - 1);
        char yChar = (char)('1' + y - 1);
        return xChar.ToString() + yChar.ToString();
    }

    /// <summary>
    /// 게임 종료 시 Stockfish 프로세스 종료
    /// </summary>
    void OnApplicationQuit()
    {
        if (stockfish != null)
        {
            try
            {
                if (!stockfish.HasExited)
                {
                    stockfish.Kill();
                }
                stockfish.Dispose();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning("Stockfish 종료 중 오류 발생: " + e.Message);
            }
        }
    }
}
