using UnityEngine;

public class JampScare_Zombie : MonoBehaviour
{
    enum SoundType
    {
        Zombie_Sound_1,
        Zombie_Sound_2,
        Zombie_Sound_3,
        Zombie_Sound_4,
    }

    [SerializeField] private Animator animator;
    [SerializeField] private Collider triggerCollider;
    [SerializeField] private SoundType soundType;

    private void Awake()
    {
        if(!animator) animator = GetComponent<Animator>();
        if (!triggerCollider) triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.I.PlaySFXAt(soundType.ToString(),transform.position);

            animator.SetTrigger("JampScare");
            triggerCollider.enabled = false; // 한 번만 작동하도록 콜라이더 비활성화
        }
    }
}
