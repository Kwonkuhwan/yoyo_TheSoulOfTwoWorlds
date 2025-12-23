using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [Tooltip("소리가 닿는 반경")]
    public float radius = 15f;
    [Tooltip("소리의 기본 세기 (중심에서의 최대값)")]
    public float baseLoudness = 1f;
    [Tooltip("디버그용 기즈모")]
    public bool showGizmo = true;

    public void Emit()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, ~0, QueryTriggerInteraction.Ignore);
        foreach (var h in hits)
        {
            var listeners = h.GetComponentsInChildren<ISoundListener>();
            foreach (var l in listeners)
            {
                float distance = Vector3.Distance(transform.position, h.transform.position);

                // 거리 기반 감쇠 (거리가 멀수록 작게)
                // 1 / (1 + distance² / radius²) 형태로 하면 꽤 자연스럽습니다.
                float falloff = 1f / (1f + (distance * distance) / (radius * radius));

                // 최종 소리 세기
                float loudness = baseLoudness * falloff;

                l.HearSound(transform.position, loudness);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
