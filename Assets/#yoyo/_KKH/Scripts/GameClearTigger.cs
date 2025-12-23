using UnityEngine;

public class GameClearTigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instace.isGameClear = true;
            // 게임 클리어 처리 로직 추가
            Debug.Log("게임 클리어!");

            GameManager.instace.LoadScene("GameOver");
        }
    }
}
