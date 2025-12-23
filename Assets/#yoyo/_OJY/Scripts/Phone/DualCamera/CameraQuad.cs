using UnityEngine;
using System.IO;

public class CameraQuad : MonoBehaviour
{
    Renderer quadRenderer;

    private void Awake()
    {
        transform.localScale = new Vector3(1f, 0.5625f, 1.0f);
    }
}
