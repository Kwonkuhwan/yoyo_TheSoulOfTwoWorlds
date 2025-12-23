using UnityEngine;
using TMPro;
using BNG;
public class FPS : MonoBehaviour
{
    TextMeshProUGUI fpsText;
    float deltaTime;
    

    private void Awake()
    {
        fpsText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        // 매 프레임 마다 델타타임을 보정해서 저장
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // FPS 계산
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        fpsText.text = $"{Mathf.FloorToInt(fps)}fps({msec.ToString("F2")}ms)";
    }
}