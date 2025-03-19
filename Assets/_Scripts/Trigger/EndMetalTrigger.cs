using UnityEngine;

public class EndMetalTrigger : MonoBehaviour
{
    [SerializeField] private TerminalScript endingScript;
    [SerializeField] private MonitorManager monitorManager;
    [SerializeField] private MetalDoor metalDoor;
    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        monitorManager.StartTerminalSequence(endingScript, other.gameObject);
        Destroy(gameObject);
    }
}
