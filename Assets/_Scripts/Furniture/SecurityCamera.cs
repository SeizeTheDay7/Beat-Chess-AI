using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField] private GameObject joint;
    [SerializeField] private GameObject cameraHead;
    [SerializeField] private GameObject target;

    void Update()
    {
        // JOINT 회전 (z축 회전만 추출)
        Vector3 jointToTarget = target.transform.position - joint.transform.position;
        Vector3 jointForward = -joint.transform.up;
        Vector3 jointToTargetXZ = new Vector3(jointToTarget.x, 0, jointToTarget.z).normalized;
        Vector3 YBackwardXZ = new Vector3(jointForward.x, 0, jointForward.z).normalized;
        float angle = Vector3.SignedAngle(YBackwardXZ, jointToTargetXZ, Vector3.up);
        joint.transform.localRotation = Quaternion.Euler(0, 0, joint.transform.localEulerAngles.z + angle);

        // CAMERA 회전 (x축 회전만 추출)
        Vector3 cameraToTarget = target.transform.position - cameraHead.transform.position;
        float horizontalDistance = new Vector3(cameraToTarget.x, 0, cameraToTarget.z).magnitude;
        float verticalAngle = Mathf.Atan2(cameraToTarget.y, horizontalDistance) * Mathf.Rad2Deg;
        cameraHead.transform.localRotation = Quaternion.Euler(-verticalAngle, 0, 0);
    }
}
