using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private bool isEnableWindwos = false; // Set to false for mobile platforms
    [SerializeField] private bool isWindwos = false; // Set to false for mobile platforms
    public Transform cameraTransform;
    public float distance = 2f;
    public float up = 1.3f;

    private void Awake()
    {
        if (isEnableWindwos)
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_WSA_10_0
            isWindwos = true;
#else
            isWindwos = false;
#endif
        }
    }
    private void Start()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_WSA_10_0
        transform.position = cameraTransform.position + cameraTransform.forward * distance + cameraTransform.up * up;
        //transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
#endif
    }

    void LateUpdate()
    {
        if(!isWindwos)
        {
            transform.position = cameraTransform.position + cameraTransform.forward * distance;
            transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
        }
    }
}
