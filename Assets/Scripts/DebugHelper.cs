using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    [SerializeField] private RoboticArm roboticArm;
    [SerializeField] private Transform target;
    [SerializeField] private float time = 1f;
    [SerializeField] private GameObject A;
    [SerializeField] private GameObject B;
    [SerializeField] private GameObject test_piece;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DoSomething();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            FoldArm();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoookAB();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            roboticArm.Open_Tongs(time);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            roboticArm.Grip_Tongs(time);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            roboticArm.MovePieceToPos(test_piece, target.position, time);
        }

    }

    private void DoSomething()
    {
        Debug.Log("Magic Happens!");

    }

    private void FoldArm()
    {
        roboticArm.Fold_Arm();
    }

    private void LoookAB()
    {
        print("A Look at B");
        Vector3 BPos = B.transform.GetComponent<MeshRenderer>().bounds.center;
        A.transform.LookAt(BPos);
    }
}
