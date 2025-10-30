using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterJumpSquareBeta : MonoBehaviour
{
    [SerializeField] public Transform target;

    [SerializeField] private Animator animator;

    [SerializeField] public bool isPlayerIn = false;
    [SerializeField] public bool isTimeOver = false; // Flag to check if time is over
    [SerializeField] private float timeOverMax = 5.0f;
    [SerializeField] private float timeOver = 0.0f; // Timer for time over

    [SerializeField] protected NavMeshAgent agent;

    [SerializeField] protected AudioClip chuckleClip; // Audio clip for sound effects
    [SerializeField] protected AudioClip crazyCryingClip; // Audio clip for sound effects
    [SerializeField] protected AudioClip screamClip; // Audio clip for sound effects
    [SerializeField] protected AudioSource audioSource; // Audio source for playing sound effects

    [SerializeField] protected FootstepController footstepController;

    [SerializeField] protected Transform trReset;

    private void Awake()
    {
        if (!agent)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = true;
        }

        if (!animator)
        {
            animator = GetComponent<Animator>(); // Ensure the Animator component is attached
        }
    }

    private void Start()
    {
        if (SoundManager.Instance)
        {
            audioSource.volume = SoundManager.Instance.sfxSource.volume;
        }

        PlayAudioChuckle();
        footstepController.StartWalking();
    }

    private void Update()
    {
        if (isPlayerIn && isTimeOver)
        {
            agent.SetDestination(target.position); // Set the agent's destination to the target's position            
        }

        if (isPlayerIn && !isTimeOver)
        {
            if (timeOver < timeOverMax)
            {
                timeOver += Time.deltaTime; // Increment the timer
            }
            else
            {
                isTimeOver = true; // Set the flag to true when time is over
                PlayerIn(true);
            }
        }
    }

    virtual public void PlayAudioChuckle()
    {
        audioSource.clip = chuckleClip; // Set the audio clip for the audio source
        audioSource.Play();
    }

    virtual public void PlayAudioCrazyCrying()
    {
        audioSource.clip = crazyCryingClip; // Set the audio clip for the audio source
        audioSource.Play();
    }

    virtual public void PlayAudioScream()
    {
        audioSource.clip = screamClip;
        audioSource.Play();
    }

    public void PlayerIn(bool isOn)
    {
        isPlayerIn = isOn;

        animator.ResetTrigger("Run");
        animator.ResetTrigger("HeadShake");
        animator.ResetTrigger("IDLE");

        if (isPlayerIn && isTimeOver)
        {
            animator.SetTrigger("Run");
            //footstepController.isRunning = true; // Set the footstep controller to running state
            agent.speed = 5.0f;
            PlayAudioCrazyCrying();
        }
        else if (isPlayerIn && !isTimeOver)
        {
            animator.SetTrigger("HeadShake");
            //footstepController.isRunning = false; // Set the footstep controller to walking state
            agent.speed = 0.0f;
            PlayAudioChuckle();
        }
        
        if(!isPlayerIn)
        {
            animator.SetTrigger("IDLE");
            footstepController.isRunning = false; // Set the footstep controller to running state
            agent.speed = 0.0f;
            isTimeOver = false; // Reset the time over flag
            timeOver = 0.0f; // Reset the timer
            target = trReset;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isPlayerIn || !isTimeOver) return; // If the player is not in or time is not over, do nothing

            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager)
            {
                if (playerManager.isStern) return;

                PlayAudioScream();
                //SoundManager.Instance.Play3DSound("GhostScream", other.transform.position);
                footstepController.StopWalking();

                other.GetComponent<PlayerManager>().Stern(); // Call the Die method on the player manager

                StartCoroutine(Disable()); // Start the coroutine to disable the monster after a delay
            }
        }
    }

    public IEnumerator Disable()
    {
        //PlayAudioChuckle();
        yield return new WaitForSeconds(2f); // Wait for 1 second before disabling
        gameObject.SetActive(false); // Deactivate the monster game object
    }
}
