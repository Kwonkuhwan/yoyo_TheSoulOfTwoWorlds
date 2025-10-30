using FronkonGames.Glitches.Artifacts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterJumpSquare : MonoBehaviour
{
    [SerializeField] private int targetIdx = 0;
    [SerializeField] private List<GameObject> checkPoints;
    [SerializeField] public Transform target;

    [SerializeField] private Animator animator;

    [SerializeField] public bool isPlayerIn = false;
    [SerializeField] protected NavMeshAgent agent;

    [SerializeField] protected AudioClip chuckleClip; // Audio clip for sound effects
    [SerializeField] protected AudioClip crazyCryingClip; // Audio clip for sound effects
    [SerializeField] protected AudioClip screamClip; // Audio clip for sound effects
    [SerializeField] protected AudioSource audioSource; // Audio source for playing sound effects

    [SerializeField] protected FootstepController footstepController;

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

        targetIdx = 0;
    }

    private void Start()
    {
        if(SoundManager.Instance)
        {
            audioSource.volume = SoundManager.Instance.sfxSource.volume;
        }

        PlayAudioChuckle();
        footstepController.StartWalking();
    }

    private void Update()
    {
        agent.SetDestination(target.position); // Set the agent's destination to the target's position               

        if (!isPlayerIn)
        {
            float distanceToTarget = GetNavMeshPathDistance(transform.position, target.position); // Calculate distance to target
            if (distanceToTarget < 1.0f)
            {
                if (targetIdx == 0)
                {
                    targetIdx = 1;
                }
                else
                {
                    targetIdx = 0;
                }
                target = checkPoints[targetIdx].transform;
            }
        }
    }

    private void TargetReSet()
    {
        if (targetIdx == 0)
        {
            targetIdx = 1;
        }
        else
        {
            targetIdx = 0;
        }
        target = checkPoints[targetIdx].transform;
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
        if (isPlayerIn)
        {
            agent.speed = 5.0f;
            PlayAudioCrazyCrying();
            footstepController.isRunning = true; // Set the footstep controller to running state
            animator.SetTrigger("Run");
        }
        else
        {
            agent.speed = 3.0f;
            animator.SetTrigger("Walk");
            footstepController.isRunning = false; // Set the footstep controller to running state
            PlayAudioChuckle();
            TargetReSet();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager)
            {
                if (playerManager.isStern) return;

                Debug.Log("MonsterJumpSquare: Player entered the trigger area.");
                PlayAudioScream();
                SoundManager.Instance.Play3DSound("GhostScream", other.transform.position);
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
