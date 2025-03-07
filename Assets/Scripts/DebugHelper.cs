using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    [SerializeField] private RoboticArm roboticArm;
    GameManager gameManager;
    InputHandler inputHandler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject serviceLocator = GameObject.FindGameObjectWithTag("ServiceLocator");
        gameManager = serviceLocator.GetComponentInChildren<GameManager>();
        inputHandler = serviceLocator.GetComponentInChildren<InputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            // gameManager.NextStage();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            // inputHandler.deleteMode = true;
        }

    }
}
