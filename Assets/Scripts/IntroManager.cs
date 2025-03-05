using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Playables;
using DG.Tweening;
using System.Collections.Generic;

public class IntroManager : MonoBehaviour
{
    private GameManager gameManager;
    private TerminalText terminalText;

    private CinemachineCamera current_vcam;
    private bool isWatchingMonitor = false;
    private float vcam_trans_time;
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private CinemachineCamera player_vacm;
    [SerializeField] private CinemachineCamera monitor_vcam;
    [SerializeField] private string[] introTexts; // bool이 true면 기존 텍스트와 함께 출력
    [SerializeField] private bool[] introTextsType; // true면 기존 텍스트와 함께 출력
    private int textIdx = 0;

    private void Start()
    {
        GameObject serviceLocator = GameObject.FindGameObjectWithTag("ServiceLocator");
        gameManager = serviceLocator.GetComponentInChildren<GameManager>();
        terminalText = serviceLocator.GetComponentInChildren<TerminalText>();

        vcam_trans_time = brain.DefaultBlend.BlendTime;

        StartIntro();
    }

    void Update()
    {
        if (isWatchingMonitor && Input.GetKeyDown(KeyCode.Return))
        {
            NextTerminalTexts();
        }
    }

    private void NextTerminalTexts()
    {
        if (textIdx == introTexts.Length)
        {
            isWatchingMonitor = false;
            ChangeVcam(player_vacm);
            gameManager.StartGame();
            Destroy(this);
            return;
        }

        if (introTextsType[textIdx])
            terminalText.AddTerminalText(introTexts[textIdx]);
        else
            terminalText.SetTerminalText(introTexts[textIdx]);

        textIdx++;
    }

    private void StartIntro()
    {
        // 천이 벗겨짐, 카메라가 모니터 앞으로 감, 모니터에 대사 출력, 엔터 누르면 다음 대사, 마지막 대사 뜬 후에 카메라가 돌아오면 게임 시작돼있음.

        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() => ChangeVcam(monitor_vcam));

        seq.AppendInterval(vcam_trans_time);
        seq.AppendCallback(() =>
        {
            isWatchingMonitor = true;
            NextTerminalTexts();
        });
    }

    private void ChangeVcam(CinemachineCamera vcam)
    {
        if (current_vcam != null)
            current_vcam.Priority = 0;

        vcam.Priority = 10;
        current_vcam = vcam;
    }
}