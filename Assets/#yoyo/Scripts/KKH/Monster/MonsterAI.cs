using BNG;
using FronkonGames.Glitches.Artifacts;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class MonsterAI : MonoBehaviour
{
    //[SerializeField] private bool isPlayerDie = false; // Flag to check if the monster is active
    public bool isRun = false; // Flag to check if the monster is running
    [SerializeField] protected float fRunCoolTime = 5f; // Cooldown time for running
    [SerializeField] protected float fRunCoolTimeCounter = 0f; // Counter for the cooldown time

    public NavMeshAgent agent; // Reference to the NavMeshAgent component
    public Transform target;

    [SerializeField] protected float walkSpeed = 1.5f; // Speed of the monster
    [SerializeField] protected float runSpeed = 3f; // Speed of the monster

    [SerializeField] protected bool isArtifacts = true; // Flag to check if artifacts are enabled
    protected Artifacts.Settings ArtifactsSettings; // Reference to the settings for the monster

    [SerializeField] protected float targetMaxDistance = 15.0f; // Maximum distance to the target

    [SerializeField] protected Animator animator; // Animator for the monster

    [SerializeField] protected string responClipName; // Audio clip for sound effects
    [SerializeField] protected AudioClip chuckleClip; // Audio clip for sound effects
    [SerializeField] protected AudioClip crazyCryingClip; // Audio clip for sound effects
    [SerializeField] protected AudioClip getPlayerClip; // Audio clip for sound effects
    [SerializeField] protected AudioSource audioSource; // Audio source for playing sound effects

    [SerializeField] protected FootstepController footstepController; // Reference to the footstep controller

    [SerializeField] private float MaxPlayerTime = 600.0f;
    [SerializeField] private float PlayerTimeCounter = 0.0f; // Counter for the player's time

    virtual public void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>(); // Ensure the Animator component is attached
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); // Ensure the AudioSource component is attached
        }

        // Ensure the NavMeshAgent component is attached
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        agent.speed = walkSpeed; // Set the speed of the NavMeshAgent

    }

    virtual public void Start()
    {
        isArtifacts = true;
        PlayerTimeCounter = 0.0f; // Initialize the player time counter

        //SoundManager.Instance.Play3DSound("WomanChuckle", transform.position); // Play a sound effect when the monster runs
        if (SoundManager.Instance)
        {
            audioSource.volume = SoundManager.Instance.sfxSource.volume;
        }

        PlayAudioRespon();
        PlayAudioChuckle();
        footstepController.StartWalking();
    }

    virtual public void Update()
    {
        agent.SetDestination(target.position); // Set the agent's destination to the target's position

        if (isRun)
        {
            fRunCoolTimeCounter += Time.deltaTime; // Increment the cooldown counter
            if (fRunCoolTimeCounter >= fRunCoolTime)
            {
                isRun = false; // Reset the run state after cooldown
                SetPlayerLook(false);
                fRunCoolTimeCounter = 0f; // Reset the cooldown counter
                footstepController.isRunning = true;

                Debug.Log("Monster has stopped running.");
            }
        }
        else
        {
            footstepController.isRunning = false;
        }

        if (isArtifacts)
        {
            float distanceToTarget = GetNavMeshPathDistance(transform.position, target.position); // Calculate distance to target
            float intensity = 1f - Mathf.Clamp01(distanceToTarget / targetMaxDistance);
            GameManager.Instance.SetArtifacts(intensity); // Set the artifacts intensity based on distance
        }

        if (MaxPlayerTime > PlayerTimeCounter)
        {
            PlayerTimeCounter += Time.deltaTime; // Increment the player time counter
        }
        else
        {
            Run(15.0f); // Increase speed when player dies
        }
    }

    float GetNavMeshPathDistance(Vector3 start, Vector3 end)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path))
        {
            float totalDistance = 0f;
            for (int i = 1; i < path.corners.Length; i++)
            {
                totalDistance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
            return totalDistance;
        }
        else
        {
            Debug.LogWarning("NavMesh 경로 계산 실패");
            return -1f;
        }
    }

    virtual public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (!playerManager.isPlayerDie)
            {
                footstepController.StopWalking();

                // Logic for when the monster collides with the player
                //PlayAudioGetPlayer();
                SoundManager.Instance.Play3DSound("DiePlayer", other.transform.position); // Play a sound effect when the monster gets the player
                SoundManager.Instance.Play3DSound("GetPlayer", other.transform.position); // Play a sound effect when the monster gets the player
                isArtifacts = false; // Disable artifacts when the monster collides with the player
                                     // You can add more logic here, like triggering an attack or a chase
                GameManager.Instance.ReSetArtifacts(); // Reset the artifacts intensity

                playerManager.Die(); // Call the Die method on the player manager
                gameObject.SetActive(false); // Deactivate the monster game object

                //StartCoroutine(Disable()); // Start the coroutine to disable the monster after a delay}
            }
        }
    }

    virtual public IEnumerator Disable()
    {
        yield return new WaitForSeconds(3f); // Wait for 1 second before disabling
        gameObject.SetActive(false); // Deactivate the monster game object
    }

    virtual public void PlayAudioRespon()
    {
        SoundManager.Instance.Play3DSound(responClipName, transform.position);
    }

    virtual public void PlayAudioChuckle()
    {
        audioSource.clip = chuckleClip; // Set the audio clip for the audio source
        audioSource.loop = true;
        audioSource.Play();
    }

    virtual public void PlayAudioCrazyCrying()
    {
        audioSource.clip = crazyCryingClip; // Set the audio clip for the audio source
        audioSource.loop = false;
        audioSource.Play();
    }

    virtual public void PlayAudioGetPlayer()
    {
        audioSource.clip = getPlayerClip; // Set the audio clip for the audio source
        audioSource.loop = false;
        audioSource.Play();
    }

    virtual public void SetPlayerLook(bool value)
    {
        isRun = value; // Update the player die state
        if (isRun)
        {
            Run(runSpeed); // Increase speed when player dies
        }
        else
        {
            Walk(walkSpeed); // Reset speed when player is alive
        }
    }

    private void Run(float speed)
    {
        agent.speed = speed; // Increase speed when player dies
        PlayAudioCrazyCrying();
        animator.SetTrigger("Run"); // Trigger the run animation
    }
    private void Walk(float speed)
    {
        agent.speed = speed; // Reset speed when player is alive
                                 //SoundManager.Instance.Play3DSound("WomanChuckle", transform.position); // Play a sound effect when the monster runs
        PlayAudioChuckle();
        animator.SetTrigger("Walk"); // Trigger the run animation
    }

    public void Stren()
    {
        StartCoroutine(IEStren()); // Start the coroutine to handle the stren action
    }

    private IEnumerator IEStren()
    {
        agent.speed = 0.0f;
        yield return new WaitForSeconds(3.0f); // Wait for 3 seconds
        Walk(walkSpeed);

    }
}


