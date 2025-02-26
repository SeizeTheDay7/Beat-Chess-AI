using UnityEngine;
using DG.Tweening;

public class ComputerGlitter : MonoBehaviour
{
    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 1.5f;
    [SerializeField] private float minDuration = 0.5f;
    [SerializeField] private float maxDuration = 1.5f;
    private int materialIndex = 4;

    private MaterialPropertyBlock propBlock;
    private Color baseColor;

    void Start()
    {
        propBlock = new MaterialPropertyBlock();
        objectRenderer.GetPropertyBlock(propBlock, materialIndex);
        baseColor = objectRenderer.sharedMaterials[materialIndex].GetColor("_EmissionColor");

        StartGlitterEffect();
    }

    void StartGlitterEffect()
    {
        float intensity = (Random.value > 0.5f) ? minIntensity : maxIntensity;
        float duration = Random.Range(minDuration, maxDuration);
        propBlock.SetColor("_EmissionColor", baseColor * intensity);
        objectRenderer.SetPropertyBlock(propBlock, materialIndex);

        DOVirtual.DelayedCall(duration, StartGlitterEffect);
    }
}
