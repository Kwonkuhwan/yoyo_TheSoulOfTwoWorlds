using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartTitleManager : MonoBehaviour
{
    public GameObject Warring1;
    public Button btn_Warring1;

    public GameObject Warring2;
    public bool isWarring2 = false; // Warring2가 활성화되었는지 여부를 나타내는 변수

    public GameObject Menual;

    public GameObject bgDocument;
    public Image image_Document;
    public Button btn_Next;

    public GameObject bgStartGame;

    public float fadeDuration = 1f;  // 페이드 아웃 지속 시간

    [SerializeField] private float StartDelayTime = 10.0f;
    [SerializeField] private List<TMP_Text> text_Notions;

    public AudioSource audioSource;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
        }

        btn_Warring1.onClick.AddListener(() => BtnWarringOKClick());
        btn_Next.onClick.AddListener(() => NextDocument());
    }

    private void Update()
    {
        if (Warring2.activeSelf)
        {
            if (Warring2.GetComponent<CanvasGroup>().alpha == 1f && !isWarring2)
            {
                StartCoroutine(FadeOut(Warring2, Menual,false));
            }
        }
    }

    private void BtnWarringOKClick()
    {
        SoundManager.Instance.PlayUI("버튼 클릭 소리");
        StartCoroutine(FadeOut(Warring1, Warring2, false));
    }

    private void NextDocument()
    {
        SoundManager.Instance.PlayUI("버튼 클릭 소리");

        StartCoroutine(FadeOut(bgDocument, bgStartGame, true));
    }

    private IEnumerator FadeOut(GameObject obj, GameObject fideInObj, bool isStartGame)
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

        StartCoroutine(FadeIn(fideInObj, isStartGame));
    }

    public IEnumerator FadeIn(GameObject obj, bool isStartGame)
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

        if (isStartGame)
        {
            StartCoroutine(NextScene());
        }
    }

    IEnumerator NextScene()
    {
        foreach (TMP_Text text in text_Notions)
        {
            StartCoroutine(TextFadeIn(text));

            yield return new WaitForSeconds(1.5f);
        }

        audioSource.Play();
        yield return new WaitForSeconds(StartDelayTime);

        GameStart();
    }

    IEnumerator TextFadeIn(TMP_Text text)
    {
        Color textColor = new Color(text.color.r, text.color.b, text.color.g, 1.0f);
        float elapsed = 0f;
        float startAlpha = text.color.a;
        float textfadeDruration = 1.0f; // 텍스트 페이드 인 지속 시간 설정

        while (elapsed < textfadeDruration)
        {
            elapsed += Time.deltaTime;
            textColor.a = Mathf.Lerp(startAlpha, 1f, elapsed / textfadeDruration);
            text.color = textColor;
            yield return null;
        }

        textColor.a = 1f;
        text.color = textColor;
    }

    private void GameStart()
    {
        GameManager.Instance.LoadNextScene("2.Tutorial");
    }
}
