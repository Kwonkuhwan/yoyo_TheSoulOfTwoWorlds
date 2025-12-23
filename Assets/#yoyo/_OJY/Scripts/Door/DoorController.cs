using UnityEngine;
using BNG;

public class DoorController : MonoBehaviour
{
    Grabbable grabbable;
    Animator doorAnimator;
    bool isOpen = false;

    private void Awake()
    {
        grabbable = GetComponent<Grabbable>();
        doorAnimator = GetComponent<Animator>();
    }

    public void DoorInteraction()
    {
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    /// <summary>
    /// 문열기 
    /// </summary>
    void OpenDoor()
    {
        if (doorAnimator != null)
        {
            if (SoundManager.I != null)
            {
                SoundManager.I.PlaySFXAt("DoorOpen", transform.position);
            }
            doorAnimator.SetTrigger("DoorOpen");
            isOpen = true;
        }
    }

    /// <summary>
    /// 문 닫기
    /// </summary>
    void CloseDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("DoorClose");
            isOpen = false;
        }
    }
}