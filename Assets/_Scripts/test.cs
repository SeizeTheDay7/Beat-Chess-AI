using UnityEngine;
using System.Collections;


public class test : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float duration;
    // [SerializeField] private Shakify shakify;
    Vector3 sp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sp = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(MoveSequenceCoroutine());
        }
    }

    private IEnumerator MoveSequenceCoroutine()
    {
        // 첫 번째 이동 (3초 동안 target 위치로 이동)
        Vector3 startPos = transform.position;
        Vector3 endPos = target.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos; // 정확한 위치 보정

        // 원래 위치로 즉시 이동
        transform.position = sp;
    }
}
