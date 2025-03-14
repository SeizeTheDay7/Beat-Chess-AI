using UnityEngine;
using System.Collections;
public class OpenDoorTrigger : MonoBehaviour
{
    [SerializeField] private MetalDoor metalDoor;
    [SerializeField] private float waitTime = 1f;
    private bool triggerd = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggerd) return;
        triggerd = true;
        StartCoroutine(OpenDoorCoroutine());
    }

    private IEnumerator OpenDoorCoroutine()
    {
        yield return new WaitForSeconds(waitTime);

        float buzzDelayTime = metalDoor.PlayBuzzer();
        yield return new WaitForSeconds(buzzDelayTime + 0.1f);

        float doorOpenTime = metalDoor.MetalDoorOpen();
        yield return new WaitForSeconds(doorOpenTime);

        Destroy(gameObject);
    }
}
