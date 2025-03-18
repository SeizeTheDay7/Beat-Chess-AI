using UnityEngine;
using System.Collections;
using TMPro;

public class CreditRoll : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI credit_text;
    [SerializeField] private float credit_roll_time = 5.0f;
    private float canvasHeight;


    void Start()
    {
        canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        credit_text.rectTransform.localPosition = new Vector3(credit_text.rectTransform.localPosition.x, -canvasHeight, credit_text.rectTransform.localPosition.z);
        StartCoroutine(CreditRollCoroutine());
    }

    private IEnumerator CreditRollCoroutine()
    {
        // 크레딧 롤 올라감
        Vector3 start_position = credit_text.rectTransform.localPosition;
        Vector3 end_position = new Vector3(start_position.x, credit_text.rectTransform.rect.height - canvasHeight / 2, start_position.z);

        float elapsed_time = 0f;

        while (elapsed_time < credit_roll_time)
        {
            credit_text.rectTransform.localPosition = Vector3.Lerp(start_position, end_position, elapsed_time / credit_roll_time);
            elapsed_time += Time.deltaTime;
            yield return null;
        }

        credit_text.rectTransform.localPosition = end_position;
    }
}
