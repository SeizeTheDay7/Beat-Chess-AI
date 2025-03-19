using UnityEngine;

[CreateAssetMenu(fileName = "TerminalScript", menuName = "ScriptableObjects/TerminalScript", order = 1)]
public class TerminalScript : ScriptableObject
{
    [SerializeField] public IntroOrEnding introOrEnding;
    [SerializeField] public string[] introTexts;

    [Tooltip("bool이 true면 해당하는 인덱스의 텍스트가 기존 텍스트와 함께 출력")]
    [SerializeField] public bool[] introTextType;
}