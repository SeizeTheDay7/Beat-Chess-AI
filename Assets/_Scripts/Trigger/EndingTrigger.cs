using UnityEngine;
using Unity.Cinemachine;

public class EndingTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cake_cam;
    private
    void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
        cake_cam.Priority = 11;
    }
}
