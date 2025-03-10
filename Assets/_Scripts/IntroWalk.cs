using UnityEngine;
using System.Collections;
using Shakfy.Core;
using Unity.Cinemachine;

public class IntroWalk : MonoBehaviour
{
    [SerializeField] private GameObject walk1_target;
    [SerializeField] private GameObject walk2_target;
    [SerializeField] private GameObject walk3_target;
    [SerializeField] private GameObject walk4_target;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject chessboard;
    [SerializeField] private GameObject roboticArm;
    [SerializeField] private float walk1_duration = 5f;
    [SerializeField] private float walk2_duration = 3f;
    [SerializeField] private float walk3_duration = 2f;
    [SerializeField] private float walk4_duration = 2f;
    [SerializeField] private Shakify shakify;
    [SerializeField] private IntroManager introManager;
    [SerializeField] private CinemachineCamera vcam;

    void Start()
    {
        walk2_target.transform.LookAt(button.transform);
        walk3_target.transform.LookAt(roboticArm.transform);
        walk4_target.transform.LookAt(chessboard.transform.position + new Vector3(0, 0.5f, 0));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(MoveSequenceCoroutine());
        }
    }

    private IEnumerator MoveSequenceCoroutine()
    {
        yield return StartCoroutine(MoveToTarget(walk1_target.transform.position, walk1_duration));
        yield return StartCoroutine(MoveToTargetAndRotate(walk2_target.transform.position, walk2_target.transform.rotation, walk2_duration));
        yield return StartCoroutine(MoveToTargetAndRotate(walk3_target.transform.position, walk3_target.transform.rotation, walk3_duration));
        shakify.enabled = false;
        yield return StartCoroutine(MoveToTargetAndRotate(walk4_target.transform.position, walk4_target.transform.rotation, walk4_duration));
        vcam.Priority = 0;
        introManager.StartIntro();
        Destroy(this);
    }

    private IEnumerator MoveToTarget(Vector3 targetPosition, float duration)
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // 정확한 위치 보정
    }

    private IEnumerator MoveToTargetAndRotate(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / duration);
            transform.rotation = Quaternion.Slerp(startRot, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // 정확한 위치 보정
        transform.rotation = targetRotation; // 정확한 회전 보정
    }
}
