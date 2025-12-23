using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SightSoundCreatureAI : MonoBehaviour, ISoundListener
{
    [Header("Hearing")]
    [Tooltip("실제로 들을 반경은 Emitter에서 제어합니다. 여기선 단순히 마지막 들은 소리 위치로 이동합니다.")]
    public float stoppingDistance = 0.5f;

    [Header("Sleep Cycle")]
    public float cycleSeconds = 30f;
    public float sleepSeconds = 15f;
    public float fallAnimSeconds = 0.4f;
    public float standAnimSeconds = 0.4f;

    [Header("Animation (Optional)")]
    public Animator animator;
    [SerializeField] private string animParamIsMoving = "IsMoving";
    [SerializeField] private string animTriggerWake = "Wake";
    [SerializeField] private string animTriggerSleep = "Sleep";

    private NavMeshAgent agent;
    private bool isSleeping;
    private Quaternion uprightRotation;
    private Quaternion fallenRotation;
    private float timeInCycle;
    private Vector3? lastHeardSoundPos;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip sleepSound;
    [SerializeField] private AudioClip wakeSound;
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioSource audioSource;

    // ====== Vision / Chase ======
    [Header("Vision")]
    [SerializeField] private Transform player;
    [SerializeField] private float viewRange = 12f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private float agentEyeHeight = 1.6f;
    [SerializeField] private float playerAimHeight = 1.6f;

    [Header("Chase")]
    [SerializeField] private float detectionCooldown = 2f;
    [SerializeField] private float stoppingToPlayer = 1.2f;
    [SerializeField] private float repathInterval = 0.1f;
    [SerializeField] private bool faceMoveDirection = true;

    [Header("Sound Reaction")]
    [Tooltip("소리로 먼저 '무장'해야 시야 추격이 허용됩니다.")]
    [SerializeField] private bool requireSoundFirst = true;           // ★ 추가
    [Tooltip("소리 후 N초 동안만 시야 추격 허용(<=0 이면 무제한)")]
    [SerializeField] private float soundArmWindow = 5f;               // ★ 추가
    private float soundArmExpire = -999f;                              // ★ 추가

    private float _lastSeenTime = -999f;
    private float _nextRepathTime = 0f;
    private bool isInvestigatingSound = false;
    [SerializeField] private bool _wasMoving = false;

    private bool isChasing => Time.time - _lastSeenTime <= detectionCooldown;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();

        uprightRotation = transform.rotation;
        fallenRotation = Quaternion.AngleAxis(90f, transform.right) * uprightRotation;

        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (audioSource) audioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        timeInCycle = 0f;
        isSleeping = false;
        SetMoving(false);
        lastHeardSoundPos = null;
        isInvestigatingSound = false;
        soundArmExpire = -999f;   // ★ 초기화
    }

    private void Update()
    {
        // ----- Sleep cycle -----
        timeInCycle += Time.deltaTime;
        if (timeInCycle >= cycleSeconds) timeInCycle -= cycleSeconds;

        bool shouldSleep = timeInCycle < sleepSeconds;
        if (shouldSleep && !isSleeping) EnterSleep();
        else if (!shouldSleep && isSleeping) ExitSleep();

        if (isSleeping)
        {
            StopMove();
            return;
        }

        // ----- Vision & gating -----
        bool canSee = CanSeePlayer();
        if (canSee)
        {
            _lastSeenTime = Time.time;
            isInvestigatingSound = false; // 시야 확보되면 소리 기인 모드 해제
        }

        // ★ 소리로 '무장'되었는지
        bool soundArmed = !requireSoundFirst || (soundArmWindow <= 0f ? (soundArmExpire > 0f) : (Time.time <= soundArmExpire));
        // ★ 시야 추격 허용 여부: '무장' + 감지 유지 시간
        bool allowSightChase = soundArmed && isChasing && player;

        // ----- Decision -----
        if (allowSightChase && canSee)
        {
            // 시야로 직접 추격 (소리 우선이지만, 일단 소리로 무장한 뒤엔 시야 추격 허용)
            agent.stoppingDistance = stoppingToPlayer;
            if (Time.time >= _nextRepathTime)
            {
                agent.SetDestination(player.position);
                _nextRepathTime = Time.time + repathInterval;
            }

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
            {
                agent.velocity = Vector3.Lerp(agent.velocity, Vector3.zero, Time.deltaTime * 5f);
                Vector3 dir = (player.position - transform.position); dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                {
                    Quaternion look = Quaternion.LookRotation(dir.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 8f);
                }
            }
        }
        else
        {
            // 시야 추격 불가 → 소리 타깃이 있으면 그쪽으로, 없으면 정지
            if (isInvestigatingSound && lastHeardSoundPos.HasValue)
            {
                if (!agent.hasPath || Time.time >= _nextRepathTime)
                {
                    agent.stoppingDistance = stoppingDistance;
                    agent.SetDestination(lastHeardSoundPos.Value);
                    _nextRepathTime = Time.time + repathInterval;
                }
            }
            else
            {
                StopMove();
            }
        }

        // ----- Arrival -----
        if (agent.hasPath)
        {
            SetMoving(agent.velocity.sqrMagnitude > 0.01f);

            if (!agent.pathPending && agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, stoppingDistance))
            {
                StopMove();

                if (isInvestigatingSound)
                {
                    isInvestigatingSound = false;
                    lastHeardSoundPos = null;

                    // 예시: 도착 즉시 수면 (원치 않으면 주석)
                    timeInCycle = 0f;
                    if (!isSleeping) EnterSleep();
                }
            }
        }
        else
        {
            SetMoving(false);
        }

        // 부드러운 회전
        if (faceMoveDirection && agent.velocity.sqrMagnitude > 0.05f)
        {
            Quaternion t = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, t, Time.deltaTime * 6f);
        }

        // ★ 소리 무장 해제 조건: 시야도 없고, 소리 추적도 종료되고, 감지 유지시간도 지났을 때
        if (requireSoundFirst)
        {
            bool noSight = !canSee && !isChasing;
            bool noSoundWork = !isInvestigatingSound && !lastHeardSoundPos.HasValue && !agent.hasPath;
            if (noSight && noSoundWork)
            {
                // 창구 만료가 지났으면 비무장 상태로
                if (soundArmWindow > 0f && Time.time > soundArmExpire)
                    soundArmExpire = -999f;
            }
        }
    }

    // ===== Movement helpers =====
    private void SetMoving(bool moving)
    {
        if (animator && !string.IsNullOrEmpty(animParamIsMoving))
        {
            if (!_wasMoving)
                animator.SetBool(animParamIsMoving, moving);
        }

        if (audioSource)
        {
            if (moving)
            {
                if (audioSource.clip != moveSound) audioSource.clip = moveSound;
                if (!audioSource.isPlaying && moveSound) audioSource.Play();
            }
            else
            {
                if (audioSource.isPlaying && audioSource.clip == moveSound)
                    audioSource.Stop();
            }
        }

        _wasMoving = moving;
    }

    private void StopMove()
    {
        if (agent.enabled)
        {
            if (agent.hasPath) agent.ResetPath();
            agent.velocity = Vector3.zero;
        }
        SetMoving(false);
    }

    // ===== Sleep =====
    private void EnterSleep()
    {
        isSleeping = true;
        StopMove();

        if (animator && !string.IsNullOrEmpty(animTriggerSleep))
            animator.SetTrigger(animTriggerSleep);
        else
        {
            StopAllCoroutines();
            StartCoroutine(RotateOverTime(transform.rotation, fallenRotation, fallAnimSeconds));
        }

        if (audioSource && sleepSound)
        {
            if (audioSource.isPlaying) audioSource.Stop();
            audioSource.clip = sleepSound;
            audioSource.Play();
        }
    }

    private void ExitSleep()
    {
        isSleeping = false;

        if (animator && !string.IsNullOrEmpty(animTriggerWake))
            animator.SetTrigger(animTriggerWake);
        else
        {
            StopAllCoroutines();
            StartCoroutine(RotateOverTime(transform.rotation, uprightRotation, standAnimSeconds));
        }

        if (audioSource && wakeSound)
        {
            if (audioSource.isPlaying) audioSource.Stop();
            audioSource.clip = wakeSound;
            audioSource.Play();
        }

        // 기상 시: (무장 상태 && 소리 타깃) 이면 이어서 수사
        bool soundArmed = !requireSoundFirst || (soundArmWindow <= 0f ? (soundArmExpire > 0f) : (Time.time <= soundArmExpire));
        if (soundArmed && lastHeardSoundPos.HasValue)
            MoveTo(lastHeardSoundPos.Value);
    }

    private System.Collections.IEnumerator RotateOverTime(Quaternion from, Quaternion to, float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / seconds);
            transform.rotation = Quaternion.Slerp(from, to, k);
            yield return null;
        }
        transform.rotation = to;
    }

    // ===== Hearing =====
    public void HearSound(Vector3 position, float loudness)
    {
        if (isSleeping) return;

        // ★ 소리로 '무장' (무제한 원하면 soundArmWindow <= 0f로)
        soundArmExpire = (soundArmWindow <= 0f) ? (Time.time + 999999f) : (Time.time + soundArmWindow);

        lastHeardSoundPos = position;
        isInvestigatingSound = true;
        MoveTo(position);
    }

    private void MoveTo(Vector3 worldPos)
    {
        if (!agent || !agent.enabled) return;

        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(worldPos);
        SetMoving(true);
    }

    // ===== Vision =====
    private bool CanSeePlayer()
    {
        if (!player) return false;

        Vector3 toPlayer3D = player.position - transform.position;
        float dist = toPlayer3D.magnitude;
        if (dist > viewRange) return false;

        Vector3 dirFlat = new Vector3(toPlayer3D.x, 0f, toPlayer3D.z).normalized;
        float angle = Vector3.Angle(transform.forward, dirFlat);
        if (angle > viewAngle * 0.5f) return false;

        Vector3 eye = transform.position + Vector3.up * agentEyeHeight;
        Vector3 target = player.position + Vector3.up * playerAimHeight;
        eye += transform.forward * 0.1f;

        if (Physics.Linecast(eye, target, out RaycastHit hit, detectionMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform == player || hit.transform.IsChildOf(player))
            {
                Debug.DrawLine(eye, target, Color.green, 0.1f);
                return true;
            }
            else
            {
                Debug.DrawLine(eye, hit.point, Color.red, 0.1f);
                return false;
            }
        }
        return false;
    }

    // ===== Debug Gizmos =====
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRange);

        Vector3 left = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, +viewAngle * 0.5f, 0) * transform.forward;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + left * viewRange);
        Gizmos.DrawLine(transform.position, transform.position + right * viewRange);
    }
}

// 리스너 인터페이스
public interface ISoundListener
{
    void HearSound(Vector3 position, float loudness);
}
