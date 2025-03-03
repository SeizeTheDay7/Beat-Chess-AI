using System.Collections.Generic;
using UnityEngine;

public class PieceGrave : MonoBehaviour
{
    private float dx;
    private float dz;
    private int xIdx = 0;
    private int zIdx = 0;
    private List<GameObject> dead_pieces = new List<GameObject>();

    public void InitPieceGrave(float dx, float dz)
    {
        this.dx = dx;
        this.dz = dz;
    }

    public void ResetGrave()
    {
        xIdx = 0;
        zIdx = 0;

        foreach (GameObject piece in dead_pieces)
        {
            Destroy(piece);
        }
    }

    /// <summary>
    /// AI 기물 앞에 있는 무덤 위치 반환하면서 리스트에도 넣어놓는다
    /// </summary>
    public Vector3 GetAIGravePos(GameObject piece)
    {
        dead_pieces.Add(piece);

        Vector3 gravePos = transform.position + new Vector3(xIdx * dx, 0, zIdx * dz);
        xIdx++;
        if (xIdx > 4)
        {
            xIdx = 0;
            zIdx++;
        }
        return gravePos;
    }
}
