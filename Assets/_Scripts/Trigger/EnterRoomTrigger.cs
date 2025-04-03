using UnityEngine;

public class EnterRoomTrigger : MonoBehaviour
{
    [SerializeField] private ElevatorDoor elevatorDoor;
    private bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        elevatorDoor.ElevatorDoorClose();
        Destroy(gameObject);
    }
}
