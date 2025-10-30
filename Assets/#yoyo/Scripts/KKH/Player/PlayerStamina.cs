using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    public float stamina = 100f;
    public float maxStamina = 100f;

    public float staminaDrainPerSecond = 20f;  // 초당 20% 소모
    public float staminaRegenAmount = 15f;     // 2초마다 15% 회복

    public bool isRunning = false;
    public bool isGrappling = false; // 예시로 추가, 실제 그랩핑 로직은 필요에 따라 구현

    [SerializeField] private Slider staminaSlider; // UI 슬라이더를 사용하여 스태미나 표시

    [SerializeField] private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
        HandleStamina();
    }

    void HandleMovement()
    {
        // 달리기: Shift 키 + 이동 중
        bool tryStartRunning = isGrappling && !isRunning && stamina >= 25f;
        bool keepRunning = isRunning && isGrappling && stamina > 0f;

        isRunning = tryStartRunning || keepRunning;

        if (isRunning)
        {
            stamina -= staminaDrainPerSecond * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
        else
        {
            stamina += staminaRegenAmount * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0f, maxStamina);
        }
    }

    void HandleStamina()
    {
        if(stamina >= 100.0f)
        {
            staminaSlider.gameObject.SetActive(false); // 스태미나가 100%일 때 슬라이더 숨김
        }
        else
        {
            if (!staminaSlider.gameObject.activeInHierarchy)
            {
                staminaSlider.gameObject.SetActive(true); // 스태미나가 100%일 때 슬라이더 숨김
            }

            staminaSlider.value = stamina / maxStamina; // 슬라이더 값 업데이트
        }
    }
}
