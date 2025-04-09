using UnityEngine;

public class FrontDeskTrigger : MonoBehaviour
{
    [SerializeField] private ScriptContainer introScript;
    [SerializeField] private MonitorManager MonitorManager;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        triggered = true;
        MonitorManager.StartTerminalSequence(ScriptType.Intro, other.gameObject);
        Destroy(gameObject);
    }
}
