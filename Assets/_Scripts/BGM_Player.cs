using UnityEngine;

public class BGM_Player : MonoBehaviour
{
    [SerializeField] AudioSource main_bgm;
    [SerializeField] AudioSource noise_bgm;
    [SerializeField] AudioClip Insert_CD_sfx;
    [SerializeField] AudioClip white_noise;

    void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (main_bgm.isPlaying) return;
        main_bgm.Play();
        noise_bgm.Stop();
    }

    public void StopBGM()
    {
        if (!main_bgm.isPlaying) return;
        main_bgm.Stop();
        noise_bgm.PlayOneShot(Insert_CD_sfx);
        noise_bgm.clip = white_noise;
        noise_bgm.loop = true;
        noise_bgm.Play();
    }
}
