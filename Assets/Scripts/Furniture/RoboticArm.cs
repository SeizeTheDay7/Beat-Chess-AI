using UnityEngine;

public class RoboticArm : MonoBehaviour
{
    [SerializeField] private Transform target_to_reach;
    [SerializeField] private Transform y_axis_part;
    [SerializeField] private Transform x_axis_part;
    [SerializeField] private Transform x_axis_part_2;
    [SerializeField] private Transform hand_part;
    [SerializeField] private Transform tongs_part;
    [SerializeField][Range(0, 1f)] private float length_magic_number_subt;
    private float hand_y_offset;
    private float hand_angle_offset;
    private float r1;
    private float r2;

    void Start()
    {
        hand_y_offset = -tongs_part.localPosition.y * 110;

        r1 = (hand_part.position - x_axis_part_2.position).magnitude;
        r2 = (x_axis_part_2.position - x_axis_part.position).magnitude;
    }

    private void Set_Angle_Offset()
    {
        Vector3 a = tongs_part.position - x_axis_part.position;
        Vector3 b = hand_part.position - x_axis_part.position;
        a.y = 0;
        b.y = 0;
        hand_angle_offset = Vector3.Angle(a, b);
    }

    void Update()
    {
        if (target_to_reach == null) return;
        Set_Angle_Offset();
        Sync_Y_Rotation();
        Adjust_X_Axis_Angles();
    }


    private void Sync_Y_Rotation()
    {
        Vector3 direction = target_to_reach.position - y_axis_part.position;
        direction.y = 0; // 불연속적 계산 방지

        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        y_axis_part.rotation = Quaternion.Euler(-90, angle - hand_angle_offset, 0);
    }

    private void Adjust_X_Axis_Angles()
    {
        (float angle2, float angle3) = Find_Angle_Set();
        if (float.IsNaN(angle2) || float.IsNaN(angle3)) return;
        x_axis_part.localRotation = Quaternion.Euler(angle2, 0, 0);
        x_axis_part_2.localRotation = Quaternion.Euler(angle3, 0, 0);
        float angle1 = 90 - angle2 - angle3;
        hand_part.localRotation = Quaternion.Euler(angle1, 0, 0);
    }

    private (float, float) Find_Angle_Set()
    {
        Vector2 a = new Vector2(0, hand_y_offset);

        float y_c = x_axis_part.position.y - target_to_reach.position.y;
        Vector3 base_to_target = target_to_reach.position - x_axis_part.position;
        base_to_target.y = 0;
        float x_c = base_to_target.magnitude;
        Vector2 c = new Vector2(x_c - length_magic_number_subt, y_c);

        Vector2 b = FindCircleIntersections(a, r1, c, r2);

        float angle2 = -1 * (180 - GetAngleBetweenPoints(c, b));
        float angle3 = GetAngleBetweenPoints(b, a) + 180 - angle2;

        return (angle2, angle3);
    }

    /// <summary>
    /// 두 원의 교점을 구하여 높이가 높은 쪽을 Vector2로 반환합니다.
    /// </summary>
    /// <param name="center1">첫 번째 원의 중심</param>
    /// <param name="radius1">첫 번째 원의 반지름</param>
    /// <param name="center2">두 번째 원의 중심</param>
    /// <param name="radius2">두 번째 원의 반지름</param>
    private Vector2 FindCircleIntersections(
        Vector2 center1, float radius1,
        Vector2 center2, float radius2)
    {
        float d = Vector2.Distance(center1, center2);
        float a = (radius1 * radius1 - radius2 * radius2 + d * d) / (2 * d);
        float h = Mathf.Sqrt(radius1 * radius1 - a * a);
        Vector2 direction = (center2 - center1).normalized;
        Vector2 m = center1 + a * direction;
        Vector2 perp = new Vector2(-direction.y, direction.x);

        Vector2 i1 = m + perp * h;

        return i1;
    }

    float GetAngleBetweenPoints(Vector2 p1, Vector2 p2)
    {
        Vector2 dir = p2 - p1;  // 방향 벡터 구하기
        float angleRad = Mathf.Atan2(dir.y, dir.x); // atan2(y, x) 사용
        float angleDeg = angleRad * Mathf.Rad2Deg;  // 라디안을 도(degree)로 변환
        return angleDeg;
    }
}