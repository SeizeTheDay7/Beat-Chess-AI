using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class MonitorManager : MonoBehaviour
{
    private GameManager gameManager;
    private TerminalText terminalText;
    private string[] introTexts;
    private bool[] introTextType;

    private CinemachineCamera current_vcam;
    private bool isWatchingMonitor = false;
    private float vcam_trans_time;
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private CinemachineCamera playing_vcam;
    [SerializeField] private CinemachineCamera monitor_vcam;
    [SerializeField] private MetalDoor metalDoor;
    private TerminalScript currentScript;
    private GameObject walking_player;
    private CinemachineCamera walking_vcam;

    private int textIdx = 0;

    private void Start()
    {
        GameObject serviceLocator = GameObject.FindGameObjectWithTag("ServiceLocator");
        gameManager = serviceLocator.GetComponentInChildren<GameManager>();
        terminalText = serviceLocator.GetComponentInChildren<TerminalText>();

        vcam_trans_time = brain.DefaultBlend.BlendTime;
    }

    /// <summary>
    /// 호출하여 터미널 시퀀스 시작
    /// </summary>
    public void StartTerminalSequence(TerminalScript script, GameObject bumped_player)
    {
        walking_player = bumped_player;
        walking_player.SetActive(false);
        walking_vcam = walking_player.transform.GetChild(0).GetComponent<CinemachineCamera>();

        currentScript = script;
        introTexts = script.introTexts;
        introTextType = script.introTextType;
        textIdx = 0;

        StartCoroutine(TerminalSequence());
    }

    private IEnumerator TerminalSequence()
    {
        ChangeVcam(monitor_vcam);
        yield return new WaitForSeconds(vcam_trans_time);

        isWatchingMonitor = true;
        NextTerminalTexts();
    }

    /// <summary>
    /// 현재 카메라 우선순위는 낮추고, 새로운 카메라 우선순위를 높임
    /// </summary>
    private void ChangeVcam(CinemachineCamera new_vcam)
    {
        if (current_vcam != null)
            current_vcam.Priority = 0;

        new_vcam.Priority = 10;
        current_vcam = new_vcam;
    }

    /// <summary>
    /// 모니터를 보고 있을 때 엔터를 누르면 다음 텍스트 출력
    /// </summary>
    void Update()
    {
        if (isWatchingMonitor && Input.GetKeyDown(KeyCode.Return))
        {
            NextTerminalTexts();
        }
    }

    private void NextTerminalTexts()
    {
        // 모든 텍스트 출력했다면 flag에 따라 다음 동작 수행
        if (textIdx == introTexts.Length)
        {
            switch (currentScript.introOrEnding)
            {
                case IntroOrEnding.Intro:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    isWatchingMonitor = false;
                    ChangeVcam(playing_vcam);
                    gameManager.StartGame();
                    break;
                case IntroOrEnding.Ending:
                    ChangeVcam(walking_vcam);
                    walking_player.SetActive(true);
                    metalDoor.MetalDoorOpen();
                    Destroy(this);
                    break;
            }

            return;
        }

        // introTextType에 따라 출력 방식 다르게
        if (introTextType[textIdx])
            terminalText.SkipToNewTerminalText(introTexts[textIdx]);
        else
            terminalText.SetTerminalText(introTexts[textIdx]);

        textIdx++;
    }


}