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

    private InputHandler inputHandler;
    private Board board;
    private string moves = " moves "; // 플레이어의 수를 기록하는 변수



    void Start()
    {
        GameObject serviceLocator = GameObject.FindGameObjectWithTag("ServiceLocator");
        inputHandler = serviceLocator.GetComponentInChildren<InputHandler>();
        board = serviceLocator.GetComponentInChildren<Board>();
        StartStockfish();
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

    /// <summary>
    /// 플레이어의 착수를 Stockfish에게 전달
    /// </summary>
    public void SendPlayerMoveToStockfish(int x, int y, int mx, int my)
    {
        string playermove = GridMoveToUCI(x, y, mx, my);
        UnityEngine.Debug.Log("플레이어의 수 : " + playermove);
        // moves += " " + playermove;

        // input.WriteLine("position startpos" + moves);

        UnityEngine.Debug.Log("플레이어가 둔 후의 FEN : " + board.GetFENstring());

        input.WriteLine("position fen " + board.GetFENstring());
        input.Flush();

        input.WriteLine("go depth 10");
        input.Flush();
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
