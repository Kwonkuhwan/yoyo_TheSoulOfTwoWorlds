using UnityEngine;

public class FlyCam : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float fastSpeed = 50f;
    public float mouseSensitivity = 3f;
    public bool lockCursor = true;

    float yaw;
    float pitch;

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        // 마우스 회전
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -89f, 89f);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // 이동 속도
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : moveSpeed;

        // 이동 입력
        Vector3 move = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        );

        // Q/E로 수직 이동
        if (Input.GetKey(KeyCode.E)) move.y += 1;
        if (Input.GetKey(KeyCode.Q)) move.y -= 1;

        transform.Translate(move * currentSpeed * Time.deltaTime, Space.Self);

        // Esc로 마우스 커서 해제
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
