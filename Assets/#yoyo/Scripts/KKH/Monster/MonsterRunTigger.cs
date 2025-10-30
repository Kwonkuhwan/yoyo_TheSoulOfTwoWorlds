using System;
using UnityEngine;

public class MonsterRunTigger : MonoBehaviour
{
    public Camera ViewCam;
    private bool hasRay = false;
    [SerializeField] private float Distance = 100f; // Raycast 거리 설정

    private Vector3 lastRayOrigin;
    private Vector3 lastRayDirection;

    void Update()
    {
        Ray ray = new Ray(ViewCam.transform.position, ViewCam.transform.forward);
        RaycastHit hit;

        lastRayOrigin = ray.origin;
        lastRayDirection = ray.direction * Distance; // 길이 조정
        hasRay = true;

        if (Physics.Raycast(ray, out hit, Distance))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Monster"))
            {
                Debug.Log("몬스터 태그됨: " + hitObject.name);
                MonsterAI monsterAI = hitObject.GetComponent<MonsterAI>();
                if (monsterAI)
                {
                    if (!monsterAI.isRun)
                    {
                        monsterAI.SetPlayerLook(true); // 몬스터가 플레이어를 바라보도록 설정
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (hasRay)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(lastRayOrigin, lastRayDirection);
            Gizmos.DrawSphere(lastRayOrigin + lastRayDirection, 0.2f); // 끝점 시각화
        }
    }
}
