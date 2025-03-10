using System.Collections.Generic;
using UnityEngine;

public class PieceGrave : MonoBehaviour
{
    private float dx;
    private float dz;
    private int xIdx_ai = 0;
    private int zIdx_ai = 0;
    private int xIdx_pl = 0;
    private int zIdx_pl = 1;
    private List<GameObject> dead_pieces = new List<GameObject>();

    public void InitPieceGrave(float dx, float dz)
    {
        this.dx = dx;
        this.dz = dz;
    }

    public void ResetGrave()
    {
        xIdx_ai = 0;
        zIdx_ai = 0;
        xIdx_pl = 0;
        zIdx_pl = 1;

        foreach (GameObject piece in dead_pieces)
        {
            Destroy(piece);
        }
    }

    /// <summary>
    /// AI 기물 앞에 있는 무덤 위치 반환하면서 나중에 리셋하기 위해 리스트에도 넣어놓는다
    /// </summary>
    public Vector3 GetAIGravePos(GameObject piece)
    {
        dead_pieces.Add(piece);

        Vector3 gravePos = transform.position + new Vector3(xIdx_ai * dx, 0, zIdx_ai * dz);
        xIdx_ai++;
        if (xIdx_ai > 4)
        {
            xIdx_ai = 0;
            zIdx_ai++;
        }
        return gravePos;
    }

    public Vector3 GetPlayerGravePos(GameObject piece)
    {
        dead_pieces.Add(piece);

        Vector3 gravePos = transform.position + new Vector3(xIdx_pl * dx, 0, -zIdx_pl * dz);
        xIdx_pl++;
        if (xIdx_pl > 4)
        {
            xIdx_pl = 0;
            zIdx_pl++;
        }
        return gravePos;
    }
}
