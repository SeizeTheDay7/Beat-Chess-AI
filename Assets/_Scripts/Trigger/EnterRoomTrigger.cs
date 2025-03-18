using UnityEngine;
using System.Collections;
using NUnit.Framework.Constraints;

public class EnterRoomTrigger : MonoBehaviour
{
    [SerializeField] private MetalDoor metalDoor;
    [SerializeField] private GameObject light_outside;
    [SerializeField] private float waitTime = 2.0f;
    [SerializeField] private Light spotlight;
    [SerializeField] private ComputerGlitter computerGlitter;
    [SerializeField] private GameObject computerScreen;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject frontDeskTrigger;
    private bool isTriggered = false;

    void Start()
    {
        computerScreen.SetActive(false); // 빌드에는 없어도 됨
    }

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        StartCoroutine(EnterRoomCoroutine());
    }

    private IEnumerator EnterRoomCoroutine()
    {
        float doorCloseTime = metalDoor.MetalDoorClose();
        // Destroy(light_outside);
        yield return new WaitForSeconds(doorCloseTime);
        yield return new WaitForSeconds(waitTime);

        spotlight.intensity = 30;
        audioSource.Play();
        computerGlitter.enabled = true;
        computerScreen.SetActive(true);

        frontDeskTrigger.SetActive(true);
        Destroy(gameObject);
    }
}
