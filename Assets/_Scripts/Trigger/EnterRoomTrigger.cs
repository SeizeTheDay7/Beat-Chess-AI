using UnityEngine;
using System.Collections;
using NUnit.Framework.Constraints;

public class EnterRoomTrigger : MonoBehaviour
{
    [SerializeField] private MetalDoor metalDoor;
    // [SerializeField] private GameObject light_outside;
    [SerializeField] private float waitTime = 2.0f;
    [SerializeField] private GameObject frontDeskTrigger;
    private bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        EnterRoom();
    }

    private void EnterRoom()
    {
        float doorCloseTime = metalDoor.MetalDoorClose();
        // Destroy(light_outside);
        frontDeskTrigger.SetActive(true);
        Destroy(gameObject);
    }
}
