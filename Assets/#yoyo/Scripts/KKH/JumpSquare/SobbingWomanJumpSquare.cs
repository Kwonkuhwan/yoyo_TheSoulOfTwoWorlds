using UnityEngine;

public class SobbingWomanJumpSquare : MonoBehaviour
{
    public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (SoundManager.Instance)
        {
            audioSource.volume = SoundManager.Instance.sfxSource.volume;
        }
    }
}
