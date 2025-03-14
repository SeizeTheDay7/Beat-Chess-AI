using UnityEngine;
using System.Collections;

public class EnterRoomTrigger : MonoBehaviour
{
    [SerializeField] private MetalDoor metalDoor;
    private bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        StartCoroutine(EnterRoomCoroutine());
    }

    private IEnumerator EnterRoomCoroutine()
    {
        float doorCloseTime = metalDoor.MetalDoorClose();
        yield return new WaitForSeconds(doorCloseTime);
        Destroy(gameObject);
    }
}
