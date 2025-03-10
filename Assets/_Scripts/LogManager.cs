using UnityEngine;
using System;
using System.IO;

public class LogManager : MonoBehaviour
{
    private string logFilePath;

    void Awake()
    {
        // 로그 파일 경로 설정
        logFilePath = Path.Combine(Application.persistentDataPath, "GameLog_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt");

        // 기존 로그 파일이 있으면 삭제
        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
        }

        // Unity 로그 이벤트 핸들러 등록
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy()
    {
        // 핸들러 해제 (메모리 누수 방지)
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // 로그 메시지 생성
        string logEntry = $"{DateTime.Now:HH:mm:ss} [{type}] {logString}\n";
        if (type == LogType.Exception || type == LogType.Error)
        {
            logEntry += $"StackTrace:\n{stackTrace}\n";
        }

        // 로그 파일에 기록
        File.AppendAllText(logFilePath, logEntry);
    }
}
