using UnityEngine;
using System.Collections;

public class Cake : MonoBehaviour, IRaycastNonGame
{
    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock propBlock;
    private bool captured = false;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float forward_distance = 2f;
    [SerializeField] private float down_distance = 1.5f;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        propBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(propBlock, 1);
    }

    /// <summary>
    /// 클릭했으면 케이크를 눈 앞으로 가져옴
    /// </summary>
    public void OnClicked(Transform player)
    {
        // 두 번 실행되지 않도록, 그리고 집었으면 다른 함수들 실행되지 않도록
        if (captured) return;
        captured = true;
        propBlock.SetFloat("_Scale", 0);
        meshRenderer.SetPropertyBlock(propBlock, 1);

        Transform camera = player.GetChild(0);
        transform.SetParent(camera);
        StartCoroutine(MoveToPlayer(camera));
    }

    private IEnumerator MoveToPlayer(Transform camera)
    {
        Vector3 startPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 endPos = camera.InverseTransformPoint(camera.position + camera.forward * forward_distance + Vector3.down * down_distance);
            transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);
            yield return null;
        }

        transform.localPosition = camera.InverseTransformPoint(camera.position + camera.forward * forward_distance + Vector3.down * down_distance);
    }

    public void OnStartLooking()
    {
        if (captured) return;
        propBlock.SetFloat("_Scale", 1.1f);
        meshRenderer.SetPropertyBlock(propBlock, 1);
    }

    public void OnEndLooking()
    {
        if (captured) return;
        propBlock.SetFloat("_Scale", 0);
        meshRenderer.SetPropertyBlock(propBlock, 1);
    }
}
