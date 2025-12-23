using UnityEngine;
using static Oculus.Interaction.Context;

[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour
{
    public static Flashlight instance { get; private set; }

    public KeyCode toggleKey = KeyCode.F;
    public float onIntensity = 2.5f;
    public float offIntensity = 0f;
    public float smoothTime = 0.12f; // 부드러운 전환

    private Light spot;
    private float target;
    private float vel;

    void Awake()
    {
        instance = this;

        spot = GetComponent<Light>();
        spot.type = LightType.Spot;
        target = spot.intensity; // 초기값 유지
    }

    private void Start()
    {
        ToggleOff();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            Toggle();
        }

        // 부드럽게 보간
        spot.intensity = Mathf.SmoothDamp(spot.intensity, target, ref vel, smoothTime);

        if (target > 0f)
        {
            float noise = (Mathf.PerlinNoise(Time.time * 12f, 0f) - 0.5f) * 0.15f;
            spot.intensity = Mathf.Clamp(spot.intensity + noise, 0f, onIntensity * 1.1f);
        }
    }

    // (선택) 모바일용 토글
    public void Toggle()
    {
        bool turnOn = spot.intensity <= 0.01f;
        target = turnOn ? onIntensity : offIntensity;
        if(SoundManager.I != null)
        {
            SoundManager.I.PlaySFXAt("Light_Switch", transform.position);
        }
    }

    public void ToggleOff()
    {
        target = offIntensity;
        if (SoundManager.I != null)
        {
            SoundManager.I.PlaySFXAt("Light_Switch", transform.position);
        }
    }

    public void ToggleOn()
    {
        target = onIntensity;
        if (SoundManager.I != null)
        {
            SoundManager.I.PlaySFXAt("Light_Switch", transform.position);
        }
    }
}