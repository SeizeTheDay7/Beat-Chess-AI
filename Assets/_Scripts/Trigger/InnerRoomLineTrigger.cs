using UnityEngine;

public class InnerRoomLineTrigger : MonoBehaviour
{
    [SerializeField] private BGM_Player bgmPlayer;
    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        bgmPlayer.PermanentStopBGM();
        Destroy(gameObject);
    }
}
