using UnityEngine;

public class RoboticArm : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform z_axis_part;
    [SerializeField] private Transform x_axis_part;
    [SerializeField] private Transform x_axis_2_part;
    [SerializeField] private Transform hand_part;
    [SerializeField] private Transform tongs_part;
    private Vector3 hand_offset;

    void Start()
    {
        hand_offset = hand_part.position - tongs_part.position;
    }

    public void SetTarget()
    {

    }

    void Update()
    {
        if (target == null) return;

        Vector3 direction = target.position - z_axis_part.position;
        direction.y = 0; // 불연속적 계산 방지

        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        z_axis_part.rotation = Quaternion.Euler(-90, 0, angle);

        hand_part.position = target.position + hand_offset;
    }
}