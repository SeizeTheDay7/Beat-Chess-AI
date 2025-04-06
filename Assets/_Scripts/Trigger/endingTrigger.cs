using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class endingTrigger : MonoBehaviour
{
    [SerializeField] private GameObject endingCanvas;
    [SerializeField] private Image endingPanel;
    [SerializeField] private float fadeDuration = 1f;
    private bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        endingCanvas.SetActive(true);
        StartCoroutine(FadeInPanel());
    }

    private IEnumerator FadeInPanel()
    {
        GetComponent<AudioSource>().Play();
        endingPanel.DOFade(1f, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene("Ending");
        Destroy(this);
    }
}
