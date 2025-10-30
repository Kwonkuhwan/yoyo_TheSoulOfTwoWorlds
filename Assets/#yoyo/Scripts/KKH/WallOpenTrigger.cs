using System.Collections.Generic;
using UnityEngine;

public class WallOpenTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<GameObject> doors;
    [SerializeField] private bool isOpen = false;
    public bool isBookOpen = false; // Flag to check if the book is open

    private void Awake()
    {
        //audioSource = GetComponent<AudioSource>();

        if (audioSource)
        {
            audioSource.loop = false;
        }
    }

    private void Update()
    {
        if(!audioSource.isPlaying && !isOpen && isBookOpen)
        {
            isOpen = true;
            OpenWall(); // Call the method to open the wall
        }
    }

    private void Start()
    {
        if (doors != null && doors.Count > 0)
        {
            foreach (GameObject door in doors)
            {
                if (door != null)
                {
                    Animator ani = door.GetComponent<Animator>();
                    if (ani == null)
                    {
                        Debug.Log("null");
                    }
                    ani?.SetTrigger("Close"); // Trigger the open animation on the door
                }
                else
                {
                    Debug.LogWarning("One of the door GameObjects is null.");
                }
            }
        }
    }

    private void OpenWall()
    {
        if (doors != null && doors.Count > 0)
        {
            foreach(GameObject door in doors)
            {
                if (door != null)
                {
                    door.GetComponent<Animator>()?.SetTrigger("Open"); // Trigger the open animation on the door
                    SoundManager.Instance.Play3DSound("DoorOpen", door.transform.position);
                }
                else
                {
                    Debug.LogWarning("One of the door GameObjects is null.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Wall GameObject is not assigned or is null.");
        }
    }
}
