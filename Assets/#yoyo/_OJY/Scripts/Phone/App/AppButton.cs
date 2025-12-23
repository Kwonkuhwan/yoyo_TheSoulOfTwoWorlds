using UnityEngine;
using UnityEngine.UI;

public class AppButton : MonoBehaviour
{
    [SerializeField]
    AppList targetApp;

    PhoneController phoneController;

    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ClickEvent);

        phoneController = FindAnyObjectByType<PhoneController>();
    }

    void ClickEvent()
    {
        if (phoneController != null)
        {
            phoneController.OpenApp(targetApp);
        }
    }
}
