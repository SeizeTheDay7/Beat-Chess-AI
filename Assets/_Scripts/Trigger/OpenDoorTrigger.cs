using UnityEngine;
using System.Collections;

public class OpenDoorTrigger : MonoBehaviour
{
    [SerializeField] private BGM_Player bgmPlayer;
    [SerializeField] private ElevatorDoor elevatorDoor;
    [SerializeField] private float waitTime_stopBGM = 3f;
    [SerializeField] private float waitTime_shotSeq = 2f;
    [SerializeField] private float waitTime_changeSign = 0.5f;
    [SerializeField] private float waitTime_dooropen = 1f;
    [SerializeField] private float waitTime_BGMrestart = 1.5f;

    private bool isTriggered = false;
    private AudioSource shotSequence;

    private void Start()
    {
        shotSequence = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        isTriggered = true;
        StartCoroutine(DoorOpenCoroutine());
    }

    private IEnumerator DoorOpenCoroutine()
    {
        // 브금이 꺼진다. 걸어가는 소리가 나고 탕 소리 난다. 잠시 후 브금 켜지며 문이 열린다.
        yield return new WaitForSeconds(waitTime_stopBGM);
        bgmPlayer.StopBGM();
        yield return new WaitForSeconds(waitTime_shotSeq);

        shotSequence.Play();
        yield return new WaitForSeconds(shotSequence.clip.length);

        yield return new WaitForSeconds(waitTime_changeSign);
        elevatorDoor.SetVacant();

        yield return new WaitForSeconds(waitTime_dooropen);
        elevatorDoor.ElevatorDoorOpen();

        yield return new WaitForSeconds(waitTime_BGMrestart);
        bgmPlayer.PlayBGM();
        Destroy(gameObject);
    }
}
