using UnityEngine;

public class FrontDeskTrigger : MonoBehaviour
{
    [SerializeField] private TerminalScript introScript;
    [SerializeField] private MonitorManager MonitorManager;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        triggered = true;
        MonitorManager.StartTerminalSequence(introScript, other.gameObject);
        Destroy(gameObject);
    }
}
