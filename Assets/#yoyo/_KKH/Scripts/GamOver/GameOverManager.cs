using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject go_GameOver;
    [SerializeField] private GameObject go_GameClear;

    [SerializeField] private Sprite[] sprite_Notions;
    [SerializeField] private Image img_notion;

    [SerializeField] private Sprite[] sprite_Count;
    [SerializeField] private Image img_count;


    private void Awake()
    {
        if(!GameManager.instace.isGameClear)
        {
            go_GameOver.SetActive(true);
            go_GameClear.SetActive(false);
        }
        else
        {
            go_GameOver.SetActive(false);
            go_GameClear.SetActive(true);
        }
    }

    private void Start()
    {
        if (!GameManager.instace.isGameClear)
        {
            int index = Random.Range(0, sprite_Notions.Length);
            img_notion.sprite = sprite_Notions[index];
            img_notion.SetNativeSize();
        }

        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        foreach (Sprite sprite in sprite_Count)
        {
            img_count.sprite = sprite;
            yield return new WaitForSeconds(1f);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMap");
    }
}
