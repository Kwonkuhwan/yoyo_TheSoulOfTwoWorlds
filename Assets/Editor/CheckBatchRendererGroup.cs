// 파일 경로: Assets/Editor/CheckBatchRendererGroup.cs
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

[InitializeOnLoad]
public static class CheckBatchRendererGroup
{
    static CheckBatchRendererGroup()
    {
        // Unity 에디터 초기화 이후에 실행되도록 지연 호출
        EditorApplication.delayCall += Check;
    }

    static void Check()
    {
#if UNITY_6000_0_OR_NEWER
        try
        {
            // Unity 6.0 이후 버전용 확인 코드
            var method = typeof(EditorGraphicsSettings).GetMethod("GetBatchRendererGroupVariants",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            if (method != null)
            {
                var value = method.Invoke(null, null)?.ToString() ?? "Unknown";

                if (value.Contains("KeepAll"))
                {
                    Debug.Log("✅ [Graphics Settings] BatchRendererGroup Variants = KeepAll");
                }
                else
                {
                    Debug.LogWarning(
                        "⚠️ [Graphics Settings] BatchRendererGroup Variants is NOT 'Keep All'.\n" +
                        "To fix: Go to Edit → Project Settings → Graphics → BatchRendererGroup Variants → set to 'Keep All'."
                    );
                }
            }
            else
            {
                Debug.LogWarning("⚠️ [Graphics Settings] Could not find method GetBatchRendererGroupVariants().");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("⚠️ [Graphics Settings] Unable to check BatchRendererGroup Variants.\n" +
                             $"Error: {e.Message}");
        }
#else
        Debug.LogWarning("⚠️ [Graphics Settings] This script is designed for Unity 6000.0 or newer.");
#endif
    }
}
