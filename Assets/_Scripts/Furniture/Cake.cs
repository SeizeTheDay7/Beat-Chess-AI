using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;

public class Cake : MonoBehaviour, IRaycastNonGame
{
    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock propBlock;
    private bool captured = false;
    [SerializeField] private WoodenDoor woodenDoor;
    [SerializeField] private float cakeMoveDuration = 0.5f;
    [SerializeField] private float forward_distance = 2f;
    [SerializeField] private float down_distance = 1.5f;
    [SerializeField] private GameObject endingCanvas;
    [SerializeField] private Image endingPanel;
    [SerializeField] private float endingWaitTime = 2f;
    [SerializeField] private float fadeDuration = 7f;
    private AudioSource audioSource;


    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        propBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(propBlock, 1);
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 클릭했으면 케이크를 눈 앞으로 가져오고 문을 닫고 엔딩 시퀀스
    /// </summary>
    public void OnClicked(Transform player)
    {
        // 두 번 실행되지 않도록, 그리고 집었으면 다른 함수들 실행되지 않도록
        if (captured) return;
        captured = true;

        woodenDoor.CloseDoor();
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

        while (elapsed < cakeMoveDuration)
        {
            elapsed += Time.deltaTime;
            Vector3 endPos = camera.InverseTransformPoint(camera.position + camera.forward * forward_distance + Vector3.down * down_distance);
            transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / cakeMoveDuration);
            yield return null;
        }

        transform.localPosition = camera.InverseTransformPoint(camera.position + camera.forward * forward_distance + Vector3.down * down_distance);
        yield return new WaitForSeconds(endingWaitTime);

        audioSource.Play();
        audioSource.DOFade(1f, fadeDuration).SetEase(Ease.Linear);

        // 화면이 점점 하얀색으로 변한다.
        endingCanvas.SetActive(true);
        endingPanel.DOFade(1f, fadeDuration).SetEase(Ease.Linear);
        yield return new WaitForSeconds(fadeDuration);

        // 렌더링은 전부 없애고, 케이크와 함께 다음 씬으로
        transform.SetParent(null);
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        meshRenderer.enabled = false;
        DontDestroyOnLoad(gameObject); // 케이크에 재생되는 엔딩 브금 끊기지 않게 하기 위해

        SceneManager.LoadScene("Ending");
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
