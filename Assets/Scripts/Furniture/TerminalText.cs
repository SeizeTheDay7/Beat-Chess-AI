using TMPro;
using UnityEngine;
using System.Collections;

public class TerminalText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI terminalText;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float cursorBlinkSpeed = 0.5f;
    private string originalText = "It's your turn.";
    private Coroutine typingCoroutine;

    void Start()
    {
        typingCoroutine = StartCoroutine(TypeText());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BackToOriginalTextWith(string context)
    {
        SetTerminalText(context + "\n" + originalText);
    }

    public void BackToOriginalText()
    {
        SetTerminalText(originalText);
    }

    public void SetTerminalText(string text)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        terminalText.text = text;
        typingCoroutine = StartCoroutine(TypeText());
    }

    public void SetTemporalTerminalText(string text)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        terminalText.text = text;
        typingCoroutine = StartCoroutine(TypeText(true));
    }

    IEnumerator TypeText(bool isTemporal = false)
    {
        terminalText.ForceMeshUpdate();
        TMP_TextInfo textInfo = terminalText.textInfo;
        int totalCharacters = textInfo.characterCount;

        terminalText.maxVisibleCharacters = 0;

        for (int i = 0; i < totalCharacters; i++)
        {
            terminalText.maxVisibleCharacters = i + 1;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (isTemporal)
        {
            yield return new WaitForSeconds(1.5f);
            typingCoroutine = null;
            SetTerminalText(originalText);
        }
        else
        {
            typingCoroutine = null;
        }
    }
}
