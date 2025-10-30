using UnityEngine;

public class MonsterHeadLookAt : MonoBehaviour
{
    [SerializeField] private Transform target; // The target to look at

    public Vector3 offsetEuler = new Vector3(0, -90.0f, 23.508f); // Y축으로 +90도 회전시켜 정면 보정

    void Update()
    {
        Vector3 targetPos = target.position;
        targetPos.y = transform.position.y; // 수직 회전 제거

        // 기본 LookRotation
        Quaternion lookRot = Quaternion.LookRotation(targetPos - transform.position);

        // 회전 오프셋 적용
        transform.rotation = lookRot * Quaternion.Euler(offsetEuler);
    }
}
