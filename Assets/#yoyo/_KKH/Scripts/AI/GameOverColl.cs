using UnityEngine;

public class GameOverColl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instace.GameOver();
        }
    }
}
