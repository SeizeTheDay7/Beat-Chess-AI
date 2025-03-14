using System.Collections;
using UnityEngine;

public class FrontDeskTrigger : MonoBehaviour
{
    [SerializeField] private IntroManager introManager;
    [SerializeField] private float waitTime = 1f;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        triggered = true;
        StartCoroutine(FrontDeskCoroutine());
    }

    private IEnumerator FrontDeskCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
        introManager.StartTerminalSequence();
        Destroy(gameObject);
    }
}
