using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class EndingTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cake_cam;
    [SerializeField] private AudioSource ending_sound;
    [SerializeField] private GameObject confetti;
    [SerializeField] private GameObject ending_canvas;
    [SerializeField] private Image background;

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
        cake_cam.Priority = 11;
        StartCoroutine(EndingCoroutine());
    }

    private IEnumerator EndingCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        confetti.SetActive(true);
        ending_sound.Play(); // 폭죽 다음 타라
        yield return new WaitForSeconds(ending_sound.clip.length + 2f);
        StartCoroutine(CreditCoroutine());
    }

    private IEnumerator CreditCoroutine()
    {
        // 엔딩 캔버스 페이드 인        
        ending_canvas.SetActive(true);

        float fade_duration = 3.0f;
        float fade_elapsed_time = 0f;

        while (fade_elapsed_time < fade_duration)
        {
            background.color = new Color(background.color.r, background.color.g, background.color.b, Mathf.Lerp(0f, 1f, fade_elapsed_time / fade_duration));
            fade_elapsed_time += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene("credit");
    }
}
