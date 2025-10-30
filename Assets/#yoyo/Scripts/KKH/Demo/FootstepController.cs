using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepController : MonoBehaviour
{
    //static private FootstepController inst;
    //static public FootstepController Inst => inst;

    public bool isPlayer = true; // Flag to check if this is the player

    public AudioClip[] footstepSounds; // Array to hold footstep sound clips
    public AudioClip[] runStepSounds; // Array to hold footstep sound clips

    [SerializeField] private float minTimeBetweenFootsteps = 0.4f;
    [SerializeField] private float maxTimeBetweenFootsteps = 0.6f;

    // 달리기일 때는 따로 설정해도 좋아요
    [SerializeField] private float minTimeBetweenRunSteps = 0.2f;
    [SerializeField] private float maxTimeBetweenRunSteps = 0.3f;

    private AudioSource audioSource; // Reference to the Audio Source component
    private bool isWalking = false; // Flag to track if the player is walking

    private float currentFootstepInterval;
    private float timeSinceLastFootstep; // Time since the last footstep sound

    [SerializeField] PlayerStamina playerStamina; // Reference to PlayerStamina script

    public bool isRunning = false;

    private void Awake()
    {
        //if (inst == null)
        //{
        //    inst = this;
        //}
        audioSource = GetComponent<AudioSource>(); // Get the Audio Source component
    }

    private void Start()
    {
        audioSource.volume = SoundManager.Instance.sfxVolume; // Set the volume from SoundManager
    }

    private void Update()
    {
        if (!isWalking)
        {
            audioSource.Stop();
            return;
        }

        if (isPlayer)
        {
            isRunning = playerStamina.isRunning;
        }

        //// 현재 재생 중인 클립이 걷기인데, isRunning이 true가 되면 교체
        //if (audioSource.isPlaying)
        //{
        //    if (isRunning && IsFootstepClip(footstepSounds, audioSource.clip))
        //    {
        //        audioSource.Stop(); // 걷기에서 달리기로 전환 시 소리 중단
        //    }
        //    else if (!isRunning && IsFootstepClip(runStepSounds, audioSource.clip))
        //    {
        //        audioSource.Stop(); // 달리기에서 걷기로 전환 시 소리 중단
        //    }
        //    else
        //    {
        //        return; // 여전히 같은 상태면 기다림
        //    }
        //}

        //// 다음 발소리를 낼 시간인지 확인
        //if (Time.time - timeSinceLastFootstep >= Random.Range(minTimeBetweenFootsteps, maxTimeBetweenFootsteps))
        //{
        //    Debug.Log($"{isRunning}");

        //    AudioClip clip = isRunning
        //        ? runStepSounds[Random.Range(0, runStepSounds.Length)]
        //        : footstepSounds[Random.Range(0, footstepSounds.Length)];

        //    audioSource.PlayOneShot(clip);
        //    timeSinceLastFootstep = Time.time;
        //}

        // 다음 재생 시간 체크
        if (Time.time - timeSinceLastFootstep >= currentFootstepInterval)
        {
            if (footstepSounds.Length == 0)
            {
                //Debug.LogWarning("Footstep sounds array is empty! Please assign footstep sounds.");
                return;
            }

            AudioClip nextClip = isRunning
                ? runStepSounds[Random.Range(0, runStepSounds.Length)]
                : footstepSounds[Random.Range(0, footstepSounds.Length)];

            // ✅ 현재 클립이 다르거나 재생 안되고 있으면 즉시 변경
            if (audioSource.clip != nextClip || !audioSource.isPlaying)
            {
                audioSource.clip = nextClip;
                audioSource.Play();
            }

            // 다음 간격 정하기
            currentFootstepInterval = isRunning
                ? Random.Range(minTimeBetweenRunSteps, maxTimeBetweenRunSteps)
                : Random.Range(minTimeBetweenFootsteps, maxTimeBetweenFootsteps);

            timeSinceLastFootstep = Time.time;
        }
    }

    private bool IsFootstepClip(AudioClip[] clips, AudioClip clip)
    {
        if (clip == null) return false;
        foreach (var c in clips)
        {
            if (c == clip) return true;
        }
        return false;
    }

    // Call this method when the player starts walking
    public void StartWalking()
    {
        isWalking = true;
    }

    // Call this method when the player stops walking
    public void StopWalking()
    {
        isWalking = false;
        audioSource.Stop();
    }
}
