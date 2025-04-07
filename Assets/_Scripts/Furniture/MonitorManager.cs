using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

// 모니터 연출을 담당하는 클래스
public class MonitorManager : MonoBehaviour
{
    private GameManager gameManager;
    private TerminalText terminalText;
    private string[] textStrings;
    private LineEffect[] lineEffects;

    private CinemachineCamera current_vcam;
    private bool isWatchingMonitor = false;
    private float vcam_trans_time;
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private CinemachineCamera playing_vcam;
    [SerializeField] private DeskLight deskLight;
    private ScriptContainer currentScript;
    private GameObject walking_player;

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
    public void StartTerminalSequence(ScriptContainer script, GameObject bumped_player)
    {
        if (bumped_player != null)
        {
            walking_player = bumped_player;
            walking_player.SetActive(false);
        }

        currentScript = script;
        textStrings = script.Texts;
        lineEffects = script.lineEffect;
        textIdx = 0;

        StartCoroutine(TerminalSequence());
    }

    private IEnumerator TerminalSequence()
    {
        // ChangeVcam(monitor_vcam);
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
        if (textIdx == textStrings.Length)
        {
            isWatchingMonitor = false;
            ChangeVcam(playing_vcam);
            StartCoroutine(StartLightOnOff());
            return;
        }

        // LineEffect에 따라 출력 방식 다르게
        if (lineEffects[textIdx] == LineEffect.Continue)
            terminalText.SkipToNewTerminalText(textStrings[textIdx]);
        else
            terminalText.SetTerminalText(textStrings[textIdx]);

        textIdx++;
    }

    private IEnumerator StartLightOnOff()
    {
        deskLight.TurnOffLight();
        gameManager.StartGame(1);
        yield return new WaitForSeconds(3f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        deskLight.TurnOnLight();
    }


}