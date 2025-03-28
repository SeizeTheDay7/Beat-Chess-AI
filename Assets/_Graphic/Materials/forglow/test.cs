using UnityEngine;

public class test : MonoBehaviour
{
    MaterialPropertyBlock propBlock;
    [SerializeField] private Material material;


    void Start()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material;

    }
}
