using UnityEngine;

public class EnterRoomTrigger : MonoBehaviour
{
    [SerializeField] private MetalDoor metalDoor;
    [SerializeField] private float waitTime = 2.0f;
    private bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        metalDoor.MetalDoorClose();
        Destroy(gameObject);
    }
}
