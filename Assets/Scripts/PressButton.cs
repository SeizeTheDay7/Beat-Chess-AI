using UnityEngine;
using DG.Tweening;

public class PressButton : MonoBehaviour
{
    [SerializeField] float bounce_amount;
    [SerializeField] float press_duration;
    private Vector3 originalPos; // 원래 위치를 한 번만 저장
    private bool isAnimating = false; // 애니메이션 중복 방지

    void Start()
    {
        originalPos = transform.position; // 원래 위치 저장
    }

    public void pressButton()
    {
        Debug.Log("Button Pressed");

        if (isAnimating) return; // 이미 애니메이션 중이면 실행 안 함
        isAnimating = true; // 애니메이션 시작

        // 아래로 이동 후 다시 원위치로 복귀 (Sequence 사용)
        Sequence sequence = DOTween.Sequence()
            .Append(transform.DOMoveY(originalPos.y - bounce_amount, press_duration).SetEase(Ease.InOutQuad)) // 아래로 이동
            .Append(transform.DOMoveY(originalPos.y, press_duration).SetEase(Ease.InOutQuad)) // 원위치 복귀
            .OnComplete(() => isAnimating = false); // 애니메이션 종료 후 isAnimating을 false로 변경

        sequence.Play();
    }
}
