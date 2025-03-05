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

    }

    void Update()
    {

    }

    /// <summary>
    /// 터미널이 다른 텍스트와 함께 기본 텍스트를 출력한다.
    /// </summary>
    public void BackToOriginalTextWith(string context)
    {
        SetTerminalText(context + "\n\n" + originalText);
    }

    /// <summary>
    /// 터미널이 기본 텍스트를 출력한다.
    /// </summary>
    public void BackToOriginalText()
    {
        SetTerminalText(originalText);
    }

    /// <summary>
    /// 터미널에 텍스트를 출력한다.
    /// </summary>
    public void SetTerminalText(string text)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        terminalText.text = text;
        typingCoroutine = StartCoroutine(TypeText());
    }

    /// <summary>
    /// 터미널에 잠시 동안 유지되는 텍스트를 출력한다.
    /// </summary>
    public void SetTemporalTerminalText(string text)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        terminalText.text = text;
        typingCoroutine = StartCoroutine(TypeText(isTemporal: true));
    }

    /// <summary>
    /// 텍스트 타이핑 효과 코루틴
    /// </summary>
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

    /// <summary>
    /// 터미널에 출력되고 있던 텍스트 아래에 텍스트를 추가한다.
    /// </summary>
    public void AddTerminalText(string text)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        int preLen = terminalText.text.Length;
        terminalText.text += "\n" + text;
        typingCoroutine = StartCoroutine(AddText(preLen));
    }

    /// <summary>
    /// 텍스트 추가 코루틴
    /// </summary>
    IEnumerator AddText(int preLen)
    {
        terminalText.ForceMeshUpdate();
        TMP_TextInfo textInfo = terminalText.textInfo;
        int totalCharacters = textInfo.characterCount;

        terminalText.maxVisibleCharacters = preLen;

        for (int i = preLen; i < totalCharacters; i++)
        {
            terminalText.maxVisibleCharacters = i + 1;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null;
    }
}
