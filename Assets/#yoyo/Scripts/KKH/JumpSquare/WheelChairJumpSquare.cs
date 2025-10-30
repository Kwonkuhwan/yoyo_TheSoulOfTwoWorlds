using UnityEngine;

public class WheelChairJumpSquare : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider; // BoxCollider for the jump square
    [SerializeField] private Animation ani;
    [SerializeField] private Transform trObj;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the colliding object is tagged as "Player"
        {
            ani.Play();
            boxCollider.enabled = false; // Disable the collider to prevent repeated triggers
            SoundManager.Instance.Play3DSound("WheelChair", trObj.position); // Play jump sound effect
            SoundManager.Instance.Play3DSound("Laughing", trObj.position); // Play jump sound effect
        }
    }
}
