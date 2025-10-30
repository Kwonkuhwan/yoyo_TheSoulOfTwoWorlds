using UnityEngine;

public class WallTiggerObj : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject Wall;

    [SerializeField] private BoxCollider tigger;


    private void Awake()
    {
        tigger = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        door.GetComponent<Animator>().SetTrigger("Close");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.GetComponent<Animator>().SetTrigger("Open");
            tigger.enabled = false;
        }
    }
}
