using UnityEngine;

public class TurnOnLightTrigger : MonoBehaviour
{
    [SerializeField] private DeskLight deskLight;
    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        deskLight.TurnOnLight();
        Destroy(gameObject);
    }
}
