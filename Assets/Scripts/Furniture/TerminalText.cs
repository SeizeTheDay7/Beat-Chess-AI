using TMPro;
using UnityEngine;
using System.Collections;
using System.Text;

public class TerminalText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI terminalText;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float cursorBlinkSpeed = 0.5f;
    private string originalText = "It's your turn.";
    private Coroutine typingCoroutine;

    private string fullText;
    private StringBuilder displayedText = new StringBuilder();

    void Start()
    {
        typingCoroutine = StartCoroutine(TypeText());
    }

    // Update is called once per frame
    void Update()
    {

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

    IEnumerator TypeText()
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

        typingCoroutine = null;
    }
}
