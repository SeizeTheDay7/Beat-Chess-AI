using UnityEngine;
using System.Collections;

public class OpenDoorTrigger : MonoBehaviour
{
    [SerializeField] private MetalDoor metalDoor;
    [SerializeField] private DeskLight deskLight;
    [SerializeField] private GameObject human;
    [SerializeField] private float waitTime_peek = 2f;
    [SerializeField] private float waitTime_dooropen = 1f;
    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        StartCoroutine(DoorOpenCoroutine());
    }

    private IEnumerator DoorOpenCoroutine()
    {
        yield return new WaitForSeconds(waitTime_peek);
        deskLight.TurnOffLight();
        Destroy(human);
        yield return new WaitForSeconds(waitTime_dooropen);
        metalDoor.MetalDoorOpen();
        Destroy(gameObject);
    }
}
