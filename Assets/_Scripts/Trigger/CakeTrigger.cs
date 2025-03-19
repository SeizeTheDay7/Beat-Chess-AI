using UnityEngine;
using System.Collections;

public class CakeTrigger : MonoBehaviour
{
    [SerializeField] private GameObject confetti;
    [SerializeField] private AudioSource ending_sound;
    [SerializeField] private float balloon_pop_start_dealy = 1.0f;
    [SerializeField] private float balloon_pop_dealy = 0.1f;
    [SerializeField] private GameObject[] balloons;
    [SerializeField] private GameObject EndMetalTrigger;
    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        StartCoroutine(endingCoroutine());
    }

    private IEnumerator endingCoroutine()
    {
        confetti.SetActive(true);
        ending_sound.Play(); // 폭죽 다음 타라
        yield return new WaitForSeconds(ending_sound.clip.length + balloon_pop_start_dealy);
        foreach (GameObject balloon in balloons)
        {
            balloon.GetComponent<ParticleSystem>().Play();
            balloon.GetComponent<MeshRenderer>().enabled = false;
            balloon.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(balloon_pop_dealy);
        }
        EndMetalTrigger.SetActive(true);
        Destroy(gameObject);
    }
}
