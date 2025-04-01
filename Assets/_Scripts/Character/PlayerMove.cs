using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] CinemachineCamera player_vcam;
    [SerializeField] float walk_gap = 0.5f;
    [SerializeField] float amplitudeGain_walk = 2f;
    [SerializeField] float frequencyGain_walk = 1.5f;
    [SerializeField] AudioClip[] footstepSounds;
    AudioSource footstep;
    public float moveSpeed = 5f;
    private CharacterController characterController;
    private CinemachineImpulseSource impulse;
    private CinemachineBasicMultiChannelPerlin noise;
    private bool wait_nextFootstep = false;
    private float targetAmplitudeGain;
    private float targetFrequencyGain;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        impulse = GetComponent<CinemachineImpulseSource>();
        noise = player_vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        footstep = gameObject.AddComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked; // 마우스를 화면 중앙에 고정
        Cursor.visible = false; // 마우스 커서 숨김
    }

    void Update()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += player_vcam.transform.forward;
        if (Input.GetKey(KeyCode.S)) direction += -player_vcam.transform.forward;
        if (Input.GetKey(KeyCode.A)) direction += -player_vcam.transform.right;
        if (Input.GetKey(KeyCode.D)) direction += player_vcam.transform.right;

        direction.y = 0f; // 점프를 구현하지 않는 이상 y 축 이동 방지 (중력 유지)
        direction.Normalize();

        if (direction != Vector3.zero)
        {
            targetAmplitudeGain = amplitudeGain_walk;
            targetFrequencyGain = frequencyGain_walk;
            if (!wait_nextFootstep)
            {
                wait_nextFootstep = true;
                StartCoroutine(GenerateImpulseWithDelay());
            }
        }
        else
        {
            targetAmplitudeGain = 0.7f;
            targetFrequencyGain = 0.7f;
        }

        noise.AmplitudeGain = Mathf.Lerp(noise.AmplitudeGain, targetAmplitudeGain, Time.deltaTime * 5f);
        noise.FrequencyGain = Mathf.Lerp(noise.FrequencyGain, targetFrequencyGain, Time.deltaTime * 5f);

        characterController.Move(direction * moveSpeed * Time.deltaTime);
    }

    private IEnumerator GenerateImpulseWithDelay()
    {
        impulse.GenerateImpulse();
        PlayFootstepSound();
        yield return new WaitForSeconds(walk_gap);
        wait_nextFootstep = false;
    }

    private void PlayFootstepSound()
    {
        if (footstepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            footstep.clip = footstepSounds[randomIndex];
            footstep.Play();
        }
    }

}