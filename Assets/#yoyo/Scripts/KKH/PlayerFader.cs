using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PlayerFader : MonoBehaviour
{    
    [Tooltip("Color of the fade. Alpha will be modified when fading in / out")]
    public Color FadeColor = Color.black;

    [Tooltip("How fast to fade in / out")]
    public float FadeInSpeed = 6f;

    public float FadeOutSpeed = 6f;

    [Tooltip("Wait X seconds before fading scene in")]
    public float SceneFadeInDelay = 1f;

    [SerializeField] private CanvasGroup canvasGroup;
    IEnumerator fadeRoutine;

    /// <summary>
    /// Fade from transparent to solid color
    /// </summary>
    public virtual void DoFadeIn()
    {

        // Stop if currently running
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        // Do the fade routine
        if (canvasGroup != null)
        {
            fadeRoutine = doFade(canvasGroup.alpha, 1);
            StartCoroutine(fadeRoutine);
        }
    }

    /// <summary>
    /// Fade from solid color to transparent
    /// </summary>
    public virtual void DoFadeOut()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = doFade(canvasGroup.alpha, 0);
        StartCoroutine(fadeRoutine);
    }

    public virtual void SetFadeLevel(float fadeLevel)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            // Debug.Log("----- Stopped Routine");
        }

        // No Canvas available to fade
        if (canvasGroup == null)
        {
            return;
        }

        fadeRoutine = doFade(canvasGroup.alpha, fadeLevel);
        StartCoroutine(fadeRoutine);
    }

    IEnumerator doFade(float alphaFrom, float alphaTo)
    {

        float alpha = alphaFrom;

        updateImageAlpha(alpha);

        while (alpha != alphaTo)
        {

            if (alphaFrom < alphaTo)
            {
                alpha += Time.deltaTime * FadeInSpeed;
                if (alpha > alphaTo)
                {
                    alpha = alphaTo;
                }
            }
            else
            {
                alpha -= Time.deltaTime * FadeOutSpeed;
                if (alpha < alphaTo)
                {
                    alpha = alphaTo;
                }
            }

            updateImageAlpha(alpha);

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        // Ensure alpha is always applied
        updateImageAlpha(alphaTo);
    }

    protected virtual void updateImageAlpha(float alphaValue)
    {

        // Canvas Group was Destroyed.
        if (canvasGroup == null)
        {
            return;
        }

        // Enable canvas if necessary
        if (!canvasGroup.gameObject.activeSelf)
        {
            canvasGroup.gameObject.SetActive(true);
        }

        canvasGroup.alpha = alphaValue;

        // Disable Canvas if we're done
        if (alphaValue == 0 && canvasGroup.gameObject.activeSelf)
        {
            canvasGroup.gameObject.SetActive(false);
        }
    }
}
