using System.Collections;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [Range(0.0f, 1.0f)] public float dissolveValue = 0.0f;
    public Renderer[] renderers;
    public Shader dissolveSharder;
    public Shader orgSharder;

    public bool isDissolving = false;

    float dissolveDuration = 3f;
    float startTime;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        if (isDissolving)
        {
            float elapsed = Time.time - startTime; // 경과 시간
            float value = Mathf.Clamp01(elapsed / dissolveDuration);
            dissolveValue = value;
            SetDissolveValue(value);
        }
    }

    private IEnumerator DisableObject()
    {
        yield return new WaitForSeconds(5.0f);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (dissolveSharder == null || orgSharder == null)
        {
            Debug.LogError("Please assign the dissolve and original shaders in the inspector.");
            return;
        }
        SetDissolveMaterial();
        StartCoroutine(DisableObject());
    }

    private void OnDisable()
    {
        if (dissolveSharder == null || orgSharder == null)
        {
            Debug.LogError("Please assign the dissolve and original shaders in the inspector.");
            return;
        }

        ResetMaterial();
    }

    private void SetDissolveMaterial()
    {
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                material.shader = dissolveSharder;
            }
        }

        isDissolving = true;
        startTime = Time.time; // 시작 시간 기록
    }

    private void ResetMaterial()
    {
        isDissolving = false;
        dissolveValue = 0.0f;
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                material.shader = orgSharder;
            }
        }
    }

    private void SetDissolveValue(float value)
    {
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                material.SetFloat("_Dissolve", value);
            }
        }
    }
}
