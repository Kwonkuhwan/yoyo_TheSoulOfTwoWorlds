using BNG;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartUIManager : MonoBehaviour
{
    [SerializeField] private Image img_Background;

    [SerializeField] private CanvasGroup canvas_Title;
    [SerializeField] private CanvasGroup canvas_Popups;

    [SerializeField] private bool isStart = false;
    [SerializeField] private bool isCrutchesFadeStart = false;
    [SerializeField] private bool isIDCardFadeStart = false;
    [SerializeField] private CanvasGroup canvas_Crutches;
    [SerializeField] private CanvasGroup canvas_IDCard;


    public void OnClickStartButton()
    {
        StartCoroutine(IEFadeShow());
    }

    private void Update()
    {
        if ((InputBridge.Instance.RightTriggerDown || InputBridge.Instance.LeftTriggerDown || Input.GetKeyDown(KeyCode.PageDown)) && !isStart)
        {
            isStart = true;
            Debug.Log("Skip Intro");
            OnClickStartButton();
        }
    }

    IEnumerator IEFadeShow()
    {
        StartCoroutine(FadePopup(canvas_Title, canvas_Popups));

        yield return new WaitForSeconds(5.0f);
        if (canvas_Crutches.alpha == 1 && !isCrutchesFadeStart)
        {
            isCrutchesFadeStart = true;
            StartCoroutine(FadePopup(canvas_Crutches, canvas_IDCard));
        }

        yield return new WaitForSeconds(5.0f);
        if (canvas_IDCard.alpha == 1 && !isIDCardFadeStart)
        {
            isIDCardFadeStart = true;
            StartCoroutine(FadePopup(canvas_IDCard));
        }

        MapManager.instance.isStart = true;
    }

    IEnumerator FadePopup(CanvasGroup canvasGroup1, CanvasGroup canvasGroup2 = null, float duration = 1f)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            canvasGroup1.alpha = 1f - alpha;

            if (canvasGroup2 != null)
                canvasGroup2.alpha = alpha;
            yield return null;
        }
        canvasGroup1.alpha = 0f;
        if (canvasGroup2 != null)
            canvasGroup2.alpha = 1f;
    }
}
