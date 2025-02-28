using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    [SerializeField] private RoboticArm roboticArm;
    [SerializeField] private Transform target;
    [SerializeField] private float move_time = 1f;

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
    }

    private void DoSomething()
    {
        Debug.Log("Magic Happens!");
        roboticArm.SetTarget(target, move_time);
    }
}
