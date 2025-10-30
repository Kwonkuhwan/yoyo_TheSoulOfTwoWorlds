using UnityEngine;

public class RandAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private float coolTime = 0.0f;                                  
    [SerializeField] private float maxCoolTime = 2.0f;

    [SerializeField] private AudioClip[] clips;

    private void Awake()
    {
        maxCoolTime = Random.Range(60f, 120f);
        audioSource = GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (coolTime < maxCoolTime)
        {
            coolTime += Time.deltaTime;
        }
        else
        {
            RandAudioStart();
            coolTime = 0.0f;
        }
    }

    private void RandAudioStart()
    {
        if (clips.Length <= 0) return;
        audioSource.clip = clips[Random.Range(0, clips.Length - 1)];
        audioSource.Play();
        maxCoolTime = Random.Range(60f, 120f);

    }
}
