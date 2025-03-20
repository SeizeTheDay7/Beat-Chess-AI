using UnityEngine;

public class TurnOnLightTrigger : MonoBehaviour
{
    [SerializeField] private Light tableLight;
    [SerializeField] private ComputerGlitter computerGlitter;
    [SerializeField] private GameObject computerScreen;
    private bool isTriggered = false;

    void Start()
    {
        computerScreen.SetActive(false); // 빌드에는 없어도 됨. 그냥 꺼놓을거임.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;

        // computerGlitter.enabled = true;
        computerScreen.SetActive(true);

        tableLight.intensity = 30;
        GetComponent<AudioSource>().Play();

        Destroy(gameObject, 1.0f);
    }
}
