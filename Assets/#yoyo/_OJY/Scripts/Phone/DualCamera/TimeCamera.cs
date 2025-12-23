using Oculus.Interaction.Unity.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class TimeCamera : MonoBehaviour
{
    MapManager mapManager;

    GameObject cameraObject;
    Camera cam;

    Vector3 cameraPos;
    Quaternion cameraRot;

    [Header("카메라 렌더 텍스쳐")]
    public RenderTexture originRt;
    RenderTexture rt;

    [Header("사진이 들어갈 UI")]
    public Image image; //사진첩 UI

    int width; 
    int height;

    private void Awake()
    {
        mapManager = FindAnyObjectByType<MapManager>();
        //cameraObject = mapManager.GetSubCamera();
        cam = cameraObject.GetComponent<Camera>();

        Init();
    }

    private void Update()
    {
        cameraPos = transform.position;
        cameraRot = transform.rotation;

        cameraObject.transform.localPosition = cameraPos;
        cameraObject.transform.localRotation = cameraRot;

        if (Input.GetKeyDown(KeyCode.C))
        {
            Capture();
        }
    }

    void Init()
    {
        width = Screen.width;
        height = Screen.height;
    }

    void Capture()
    {
        // 2. 임시 RenderTexture 생성
        // 카메라 출력(Render) 결과를 이 RenderTexture에 저장
        // depth = 24 → 깊이 버퍼 사용 (필요시)
        rt = new RenderTexture(width, height, 24);

        // 3. 카메라의 출력 대상(Target Texture)을 방금 만든 RenderTexture로 설정
        // 이렇게 하면 cam.Render() 호출 시 화면이 아닌 rt에 렌더링됨
        cam.targetTexture = rt;

        // 4. 캡쳐용 Texture2D 생성
        // RenderTexture의 픽셀 데이터를 읽어서 Texture2D로 변환할 때 사용
        // TextureFormat.RGB24 → 알파 채널 없는 RGB
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);

        // 5. 카메라 렌더링
        // cam.Render() 호출 시 targetTexture로 화면을 그림
        cam.Render();

        // 6. RenderTexture를 활성화
        // Texture2D.ReadPixels는 현재 활성화된 RenderTexture에서 픽셀을 읽음
        RenderTexture.active = rt;

        // 7. RenderTexture 픽셀 데이터를 Texture2D로 읽어오기
        // Rect(0,0,width,height) → 전체 영역
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        // 8. Texture2D 적용
        // ReadPixels 후 반드시 Apply() 호출해야 픽셀 데이터가 GPU/Texture에 반영됨
        screenShot.Apply();

        // 9. UI에 캡쳐 화면 표시
        // Texture2D → Sprite 변환 후 Image 컴포넌트에 넣기
        image.sprite = Sprite.Create(
            screenShot,
            new Rect(0, 0, screenShot.width, screenShot.height),
            new Vector2(0.5f, 0.5f) // Pivot 중앙
        );

        // 10. 정리
        // 기존 카메라 렌더 텍스쳐로 변환
        cam.targetTexture = originRt;

        // 활성화된 RenderTexture 해제
        RenderTexture.active = null;

        // 임시 RenderTexture 메모리 해제
        Destroy(rt);        
    }
}
