using UnityEngine;

public class SecondMatchTrigger : MonoBehaviour
{
    [SerializeField] private ScriptContainer secondMatchScript;
    [SerializeField] private MonitorManager monitorManager;
    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        // monitorManager.StartTerminalSequence(secondMatchScript, other.gameObject);
        Destroy(gameObject);
    }
}
