using UnityEngine;

public class WallGhost : MonoBehaviour
{
    [SerializeField] private Animation ani;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ani.Play();
            SoundManager.Instance.Play3DSound("¥‹√º øÙ¿Ω", ani.transform);
        }
    }
}
