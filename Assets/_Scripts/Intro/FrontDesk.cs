using UnityEngine;

public class FrontDesk : MonoBehaviour
{
    [SerializeField] private IntroManager introManager;

    private void OnTriggerEnter(Collider other)
    {
        introManager.StartTerminalSequence();
        Destroy(this);
    }
}
