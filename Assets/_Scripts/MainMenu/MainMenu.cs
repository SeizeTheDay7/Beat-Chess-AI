using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game"); // MainScene으로 이동
    }

    public void ExitGame()
    {
        Application.Quit(); // 게임 종료
        Debug.Log("QUIT"); // 콘솔에 "QUIT" 출력
    }
}
