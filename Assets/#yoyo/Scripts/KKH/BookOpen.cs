using UnityEngine;

public class BookOpen : MonoBehaviour
{
    [SerializeField] private BoxCollider bookTigger;

    private void Awake()
    {
        bookTigger = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) // Check if the collider belongs to the player
        {
            bookTigger.enabled = false; // Disable the trigger collider to prevent multiple triggers
            other.GetComponent<PlayerManager>().OpenBook(); // Call the OpenBook method on the PlayerManager component
        }
    }
}
