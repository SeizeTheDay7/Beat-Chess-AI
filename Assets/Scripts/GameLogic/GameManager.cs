using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Board board;
    private MoveValidator moveValidator;
    private InputHandler inputHandler;
    private TerminalText terminalText;
    [SerializeField] private AudioSource stageClaerSound;
    [SerializeField] private AudioSource gameClearSound;
    [SerializeField] private RoboticArm roboticArm;

    private IGameState currentState; // 현재 활성 상태
    public bool whiteTurn = true; // 턴을 나타내는 변수. whiteTurn이 true면 백(플레이어), false면 흑(AI)
    [SerializeField] private Canvas gameoverCanvas; // 게임 오버 캔버스
    [SerializeField] private Canvas endingCanvas; // 엔딩 캔버스
    [SerializeField] Shooter shooter;
    [SerializeField] Light spotLight;


    // 상태 스크립트 인스턴스들
    [HideInInspector] public WhiteTurnState whiteTurnState;
    [HideInInspector] public BlackTurnState blackTurnState;

    [HideInInspector] public PlayerTurnState playerTurnState;
    [HideInInspector] public AITurnState aiTurnState;
    [HideInInspector] public WaitingState waitingState;

    private int stage = 1;

    void Start()
    {
        SetComponents();

        playerTurnState = new PlayerTurnState(this);
        aiTurnState = new AITurnState(this);
        waitingState = new WaitingState(roboticArm);

        currentState = waitingState;
    }

    void SetComponents()
    {
        GameObject serviceLocator = GameObject.FindGameObjectWithTag("ServiceLocator");
        board = serviceLocator.GetComponentInChildren<Board>();
        moveValidator = serviceLocator.GetComponentInChildren<MoveValidator>();
        inputHandler = serviceLocator.GetComponentInChildren<InputHandler>();
        terminalText = serviceLocator.GetComponentInChildren<TerminalText>();
    }

    void Update()
    {
        currentState.UpdateState();
    }

    public void EndTurn()
    {
        // print("FEN string : " + board.GetFENstring());
        moveValidator.ResetEnPassantCandidates(whiteTurn);
        ChangeState();
    }

    void ChangeState()
    {
        if (currentState != null) currentState.ExitState();

        currentState = whiteTurn ? aiTurnState : playerTurnState;
        whiteTurn = !whiteTurn;

        currentState.EnterState();
    }

    public void ChangeToWaitingState()
    {
        currentState = waitingState;
        waitingState.EnterState();
    }

    public bool IsWaitingState()
    {
        return currentState == waitingState;
    }

    /// <summary>
    /// 맨 처음 게임 시작할 때 호출되는 함수
    /// </summary>
    public void StartGame()
    {
        currentState = playerTurnState;
        terminalText.BackToOriginalText();
        board.SetBoard();
    }

    /// <summary>
    /// 다음 스테이지로 넘어가는 함수
    /// </summary>
    public void NextStage()
    {
        spotLight.enabled = false;
        ChangeToWaitingState();
        terminalText.SetTerminalText("Stage Clear!");

        stage++;

        if (stage > 3)
        {
            gameClearSound.Play();
            endingCanvas.gameObject.SetActive(true);
            return;
        }

        stageClaerSound.Play();

        Invoke("ResetGame", 3f);
    }

    /// <summary>
    /// 게임 오버 후에 restart 눌렀을 때 호출
    /// </summary>
    public void RestartGame()
    {
        ResetGame();
        gameoverCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// 새 게임을 시작하기 위해 정보를 초기화하는 함수
    /// </summary>
    public void ResetGame()
    {
        whiteTurn = true;
        currentState = playerTurnState;
        spotLight.enabled = true;

        board.ResetBoard();
        inputHandler.ResetInputHandler(stage);
        terminalText.BackToOriginalTextWith("Stage " + stage);
    }

    /// <summary>
    /// 종 치거나 게임 지면 호출되는 함수
    /// </summary>
    public void GameOver()
    {
        ChangeToWaitingState();
        shooter.PlayShooterFootstep();
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}