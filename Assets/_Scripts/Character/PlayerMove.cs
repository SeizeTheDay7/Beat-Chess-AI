using UnityEngine;
using Unity.Cinemachine;
using Shakfy.Core;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] CinemachineCamera player_vcam;
    [SerializeField] Shakify shakify;
    public float moveSpeed = 5f;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // 마우스를 화면 중앙에 고정
        Cursor.visible = false; // 마우스 커서 숨김
    }

    void Update()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction += player_vcam.transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += -player_vcam.transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += -player_vcam.transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += player_vcam.transform.right;
        }

        direction.y = 0f; // 점프를 구현하지 않는 이상 y 축 이동 방지 (중력 유지)
        direction.Normalize();

        if (direction != Vector3.zero) shakify.Shake();

        characterController.Move(direction * moveSpeed * Time.deltaTime);
    }
}
