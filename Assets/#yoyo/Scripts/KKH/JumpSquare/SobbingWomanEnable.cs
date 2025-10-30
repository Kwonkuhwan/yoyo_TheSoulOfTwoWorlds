using UnityEngine;

public class SobbingWomanEnable : MonoBehaviour
{
    [SerializeField] private GameObject SobbingWomanObj;

    private void OnTriggerEnter(Collider other)
    {
        SobbingWomanObj.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        SobbingWomanObj.SetActive(false);
    }
}
