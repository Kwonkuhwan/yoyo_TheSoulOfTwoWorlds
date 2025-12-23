using UnityEngine;
public enum AppList
{
    Gallery,
    Camera,
    Messenger
}

public class PhoneController : MonoBehaviour
{
    [Header("앨범")]
    [SerializeField] GameObject app_album;

    [Header("카메라")]
    [SerializeField] GameObject app_camera;

    [Header("메신저")]
    [SerializeField] GameObject app_messenger;

    GameObject currentApp;

    public void OpenApp(AppList app)
    {
        currentApp.SetActive(false);
        if (app == AppList.Gallery)
        {
            currentApp = app_album;
        }
        else if (app == AppList.Camera)
        {
            currentApp = app_camera;
        }
        else if (app == AppList.Messenger)
        {
            currentApp = app_messenger;
        }
        currentApp.SetActive(true);
    }
}
