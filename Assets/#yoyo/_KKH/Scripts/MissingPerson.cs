using UnityEngine;

public class MissingPerson : MonoBehaviour
{
    [SerializeField] private Animation ani;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ani)
        {
            ani.Play();
        }
    }
}
