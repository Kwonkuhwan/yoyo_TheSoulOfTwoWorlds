using System.Collections;
using UnityEngine;

public class DoorJumpSquare : MonoBehaviour
{
    public Animator animator;
    public Transform trDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("Close");
            SoundManager.Instance.Play3DSound("DoorClose", trDoor.position);
        }

        StartCoroutine(Open());
    }

    IEnumerator Open()
    {
        yield return new WaitForSeconds(3.0f);

        animator.SetTrigger("Open");
        SoundManager.Instance.Play3DSound("DoorOpen", trDoor.position);

        yield return new WaitForSeconds(2.0f);

        gameObject.SetActive(false);
    }
}
