using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KeyLock"))
        {
            Debug.Log("Key »ðÀÔ");
            other.GetComponent<Lock_Key>().OpenDoor();
            Destroy(gameObject);
        }
    }
}
