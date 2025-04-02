using UnityEngine;

public class InnerRoomLineTrigger : MonoBehaviour
{
    [SerializeField] private BGM_Player bgmPlayer;
    [SerializeField] private LightManipulate lightManipulate;
    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        bgmPlayer.PermanentStopBGM();
        lightManipulate.TurnOnLight();
        Destroy(gameObject);
    }
}
