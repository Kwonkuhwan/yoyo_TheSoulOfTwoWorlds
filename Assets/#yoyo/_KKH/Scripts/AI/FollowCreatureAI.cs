using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class FollowCreatureAI : MonoBehaviour
{
    public NavMeshAgent agent;
    [Header("Patrol")]
    public Transform waypointParent;
    public List<Transform> waypoints;
    public float waypointArriveThreshold = 0.5f;
    public bool loop = true;

    [Header("Detection")]
    public Transform player;
    public float viewRange = 12f;       // 최대 시야 거리
    public float viewAngle = 90f;       // 시야 각도(양쪽 합)
    public float detectionCooldown = 2f; // 플레이어를 놓친 뒤 버티는 시간

    [Header("Chase")]
    [SerializeField] private LayerMask detectionMask; // Player | Obstacles

    [SerializeField] private float agentEyeHeight = 1.6f;
    [SerializeField] private float playerAimHeight = 1.6f; // 플레이어 머리/가슴 높이에 맞게

    public float stoppingToPlayer = 1.2f;   // 플레이어 앞에서 멈출 거리
    public float repathInterval = 0.1f;     // 경로 갱신 주기(초)
    public bool faceMoveDirection = true;   // 부드러운 회전

    private int _patrolIndex = 0;
    private float _lastSeenTime = -999f;
    private float _nextRepathTime = 0f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string Patrol = "Patrol";
    [SerializeField] private string Chase = "Chase";
    [SerializeField] private string Sleep = "Sleep";


    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip patrolClip;
    [SerializeField] private AudioClip chaseClip;

    private enum State { Patrol, Chase }
    private State _state = State.Patrol;

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.loop = true;
            audioSource.clip = patrolClip;
        }
    }

    void Start()
    {
        if (agent)
        {
            agent.autoBraking = false; // 목적지 전환 시 급정지 방지
        }

        if (waypointParent != null && waypoints.Count == 0)
        {
            for (int i = 0; i < waypointParent.childCount; i++)
            {
                waypoints.Add(waypointParent.GetChild(i));
            }
        }
        else
        {
            Debug.LogError("WayPointParent is Null or waypoints count over");
            return;
        }
        if (waypoints != null && waypoints.Count
            > 0)
            SetPatrolDestination();

        if (animator)
        {
            animator.SetTrigger("Patrol");
        }

        if (audioSource)
        {
            audioSource.Play();
        }
    }

    void Update()
    {
        bool canSee = CanSeePlayer();

        // 상태 전이
        if (canSee)
        {
            _lastSeenTime = Time.time;
            _state = State.Chase;
        }
        else
        {
            if (Time.time - _lastSeenTime > detectionCooldown)
                _state = State.Patrol;
        }

        // 상태 동작
        switch (_state)
        {
            case State.Patrol:
                PatrolUpdate();
                break;
            case State.Chase:
                ChaseUpdate();
                break;
        }

        // 부드러운 회전(옵션)
        if (faceMoveDirection && agent.velocity.sqrMagnitude > 0.05f)
        {
            Quaternion t = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, t, Time.deltaTime * 6f);
        }
    }

    // ===== Patrol =====
    void PatrolUpdate()
    {
        agent.stoppingDistance = 0f;
        if (agent.pathPending) return;

        if (!agent.hasPath || agent.remainingDistance <= waypointArriveThreshold)
        {
            AdvancePatrol();
        }
    }

    void AdvancePatrol()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        if (audioSource && audioSource.clip != patrolClip)
        {
            audioSource.clip = patrolClip;
        }

        _patrolIndex++;
        if (_patrolIndex >= waypoints.Count)
        {
            if (loop) _patrolIndex = 0;
            else _patrolIndex = waypoints.Count - 1; // 마지막에서 정지
        }
        SetPatrolDestination();
    }

    void SetPatrolDestination()
    {
        if (waypoints == null || waypoints.Count == 0) return;
        agent.SetDestination(waypoints[_patrolIndex].position);
    }

    // ===== Chase =====
    void ChaseUpdate()
    {
        if (!player) return;

        agent.stoppingDistance = stoppingToPlayer;

        // 경로 갱신 간격으로 부드럽게 추격(너무 자주 갱신하면 떨림)
        if (Time.time >= _nextRepathTime)
        {
            agent.SetDestination(player.position);
            _nextRepathTime = Time.time + repathInterval;
        }

        // 멈출 거리 이내면 속도 줄이고 시선 맞추기(선택)
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
        {
            agent.velocity = Vector3.Lerp(agent.velocity, Vector3.zero, Time.deltaTime * 5f);

            // 플레이어 바라보기(원하면 사용)
            Vector3 dir = (player.position - transform.position);
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion look = Quaternion.LookRotation(dir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 8f);
            }
        }

        if (audioSource && audioSource.clip != chaseClip)
        {
            audioSource.clip = chaseClip;
        }
    }

    // ===== Vision =====
    bool CanSeePlayer()
    {
        if (!player) return false;

        // 1) 거리‧각도 1차 필터 (이전 로직 유지)
        Vector3 toPlayer3D = player.position - transform.position;
        float dist = toPlayer3D.magnitude;
        if (dist > viewRange)
        {
            return false;
        }

        Vector3 dirFlat = new Vector3(toPlayer3D.x, 0f, toPlayer3D.z).normalized;
        float angle = Vector3.Angle(transform.forward, dirFlat);
        if (angle > viewAngle * 0.5f)
        {
            return false;
        }

        // 2) 눈 위치/목표 위치
        Vector3 eye = transform.position + Vector3.up * agentEyeHeight;
        Vector3 target = player.position + Vector3.up * playerAimHeight;

        // 3) 자기 자신과의 간섭을 줄이기 위해 약간 앞으로 오프셋
        eye += transform.forward * 0.1f;

        // 4) Linecast: Player | Obstacles 만 맞도록 (트리거 무시)
        if (Physics.Linecast(eye, target, out RaycastHit hit, detectionMask, QueryTriggerInteraction.Ignore))
        {
            // 플레이어인지 판정 (루트/자식 모두 허용)
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

        // 라인에 아무 것도 안 맞으면 보통 '보인다'고 하지 않음 (필요 시 true로 바꿀 수 있음)
        return false;
    }

    // 디버그 시야 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRange);

        // 시야각 라인
        Vector3 left = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, +viewAngle * 0.5f, 0) * transform.forward;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + left * viewRange);
        Gizmos.DrawLine(transform.position, transform.position + right * viewRange);
    }
}
