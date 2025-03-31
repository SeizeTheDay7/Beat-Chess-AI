using UnityEngine;

public class EnterRoomTrigger : MonoBehaviour
{
    [SerializeField] private MetalDoor metalDoor;
    [SerializeField] private BGM_Player bgmPlayer;
    private bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        metalDoor.MetalDoorClose();
        bgmPlayer.StopBGM();
        Destroy(gameObject);
    }
}
