using UnityEngine;

public class raycast : MonoBehaviour
{
    [SerializeField] float rayLength = 7f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayLength))
            {
                hit.collider.GetComponent<IRaycastReceiver>()?.OnRaycastHit();
            }
        }
    }
}

