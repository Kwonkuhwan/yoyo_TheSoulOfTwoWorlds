using System.Collections;
using UnityEngine;

public class StartUI : MonoBehaviour
{
    [SerializeField] private float StartDelayTime = 10.0f;

    

    void Start()
    {
        StartCoroutine(NextScene());
    }

    private void Update()
    {
    }

    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(StartDelayTime);

        GameStart();
    }

    private void GameStart()
    {
        GameManager.Instance.LoadNextScene("2.Tutorial");
    }
}
