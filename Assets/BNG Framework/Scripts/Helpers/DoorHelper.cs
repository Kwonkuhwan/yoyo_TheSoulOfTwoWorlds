using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG
{
    // 문(Door)의 개폐, 잠금/잠금 해제, 사운드 재생 등을 관리하는 헬퍼 스크립트입니다.
    public class DoorHelper : MonoBehaviour
    {

        public AudioClip DoorOpenSound; // 문이 열릴 때 재생할 오디오 클립
        public AudioClip DoorCloseSound; // 문이 닫힐 때 재생할 오디오 클립

        /// <summary>
        /// 닫힌 상태에서 문을 열기 위해 손잡이를 돌려야 하는지 여부입니다.
        /// </summary>
        [Tooltip("닫힌 위치에서 문을 열기 위해 손잡이를 돌려야 하는지 여부입니다.")]
        public bool RequireHandleTurnToOpen = false;

        /// <summary>
        /// RequireHandleTurnToOpen이 true이고 손잡이가 완전히 열리지 않은 경우,
        /// 문은 키네마틱(kinematic)/움직일 수 없는 상태가 됩니다.
        /// </summary>
        bool handleLocked = false; // 손잡이 잠금 상태 (문짝의 움직임을 제한)

        /// <summary>
        /// 이 Transform은 손잡이가 몇 도 회전했는지 측정하는 데 사용됩니다. 
        /// RequireHandleTurnToOpen이 true인 경우 필수입니다.
        /// </summary>
        public Transform HandleFollower;

        public float DegreesTurned; // 손잡이가 돌아간 현재 각도

        /// <summary>
        /// 래치(Latch)가 열리려면 손잡이가 몇 도 회전해야 하는지 지정합니다.
        /// </summary>
        public float DegreesTurnToOpen = 10f;

        /// <summary>
        /// 손잡이 회전에 따라 이 Transform(일반적으로 잠금쇠)을 회전시킵니다.
        /// </summary>
        public Transform DoorLockTransform;
        float initialLockPosition; // 잠금쇠 Transform의 초기 X축 Local Position

        HingeJoint hinge; // 문에 연결된 HingeJoint 컴포넌트
        Rigidbody rigid; // 문의 Rigidbody 컴포넌트
        bool playedOpenSound = false; // 열림 소리 재생 여부

        // 닫힘 소리를 재생하기 전에 문이 어느 정도 열려야 할지 결정합니다.
        bool readyToPlayCloseSound = false; // 닫힘 소리를 재생할 준비가 되었는지 여부

        public float AngularVelocitySnapDoor = 0.2f; // 이 각속도 이하로 움직일 때 문을 닫힌 위치로 스냅합니다.

        public float angle; // 현재 문이 열린 각도 (0에 가까울수록 닫힘)
        public float AngularVelocity = 0.2f; // 문의 현재 각속도

        [Tooltip("True이면 문이 사용자 입력에 반응하지 않습니다 (항상 잠금 상태)")]
        public bool DoorIsLocked = false; // 영구적인 문 잠금 상태

        public float lockPos; // 잠금쇠의 계산된 현재 위치

        // public string DebugText; // 디버그 텍스트 (주석 처리됨)

        // 가비지 컬렉션(GC)을 줄이기 위한 캐시 변수
        Vector3 currentRotation;
        float moveLockAmount, rotateAngles, ratio;

        void Start()
        {
            // 필요한 컴포넌트 가져오기
            hinge = GetComponent<HingeJoint>();
            rigid = GetComponent<Rigidbody>();

            // 잠금쇠 Transform이 있다면 초기 위치를 저장합니다.
            if (DoorLockTransform)
            {
                initialLockPosition = DoorLockTransform.transform.localPosition.x;
            }
        }

        void Update()
        {

            // 문이 닫힐 때 스냅하는 데 사용되는 각속도를 읽습니다.
            AngularVelocity = rigid.angularVelocity.magnitude;

            // DebugText = rigid.angularVelocity.x + ", " + rigid.angularVelocity.y + ", " + rigid.angularVelocity.z; // 디버그 용

            // 힌지 각도 계산 (문의 열림 정도)
            currentRotation = transform.localEulerAngles;
            angle = Mathf.Floor(currentRotation.y); // Y축 회전 각도를 가져옵니다.

            // 힌지 각도를 0 (완전히 닫힘)에서 180 (완전히 열림) 사이의 값으로 조정합니다.
            if (angle >= 180)
            {
                angle -= 180;  // 180~360 범위의 각도를 0~180으로 조정
            }
            else
            {
                angle = 180 - angle; // 0~180 범위의 각도를 180~0으로 조정
            }

            // 문 열림 사운드 재생
            if (angle > 10)
            { // 문이 10도 이상 열렸을 때
                if (!playedOpenSound)
                {
                    // VRUtils.Instance를 사용하여 공간에 사운드 클립 재생
                    // VRUtils는 별도의 스크립트에서 제공하는 것으로 보입니다.
                    VRUtils.Instance.PlaySpatialClipAt(DoorOpenSound, transform.position, 1f, 1f);
                    playedOpenSound = true;
                }
            }

            // 문이 30도 이상 열렸다면 닫힘 소리 재생 준비 완료
            if (angle > 30)
            {
                readyToPlayCloseSound = true;
            }

            // 열림 사운드 재생 플래그 초기화 (문이 거의 닫혔을 때)
            if (angle < 2 && playedOpenSound)
            {
                playedOpenSound = false;
            }

            // 문을 닫힌 위치로 스냅해야 하는지 확인합니다. (각도 1도 미만, 각속도가 낮을 때)
            if (angle < 1 && AngularVelocity <= AngularVelocitySnapDoor)
            {
                // 문이 무한히 흔들리는 것을 방지하기 위해 각속도를 0으로 초기화합니다.
                if (!rigid.isKinematic)
                {
                    rigid.angularVelocity = Vector3.zero;
                }
            }

            // 닫힘 사운드 재생
            if (readyToPlayCloseSound && angle < 2)
            { // 문이 열렸다가 (readyToPlayCloseSound=true) 다시 2도 미만으로 닫힐 때
                VRUtils.Instance.PlaySpatialClipAt(DoorCloseSound, transform.position, 1f, 1f);
                readyToPlayCloseSound = false;
            }

            // 손잡이가 있다면 회전 각도를 계산합니다.
            if (HandleFollower)
            {
                // 손잡이의 로컬 Y축 회전 각도를 기반으로 DegreesTurned를 계산합니다. (270도를 기준으로 회전량을 측정)
                DegreesTurned = Mathf.Abs(HandleFollower.localEulerAngles.y - 270);
            }

            // 잠금쇠 Transform 위치 업데이트
            if (DoorLockTransform)
            {
                // 잠금쇠 이동량 및 회전 각도 정의
                moveLockAmount = 0.025f;
                rotateAngles = 55;
                // 회전된 각도(DegreesTurned)를 기준으로 잠금쇠 이동 비율 계산
                ratio = rotateAngles / (rotateAngles - Mathf.Clamp(DegreesTurned, 0, rotateAngles));
                // 잠금쇠의 목표 위치 계산
                lockPos = initialLockPosition - (ratio * moveLockAmount) + moveLockAmount;
                // 계산된 위치를 초기 위치와 최대 이동량 사이로 클램프
                lockPos = Mathf.Clamp(lockPos, initialLockPosition - moveLockAmount, initialLockPosition);

                // 잠금쇠 Transform의 로컬 위치를 설정
                DoorLockTransform.transform.localPosition = new Vector3(lockPos, DoorLockTransform.transform.localPosition.y, DoorLockTransform.transform.localPosition.z);
            }

            // 잠금 상태 설정 (손잡이 회전 요구 사항에 따라)
            if (RequireHandleTurnToOpen)
            {
                // 손잡이가 DegreesTurnToOpen보다 적게 돌아갔으면 잠금 상태입니다.
                handleLocked = DegreesTurned < DegreesTurnToOpen;
            }

            // 문이 닫혔고 (angle < 0.02f) 잠겨 있어야 하는 경우 (손잡이 잠금 또는 영구 잠금)
            if (angle < 0.02f && (handleLocked || DoorIsLocked))
            {
                // 충돌 감지 모드를 변경하여 성능을 최적화합니다. (kinematic 상태로 전환하기 전에)
                if (rigid.collisionDetectionMode == CollisionDetectionMode.Continuous || rigid.collisionDetectionMode == CollisionDetectionMode.ContinuousDynamic)
                {
                    rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                }

                // 문을 키네마틱(Kinematic, 외부 물리력에 반응하지 않음)으로 설정하여 잠급니다.
                rigid.isKinematic = true;
            }
            else
            {
                // 문이 잠겨 있지 않다면
                // 충돌 감지 모드를 다시 설정합니다.
                if (rigid.collisionDetectionMode == CollisionDetectionMode.ContinuousSpeculative)
                {
                    rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                }

                // 문 잠금을 해제하여 물리력에 반응하게 합니다.
                rigid.isKinematic = false;
            }
        }
    }
}
