using BNG;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentController : MonoBehaviour
{
    [SerializeField] private bool isInitialized = false;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform waypointParent;
    [SerializeField] private List<Transform> waypoints;
    private int currentIndex = 0;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    void Start()
    {
        if (agent)
        {
            Init();
        }
    }

    private void Init()
    {
        try
        {
            gameObject.tag = "NavMonster";
        }
        catch
        {
            Debug.LogError("Not Tag is NavMonster.");
            return;
        }

        try
        {
            gameObject.layer = LayerMask.NameToLayer("NavMonster");
        }
        catch
        {
            Debug.LogError("Not Layer is NavMonster.");

            return;
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

        if (waypoints.Count == 0)
        {
            Debug.LogError("No target transforms assigned for NavMeshAgentController.");
            return;
        }

       

        if (!MoveToNextPoint())
        {
            Debug.LogWarning("MoveToNextPoint is false");
            return;
        }

        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized) return;
        if (RemainingDistance())
        {
            MoveToNextPoint();
        }

        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private bool MoveToNextPoint()
    {
        if (waypoints.Count == 0) return false;

        agent.SetDestination(waypoints[currentIndex].position);
        currentIndex = (currentIndex + 1) % waypoints.Count;  // 순환 이동
        return true;
    }

    private bool RemainingDistance()
    {
        // 경로가 계산된 상태인지 확인
        if (!agent.pathPending)
        {
            if (agent.remainingDistance < 0.5f)
            {
                return true;
            }
        }

        return false;
    }
}
