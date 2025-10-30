using System.Collections;
using UnityEngine;

public class HorribleShriekJumpSquare : MonoBehaviour
{
    public AudioSource audioSource;

    private void Awake()
    {
        if (audioSource)
        {
            //audioSource.playOnAwake = false; // Prevent audio from playing on awake
            //audioSource.loop = false; // Ensure the audio does not loop
        }
    }

    private void Start()
    {
        if (SoundManager.Instance)
        {
            audioSource.volume = SoundManager.Instance.sfxSource.volume;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(SoundPlay());
        }
    }

    IEnumerator SoundPlay()
    {
        GetComponent<BoxCollider>().enabled = false; // Disable the collider to prevent retriggering
        audioSource.Play();
        yield return new WaitForSeconds(3.0f);
        gameObject.SetActive(false); // Deactivate the game object after playing the sound
    }
}
