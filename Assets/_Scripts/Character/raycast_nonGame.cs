using UnityEngine;

public class raycast_nonGame : MonoBehaviour
{
    [SerializeField] float rayLength = 7f;
    int targetLayer;
    GameObject curLookObj;

    void Start()
    {
        targetLayer = LayerMask.GetMask("Furniture");
    }

    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);
        if (Physics.Raycast(ray, out hit, rayLength, targetLayer))
        {
            // 머테리얼 변경을 위해 현재 보고 있는 오브젝트 식별 및 함수 호출 (문은 아무 일도 안 일어남)
            if (curLookObj != hit.collider.gameObject)
            {
                curLookObj?.GetComponent<IRaycastNonGame>().OnEndLooking();
                curLookObj = hit.collider.gameObject;
                curLookObj.GetComponent<IRaycastNonGame>().OnStartLooking();
            }

            if (Input.GetMouseButtonDown(0))
            {
                curLookObj.GetComponent<IRaycastNonGame>().OnClicked(transform);
            }
        }
        else
        {
            curLookObj?.GetComponent<IRaycastNonGame>().OnEndLooking();
            curLookObj = null;
        }
    }
}

