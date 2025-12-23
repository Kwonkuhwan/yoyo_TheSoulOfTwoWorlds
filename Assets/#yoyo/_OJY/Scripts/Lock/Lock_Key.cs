using UnityEngine;

public class Lock_Key : MonoBehaviour
{
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        Debug.Log("¹® ¿­±â");
        anim.SetTrigger("Open");

        Destroy(gameObject, 2.0f);
    }
}
