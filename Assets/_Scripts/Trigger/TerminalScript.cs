using UnityEngine;

[CreateAssetMenu(fileName = "ScriptContainer", menuName = "ScriptableObjects/ScriptContainer", order = 1)]
public class ScriptContainer : ScriptableObject
{
    [SerializeField] public ScriptType scriptType;
    [SerializeField] public string[] Texts;

    [Tooltip("bool이 true면 해당하는 인덱스의 텍스트가 기존 텍스트와 함께 출력")]
    [SerializeField] public bool[] ContinueLine;
}