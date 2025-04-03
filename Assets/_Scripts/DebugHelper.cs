using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    [SerializeField] private ElevatorDoor elevatorDoor;
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
            gameManager.NextStage();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            // inputHandler.deleteMode = true;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            elevatorDoor.ElevatorDoorOpen();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            elevatorDoor.ElevatorDoorClose();
        }

        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     elevatorDoor.SetVacant();
        // }

        // if (Input.GetKeyDown(KeyCode.B))
        // {
        //     elevatorDoor.SetOcccupied();
        // }

    }
}
