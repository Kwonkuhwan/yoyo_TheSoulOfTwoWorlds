using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG 
{
    /// <summary>
    /// 조이스틱(레버) 타입의 물리 입력을 처리하기 위한 헬퍼 클래스
    /// </summary>
    public class MovementStick : MonoBehaviour
    {
        [Header("Deadzone")]
        [Tooltip("이 값보다 작은 입력은 무시합니다.")]
        public float DeadZone = 0.001f;

        /// <summary>
        /// 레버가 회전할 수 있는 최소 각도
        /// </summary>
        public float MinDegrees = -45f;

        /// <summary>
        /// 레버가 회전할 수 있는 최대 각도
        /// </summary>
        public float MaxDegrees = 45f;

        /// <summary>
        /// 현재 조이스틱 X축 (좌/우) 퍼센트 값
        /// </summary>
        public float LeverPercentageX = 0;

        /// <summary>
        /// 현재 조이스틱 Y축 (앞/뒤) 퍼센트 값
        /// </summary>
        public float LeverPercentageY = 0;

        /// <summary>
        /// 조이스틱 입력 벡터 (X,Y)
        /// </summary>
        public Vector2 LeverVector;

        /// <summary>
        /// 조이스틱을 부드럽게 회전시킬지 여부
        /// </summary>
        public bool UseSmoothLook = true;

        /// <summary>
        /// 부드러운 회전 속도
        /// </summary>
        public float SmoothLookSpeed = 15f;

        /// <summary>
        /// 조이스틱이 잡히지 않았을 때 리지드바디를 Kinematic으로 유지할지 여부
        /// (물리와의 상호작용을 막거나 움직이는 플랫폼 위에서 사용 시 유용)
        /// </summary>
        public bool KinematicWhileInactive = false;

        /// <summary>
        /// 조이스틱 값이 변할 때 호출되는 이벤트 (X,Y 퍼센트)
        /// </summary>
        public FloatFloatEvent onJoystickChange;

        /// <summary>
        /// 조이스틱 벡터 값이 변할 때 호출되는 이벤트 (Vector2)
        /// </summary>
        public Vector2Event onJoystickVectorChange;

        // 참조
        Grabbable grab;
        Rigidbody rb;

        // 조이스틱 회전 상태 추적
        Vector3 currentRotation;
        public float angleX;
        public float angleY;

        [SerializeField] GameObject player;
        float moveSpeed;
        float rotateSpeed;

        [SerializeField] bool bMove = false;

        void Start()
        {
            grab = GetComponent<Grabbable>();
            rb = GetComponent<Rigidbody>();

            moveSpeed = 2.0f;
            rotateSpeed = 45.0f;
        }

        void Update()
        {
            // 잡히지 않았을 때 Kinematic 여부 업데이트
            if (rb)
            {
                rb.isKinematic = KinematicWhileInactive && !grab.BeingHeld;
            }

            // 조이스틱 회전 처리
            doJoystickLook();

            // 레버 위치 고정 (로컬 위치, Z축 회전 고정)
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);

            // 현재 회전 값 계산
            currentRotation = transform.localEulerAngles;
            angleX = Mathf.Floor(currentRotation.x);
            angleX = (angleX > 180) ? angleX - 360 : angleX;

            angleY = Mathf.Floor(currentRotation.y);
            angleY = (angleY > 180) ? angleY - 360 : angleY;

            // X축 각도 제한
            if (angleX > MaxDegrees)
            {
                transform.localEulerAngles = new Vector3(MaxDegrees, currentRotation.y, currentRotation.z);
            }
            else if (angleX < MinDegrees)
            {
                transform.localEulerAngles = new Vector3(MinDegrees, currentRotation.y, currentRotation.z);
            }

            // Y축 각도 제한
            if (angleY > MaxDegrees)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentRotation.y, MaxDegrees);
            }
            else if (angleY < MinDegrees)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentRotation.y, MinDegrees);
            }

            // 퍼센트 계산 (각도를 Min~Max 범위 안에서 0~100%로 변환)
            LeverPercentageX = (angleY - MinDegrees) / (MaxDegrees - MinDegrees) * 100;
            LeverPercentageY = (angleX - MinDegrees) / (MaxDegrees - MinDegrees) * 100;

            // 퍼센트 이벤트 호출
            OnJoystickChange(LeverPercentageX, LeverPercentageY);

            // -1 ~ 1 범위로 변환
            float xInput = Mathf.Lerp(-1f, 1f, LeverPercentageX / 100);
            float yInput = Mathf.Lerp(-1f, 1f, LeverPercentageY / 100);

            // 데드존 처리
            if (DeadZone > 0)
            {
                if (Mathf.Abs(xInput) < DeadZone)
                {
                    xInput = 0;
                }
                if (Mathf.Abs(yInput) < DeadZone)
                {
                    yInput = 0;
                }
            }

            Debug.Log($"inputX : {xInput} / inputY : {yInput}");

            // 최종 입력 벡터
            LeverVector = new Vector2(xInput, yInput);

            if (bMove)
            {
                player.transform.position += player.transform.forward * yInput * moveSpeed * Time.deltaTime;
            }
            else
            {
                player.transform.Rotate(0f, xInput * rotateSpeed * Time.deltaTime, 0f);
            }

            // 벡터 이벤트 호출
            //OnJoystickChange(LeverVector);
        }

        void FixedUpdate()
        {
            // 물리 속도 초기화
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// 조이스틱이 잡혔을 때, 잡은 위치를 기준으로 회전 처리
        /// </summary>
        void doJoystickLook()
        {
            // 조이스틱이 잡히고 있을 때
            if (grab != null && grab.BeingHeld)
            {
                rb.isKinematic = false; // 잡으면 물리 켜줌
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // 기존 회전값 저장
                Quaternion originalRot = transform.rotation;

                // 그랩 위치를 기준으로 목표 회전값 계산
                Vector3 localTargetPosition = transform.InverseTransformPoint(grab.GetPrimaryGrabber().transform.position);
                Vector3 targetPosition = transform.TransformPoint(localTargetPosition);

                // 목표 방향을 바라보도록 설정
                transform.LookAt(targetPosition, transform.up);

                // 잡고 있을 때만 부드러운 회전 적용
                if (UseSmoothLook)
                {
                    Quaternion newRot = transform.rotation;
                    transform.rotation = originalRot;
                    transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * SmoothLookSpeed);
                }
            }
            // 잡히지 않았을 때 → 즉시 원위치
            else if (grab != null && !grab.BeingHeld)
            {
                rb.isKinematic = true; // 놓으면 물리 끔
                transform.localRotation = Quaternion.identity; // 즉시 원위치
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            //// 조이스틱이 잡히고 있을 때
            //if (grab != null && grab.BeingHeld)
            //{
            //    rb.linearVelocity = Vector3.zero;
            //    rb.angularVelocity = Vector3.zero;

            //    // 기존 회전값 저장
            //    Quaternion originalRot = transform.rotation;

            //    // 그랩 위치를 기준으로 목표 회전값 계산
            //    Vector3 localTargetPosition = transform.InverseTransformPoint(grab.GetPrimaryGrabber().transform.position);
            //    Vector3 targetPosition = transform.TransformPoint(localTargetPosition);

            //    // 목표 방향을 바라보도록 설정
            //    transform.LookAt(targetPosition, transform.up);

            //    // 부드러운 회전 적용
            //    if (UseSmoothLook)
            //    {
            //        Quaternion newRot = transform.rotation;
            //        transform.rotation = originalRot;
            //        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.fixedDeltaTime * SmoothLookSpeed);
            //    }
            //}
            //// 잡히지 않았고 Kinematic 상태일 때 → 원래 위치로 되돌아감
            //else if (grab != null && !grab.BeingHeld && rb.isKinematic)
            //{
            //    //transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * SmoothLookSpeed);
            //    transform.localRotation = Quaternion.identity;
            //    rb.linearVelocity = Vector3.zero;
            //    rb.angularVelocity = Vector3.zero;
            //}
        }

        /// <summary>
        /// 퍼센트 값 변경 시 호출되는 콜백
        /// </summary>
        public virtual void OnJoystickChange(float leverX, float leverY)
        {
            if (onJoystickChange != null)
            {
                onJoystickChange.Invoke(leverX, leverY);
            }
        }

        /// <summary>
        /// 벡터 값 변경 시 호출되는 콜백
        /// </summary>
        public virtual void OnJoystickChange(Vector2 joystickVector)
        {
            if (onJoystickVectorChange != null)
            {
                onJoystickVectorChange.Invoke(joystickVector);
            }
        }
    }
}

