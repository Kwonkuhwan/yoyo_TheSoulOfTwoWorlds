using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private bool isFpsEnable = true;
    [SerializeField] private TMP_Text text_FPS;

    private float ffps = 0.0f;
    public float fFps
    {
        get { return ffps; }
        set
        {
            ffps = value;
            if (text_FPS != null && isFpsEnable)
            {
                text_FPS.text = "FPS: " + ffps.ToString("F2");
            }
        }
    }

    private void Awake()
    {
        if (isFpsEnable)
        {
            text_FPS.gameObject.SetActive(true);
        }
        else
        {
            text_FPS.gameObject.SetActive(false);
        }
    }
}
