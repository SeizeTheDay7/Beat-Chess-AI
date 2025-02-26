using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using System.Collections.Generic;

public class Shooter : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // 오디오 소스
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private List<AudioClip> footstep_sounds; // 발자국 사운드
    [SerializeField][Range(0f, 10f)] private float footstep_volume;
    [SerializeField] private AudioClip reloadSound; // 재장전 사운드
    [SerializeField][Range(0f, 10f)] private float reload_volume;
    [SerializeField] private AudioClip shotSound; // 발사 사운드
    [SerializeField][Range(0f, 10f)] private float shot_volume;
    [SerializeField] private float gapBetweenFootsteps = 0.5f; // 클립 간 간격
    [SerializeField] private float footstepIncrement = 1f; // 볼륨 배수
    [SerializeField] private float reloadDelay = 1.5f; // 재장전 딜레이
    [SerializeField] private float shotDelay = 1f; // 발사 딜레이

    // [SerializeField] private float fadeDuration = 1.5f; // 페이드 인 지속 시간
    // [SerializeField] private float min3DDistance = 1f; // 3D 최소 거리
    // [SerializeField] private float max3DDistance = 10f; // 3D 최대 거리

    private int currentClipIndex = 0;

    void Start()
    {
        // Setup3DAudio(); // 3D 설정 적용
    }

    public void PlayShooterFootstep()
    {
        if (currentClipIndex >= footstep_sounds.Count)
        {
            currentClipIndex = 0;
            DOVirtual.DelayedCall(reloadDelay, PlayShooterReload);
            return;
        }

        AudioClip clip = footstep_sounds[currentClipIndex];
        audioSource.clip = clip;
        // audioSource.volume = (currentClipIndex + 1) * footstepIncrement;
        audioMixer.SetFloat("ShooterVolume", footstep_volume + footstepIncrement * (currentClipIndex + 1));
        audioSource.Play();
        DOVirtual.DelayedCall(clip.length + gapBetweenFootsteps, PlayShooterFootstep);

        currentClipIndex++;
    }

    private void PlayShooterReload()
    {
        audioMixer.SetFloat("ShooterVolume", reload_volume);
        audioSource.clip = reloadSound;
        audioSource.Play();
        DOVirtual.DelayedCall(reloadSound.length + shotDelay, PlayShooterShot);
    }

    private void PlayShooterShot()
    {
        audioMixer.SetFloat("ShooterVolume", shot_volume);
        audioSource.clip = shotSound;
        audioSource.Play();
    }

    // void Setup3DAudio()
    // {
    //     // 3D 사운드 적용
    //     audioSource.spatialBlend = 1f; // 3D 효과 (0 = 2D, 1 = 3D)
    //     audioSource.minDistance = min3DDistance;
    //     audioSource.maxDistance = max3DDistance;
    // }
}
