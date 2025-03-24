using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class CakeEnding : MonoBehaviour
{
    [SerializeField] private ScriptContainer endingScript;
    [SerializeField] private TextMeshProUGUI endingText;
    AudioSource audioSource;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float textDuration = 3.5f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(endingSequence());
    }

    private IEnumerator endingSequence()
    {
        foreach (string line in endingScript.Texts)
        {
            endingText.text = line;
            StartCoroutine(TypeText());
            yield return new WaitForSeconds(textDuration);
        }
    }

    /// <summary>
    /// 텍스트 타이핑 효과 코루틴
    /// </summary>
    private IEnumerator TypeText()
    {
        endingText.ForceMeshUpdate();
        TMP_TextInfo textInfo = endingText.textInfo;
        int totalCharacters = textInfo.characterCount;

        endingText.maxVisibleCharacters = 0;

        for (int i = 0; i < totalCharacters; i++)
        {
            endingText.maxVisibleCharacters = i + 1;
            if (i % 3 == 0) audioSource.Play();
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
