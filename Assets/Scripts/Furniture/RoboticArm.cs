using UnityEngine;
using DG.Tweening;

public class RoboticArm : MonoBehaviour
{
    private Vector3 target_position;
    [SerializeField] private Transform y_axis_part;
    [SerializeField] private Transform x_axis_part;
    [SerializeField] private Transform x_axis_part_2;
    [SerializeField] private Transform hand_part;
    [SerializeField] private Transform tongs_part;
    private Transform[] tongs_arr = new Transform[3];
    [SerializeField][Range(0, 1f)] private float length_magic_number_subt;
    private float hand_y_offset;
    private float hand_tongs_distance;
    private float hand_tongs_angle;
    private float bonus_h;
    private float bonus_w;
    private float angle_offset;
    private float r1;
    private float r2;
    [SerializeField] private float move_time = 0.1f;

    void Start()
    {
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

        for (int i = 0; i < 3; i++) tongs_arr[i] = tongs_part.GetChild(i);
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

    public void Fold_Arm()
    {
        x_axis_part.DOLocalRotate(new Vector3(-90, 0, 0), 1f);
        x_axis_part_2.DOLocalRotate(Vector3.zero, 1f);
        hand_part.DOLocalRotate(new Vector3(180, 0, 0), 1f);
    }

    public void Open_Tongs()
    {
        foreach (Transform tongs in tongs_arr)
            tongs.DOLocalRotate(tongs.transform.localRotation.eulerAngles - new Vector3(8, 0, 0), 1f);
    }

    public void Grip_Tongs()
    {
        foreach (Transform tongs in tongs_arr)
            tongs.DOLocalRotate(tongs.transform.localRotation.eulerAngles + new Vector3(8, 0, 0), 1f);
    }

    public void SetTarget(Vector3 point, float time)
    {
        move_time = time;
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => Set_Course_Dot(point + new Vector3(0, 0.5f, 0), move_time));
        seq.AppendInterval(move_time + 0.2f);
        seq.AppendCallback(() =>
        {
            Set_Course_Dot(point, move_time / 2);
        });
    }

    private void Set_Course_Dot(Vector3 point, float time)
    {
        target_position = point;
        move_time = time;
        Set_Angle_Offset();
        Set_Y_Rotation();
        Set_X_Axis_Angles();
    }

    private void Set_Angle_Offset()
    {
        Vector3 a = target_position - x_axis_part.position;
        a.y = 0;
        float h = a.magnitude;

        angle_offset = Mathf.Atan2(bonus_w, h + bonus_h) * Mathf.Rad2Deg;
    }

    private void Set_Y_Rotation()
    {
        Vector3 direction = target_position - y_axis_part.position;
        direction.y = 0; // 불연속적 계산 방지

        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        y_axis_part.DORotate(new Vector3(-90, angle - angle_offset, 0), move_time);
    }

    private void Set_X_Axis_Angles()
    {
        (float angle2, float angle3) = Find_Angle_Set();
        if (float.IsNaN(angle2) || float.IsNaN(angle3)) return;
        x_axis_part.DOLocalRotate(new Vector3(angle2, 0, 0), move_time);
        x_axis_part_2.DOLocalRotate(new Vector3(angle3, 0, 0), move_time);
        float angle1 = 90 - angle2 - angle3;
        hand_part.DOLocalRotate(new Vector3(angle1, 0, 0), move_time);
    }

    private (float, float) Find_Angle_Set()
    {
        Vector2 a = new Vector2(0, hand_y_offset);

        float y_c = x_axis_part.position.y - target_position.y;
        Vector3 base_to_target = target_position - x_axis_part.position;
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