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
    private float hand_tongs_distance;
    private float hand_tongs_angle;
    private float bonus_h;
    private float bonus_w;
    private float angle_offset;
    private float r1;
    private float r2;
    private Quaternion y_axis_part_rotation_goal;
    private Quaternion x_axis_part_rotation_goal;
    private Quaternion x_axis_part_2_rotation_goal;
    private Quaternion hand_part_rotation_goal;
    private float move_time = 1f;

    void Start()
    {
        y_axis_part_rotation_goal = y_axis_part.rotation; // 이상한 초기값으로 가버리지 않도록 현재 회전으로 초기화

        hand_y_offset = -tongs_part.localPosition.y * 110; // 원 교점 구할 때 쓰이는 손 길이
        r1 = (hand_part.position - x_axis_part_2.position).magnitude;
        r2 = (x_axis_part_2.position - x_axis_part.position).magnitude;

        Init_Arm();
        Set_Hand_Tongs_Offset();

        Fold_Arm();
    }

    private void Init_Arm()
    {
        y_axis_part.localRotation = Quaternion.Euler(-90, 0, 0);
        x_axis_part.localRotation = Quaternion.Euler(-90, 0, 0);
        x_axis_part_2.localRotation = Quaternion.Euler(90, 0, 0);
        hand_part.localRotation = Quaternion.Euler(90, 0, 0);
    }

    private void Set_Hand_Tongs_Offset()
    {
        Vector3 handPosXZ = hand_part.position;
        Vector3 tongsPosXZ = tongs_part.position;
        handPosXZ.y = 0;
        tongsPosXZ.y = 0;
        hand_tongs_distance = Vector3.Distance(handPosXZ, tongsPosXZ);

        Vector3 handToXAxis = hand_part.position - x_axis_part.position;
        Vector3 tongsToHand = tongs_part.position - hand_part.position;
        handToXAxis.y = 0;
        tongsToHand.y = 0;
        hand_tongs_angle = Vector3.Angle(handToXAxis, tongsToHand);

        bonus_h = hand_tongs_distance * Mathf.Cos(hand_tongs_angle * Mathf.Deg2Rad);
        bonus_w = hand_tongs_distance * Mathf.Sin(hand_tongs_angle * Mathf.Deg2Rad);
    }

    private void Fold_Arm()
    {
        x_axis_part_rotation_goal = Quaternion.Euler(-90, 0, 0);
        x_axis_part_2_rotation_goal = Quaternion.Euler(0, 0, 0);
        hand_part_rotation_goal = Quaternion.Euler(180, 0, 0);
    }

    void Update()
    {
        if (target_to_reach == null) return;
        Rotate_To_Goal_Continously();
    }

    private void Rotate_To_Goal_Continously()
    {
        y_axis_part.rotation = Quaternion.RotateTowards(y_axis_part.rotation, y_axis_part_rotation_goal, move_time); // local이 아니라 월드 좌표계 기준 회전 해야 함
        x_axis_part.localRotation = Quaternion.RotateTowards(x_axis_part.localRotation, x_axis_part_rotation_goal, move_time);
        x_axis_part_2.localRotation = Quaternion.RotateTowards(x_axis_part_2.localRotation, x_axis_part_2_rotation_goal, move_time);
        hand_part.localRotation = Quaternion.RotateTowards(hand_part.localRotation, hand_part_rotation_goal, move_time);
    }

    public void SetTarget(Transform target, float time)
    {
        target_to_reach = target;
        move_time = time;
        Set_Angle_Offset();
        Set_Y_Rotation();
        Set_X_Axis_Angles();
    }

    private void Set_Angle_Offset()
    {
        Vector3 a = target_to_reach.position - x_axis_part.position;
        a.y = 0;
        float h = a.magnitude;

        angle_offset = Mathf.Atan2(bonus_w, h + bonus_h) * Mathf.Rad2Deg;
    }

    private void Set_Y_Rotation()
    {
        Vector3 direction = target_to_reach.position - y_axis_part.position;
        direction.y = 0; // 불연속적 계산 방지

        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        y_axis_part_rotation_goal = Quaternion.Euler(-90, angle - angle_offset, 0);
        // y_axis_part.rotation = Quaternion.Euler(-90, angle - hand_angle_offset, 0); // local이 아니라 월드 좌표계 기준 회전 해야 함
    }

    private void Set_X_Axis_Angles()
    {
        (float angle2, float angle3) = Find_Angle_Set();
        if (float.IsNaN(angle2) || float.IsNaN(angle3)) return;
        x_axis_part_rotation_goal = Quaternion.Euler(angle2, 0, 0);
        x_axis_part_2_rotation_goal = Quaternion.Euler(angle3, 0, 0);
        float angle1 = 90 - angle2 - angle3;
        hand_part_rotation_goal = Quaternion.Euler(angle1, 0, 0);
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