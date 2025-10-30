using UnityEngine;

public class DoorTiggerObj : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject Wall;

    [SerializeField] private BoxCollider tigger;


    private void Awake()
    {
        tigger = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Wall.GetComponent<Animator>().SetTrigger("Open");
            SoundManager.Instance.Play3DSound("벽 움직이는 소리", Wall.transform);
            tigger.enabled = false;
        }
    }
}
