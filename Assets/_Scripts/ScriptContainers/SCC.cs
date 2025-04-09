using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCC", menuName = "ScriptableObjects/SCC", order = 2)]
public class SCC : ScriptableObject
{
    [SerializeField] public Dictionary<string, ScriptType> lanScript;
}