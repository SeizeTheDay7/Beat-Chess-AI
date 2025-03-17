using UnityEngine;
using System.Collections;
using NUnit.Framework.Constraints;

public class EnterRoomTrigger : MonoBehaviour
{
    [SerializeField] private MetalDoor metalDoor;
    [SerializeField] private float waitTime = 2.0f;
    [SerializeField] private Renderer lampRenderer;
    [SerializeField] private Light spotlight;
    [SerializeField] private ComputerGlitter computerGlitter;
    [SerializeField] private GameObject computerScreen;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject frontDeskTrigger;
    private MaterialPropertyBlock propBlock;
    private bool isTriggered = false;

    void Start()
    {
        computerScreen.SetActive(false); // 빌드에는 없어도 됨

        propBlock = new MaterialPropertyBlock();
        lampRenderer.GetPropertyBlock(propBlock, 0);
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
        yield return new WaitForSeconds(doorCloseTime);
        yield return new WaitForSeconds(waitTime);
        spotlight.intensity = 30;
        propBlock.SetColor("_EmissionColor", Color.white * 3.5f);
        audioSource.Play();
        computerGlitter.enabled = true;
        computerScreen.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        frontDeskTrigger.SetActive(true);
        Destroy(gameObject);
    }
}
