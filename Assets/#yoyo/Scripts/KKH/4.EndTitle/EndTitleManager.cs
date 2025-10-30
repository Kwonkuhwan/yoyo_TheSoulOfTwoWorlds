using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndTitleManager : MonoBehaviour
{
    public GameObject bgReTry;
    public GameObject bgEnd;
    public float fadeDuration = 1f;  // 페이드 아웃 지속 시간


    [SerializeField] private Button btn_Yes;
    [SerializeField] private Button btn_No;

    private void Awake()
    {
        btn_Yes.onClick.AddListener(() => YesBtnClick());
        btn_No.onClick.AddListener(() => NoBtnClick());
    }

    void Start()
    {
        //StartCoroutine(End());
    }

    private void YesBtnClick()
    {
        FadeOut(bgReTry, false);
        StartCoroutine(NextScen("3.Main_Game", 2.0f));
    }

    private void NoBtnClick()
    {
        StartCoroutine(FadeOut(bgReTry, true));
    }

    IEnumerator End()
    {
        yield return new WaitForSeconds(5.0f);

        if (GameManager.Instance)
        {
        }
    }

    private IEnumerator FadeOut(GameObject obj, bool isEnd)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();

        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        obj.SetActive(false);

        if (isEnd)
        {
            StartCoroutine(FadeIn(bgEnd));
        }
    }

    public IEnumerator FadeIn(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();

        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        StartCoroutine(NextScen("1.StartTitle", 5.0f));
    }

    private IEnumerator NextScen(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (GameManager.Instance)
        {
            GameManager.Instance.LoadNextScene(sceneName);
        }
    }
}
