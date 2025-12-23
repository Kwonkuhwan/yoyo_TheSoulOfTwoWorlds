using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instace { get; private set; }

    [Header("ID카드 획득여부")]
    [SerializeField] bool[] hasCards = new bool[5];

    int cardCount = 0;

    [Header("ID카드 획득 텍스트")]
    [SerializeField] TextMeshProUGUI infoText;

    public bool isGameClear = false;

    public GameObject GameClearObj;


    void Awake()
    {
        if (instace != null && instace != this)
        {
            Destroy(gameObject);
            return;
        }

        instace = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 세팅 초기화
    /// </summary>
    public void Init()
    {
        for (int i = 0; i < hasCards.Length; i++)
        {
            hasCards[i] = false;
        }

        cardCount = 0;
        isGameClear = false;

        GameClearObj = GameObject.Find("GameClearTigger");
        GameClearObj.SetActive(false);
    }

    private void Start()
    {
        Init();
    }

    public void HasIdCard(string name, int index)
    {
        cardCount++;
        hasCards[index] = true;
        StartCoroutine(GetCardCoroutine(name, index));
    }

    IEnumerator GetCardCoroutine(string name, int index)
    {
        infoText.text = $"{name} ID Card 획득 하셨습니다.\n{5 - cardCount} 남았습니다.";
        yield return new WaitForSeconds(2.0f);
        infoText.text = "";

        if(cardCount == hasCards.Length)
        {
            GameClearObj.SetActive(true);
        }
    }

    public void GameOver()
    {
        StartCoroutine(IEGameOver());
    }

    private IEnumerator IEGameOver()
    {
        PlayerManager.instance.Die();
        yield return new WaitForSeconds(3.0f);

        LoadScene("GameOver");
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(IELoadScene(sceneName));
    }

    private IEnumerator IELoadScene(string sceneName)
    {
        OVRScreenFade.instance.FadeOut();
        yield return new WaitForSeconds(1.0f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}