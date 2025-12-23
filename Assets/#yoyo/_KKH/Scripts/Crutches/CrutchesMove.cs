//using BNG;
//using UnityEngine;

//public class CrutchesMove : MonoBehaviour
//{
//    public BNGPlayerController playerController;
//    public bool isPlayerGrab = false;

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!isPlayerGrab) return;

//        if (other.CompareTag("Ground"))
//        {
//            Debug.Log("Crutches touched the ground");
//        }
//    }
//}


using BNG;
using System.Media;
using UnityEngine;


public class CrutchesMove : MonoBehaviour
{
    [SerializeField] private SoundPlay soundPlay; // Reference to the SoundPlay component

    [SerializeField] private Grabbable grabbable; // Reference to the Grabbable component

    enum MoveType { None = 0, Rigidbody, CharacterController, Transform }
    [SerializeField] private MoveType moveType = MoveType.None;

    public enum AllowedHand { Any = 0, Left, Right }
    [Header("Hand Filtering")]
    [SerializeField] private AllowedHand allowedHand = AllowedHand.Right;

    [Header("References")]
    [Tooltip("Player rig root used for movement (usually the XR Origin / VRIF Player root)")]
    public Transform playerRig;

    [Tooltip("Player Body Rigidbody (if your rig uses physics). Leave null if using CharacterController-only movement.")]
    public Rigidbody playerBody;


    [Tooltip("CharacterController on the rig (if you move via CharacterController). Leave null if using Rigidbody.")]
    public CharacterController characterController;


    [Tooltip("The transform of the hand/controller that grabs this crutch handle.")]
    public Transform handTransform;


    [Header("Crutch Geometry")]
    [Tooltip("The crutch tip (a child transform at the very bottom). Its collider should NOT be a trigger.")]
    public Transform tipTransform;


    [Tooltip("LayerMask of surfaces considered ground.")]
    public LayerMask groundMask = ~0;


    [Tooltip("Max angle (deg) from world up to consider the crutch planted (avoid nearly horizontal).")]
    [Range(10f, 85f)] public float maxPlantTilt = 70f;


    [Tooltip("Distance from tip to check for ground contact via SphereCast (meters).")]
    public float plantProbe = 0.03f;


    [Tooltip("Sphere radius for ground probe (meters). Match your tip collider roughly.")]
    public float tipRadius = 0.02f;


    [Header("Tuning")]
    [Tooltip("How strongly hand backward motion turns into forward velocity.")]
    public float pushGain = 2.0f;


    [Tooltip("Upward boost added on release to smooth the hop (m/s). 0 = none.")]
    public float releaseUpBoost = 0.2f;


    [Tooltip("Friction while planted (0 = slide freely, 1 = full stick). Higher = less twitch.")]
    [Range(0f, 1f)] public float plantSmoothing = 0.2f;


    [Tooltip("Minimum hand travel (meters per second) to start pushing.")]
    public float minHandSpeed = 0.05f;


    [Tooltip("Cooldown after unplant (seconds). Prevents re-plant flicker")]
    public float replantCooldown = 0.08f;


    // === Hop settings ===
    [Header("Hop (lift-then-move)")]
    [Tooltip("언플랜트 시 위로 주는 초기 상승 속도(m/s)")]
    public float hopUp = 0.6f;

    [Tooltip("언플랜트 시 전방으로 주는 초기 속도(m/s)")]
    public float hopForward = 1.6f;

    [Tooltip("공중 유지 시간(감쇠 전 가볍게 유지할 시간, 초)")]
    public float hopHoldTime = 0.10f;

    [Tooltip("공중에서 전방 속도 감쇠(초당 비율, 0=안 줄어듦, 2=빨리 줄어듦)")]
    public float airDamp = 1.2f;

    [Tooltip("공중에서의 중력 배율 (1 = 기본 중력)")]
    public float airGravityMultiplier = 1.0f;

    [Tooltip("공중에서 미세한 조향(손 이동 반영) 정도")]
    [Range(0f, 1f)] public float airControl = 0.15f;

    // runtime
    bool airborne;
    float hopTimer;
    Vector3 airVelocity;

    [Header("Plant gating")]
    [Tooltip("plant 상태 + 팁이 거의 정지일 때만 이동")]
    public bool onlyWhilePlanted = true;

    [Tooltip("팁이 정지로 간주되는 최대 이동량(m)")]
    public float tipStillEpsilon = 0.005f;

    [Tooltip("팁이 위로 들리는 것으로 간주되는 최소 상승량(m)")]
    public float tipLiftEpsilon = 0.001f;

    // === 쿨다운 ===
    [Header("Cooldown")]
    [Tooltip("착지 후 다시 plant/이동 가능해질 때까지 대기 시간(초)")]
    public float moveCooldown = 1.0f;
    private float moveEnableUntil = 0f;

    // 내부 상태
    Vector3 lastTipWorld;

    [Header("Debug")]
    public bool isGrabbed = false; // Hook this from your VRIF Grabbable events
    public bool planted;
    public Vector3 lastHandWorld;


    // >>> 내부 상태
    private Grabber activeGrabber;
    float unplantUntil;

    private void Awake()
    {
        isGrabbed = false;

        if (grabbable == null)
            grabbable = GetComponentInParent<Grabbable>();

        if(soundPlay == null)
            soundPlay = GetComponent<SoundPlay>();
    }

    void Reset()
    {
        grabbable = GetComponentInParent<Grabbable>();

        // Try to auto-find typical components
        characterController = FindAnyObjectByType<CharacterController>();
        playerBody = FindAnyObjectByType<Rigidbody>();
    }


    void OnEnable()
    {
        lastHandWorld = handTransform ? handTransform.position : Vector3.zero;
        lastTipWorld = tipTransform ? tipTransform.position : Vector3.zero;
    }


    void Update()
    {
        // 1) Grabber 선택 (허용 손 필터링 포함)
        UpdateActiveGrabber();

        // 2) handTransform / isGrabbed 동기화
        if (activeGrabber != null)
        {
            isGrabbed = true;
            handTransform = activeGrabber.transform;
        }
        else
        {
            isGrabbed = false;
            handTransform = null;
        }

        // 공중 상태면 우선 공중 이동 처리
        if (airborne)
        {
            TickAirborne();
            return;
        }

        // Simple runtime safety
        if (!isGrabbed || handTransform == null || playerRig == null)
        {
            planted = false;
            return;
        }

        // Probe the tip for ground contact & orientation gating
        //bool canPlant = Time.time >= unplantUntil && IsTipGrounded(out RaycastHit hit);
        bool canPlant = Time.time >= unplantUntil
                && Time.time >= moveEnableUntil   // 새로 추가
                && IsTipGrounded(out RaycastHit hit);

        bool goodTilt = IsGoodTilt();

        if (!planted)
        {
            if (canPlant && goodTilt)
            {
                planted = true;
                lastHandWorld = handTransform.position;
            }
        }
        else // planted
        {
            // 팁 이동량/방향 측정
            Vector3 tipNow = tipTransform ? tipTransform.position : lastTipWorld;
            Vector3 tipDelta = tipNow - lastTipWorld;
            lastTipWorld = tipNow;

            bool tipIsStill = tipDelta.magnitude <= tipStillEpsilon;
            bool tipIsLifting = tipDelta.y > tipLiftEpsilon;

            Vector3 handNow = handTransform.position;
            Vector3 handDelta = handNow - lastHandWorld;
            lastHandWorld = handNow;

            // Convert hand motion to rig-space push (backward hand motion -> forward body motion)
            Vector3 rigForward = Vector3.ProjectOnPlane(playerRig.forward, Vector3.up).normalized;
            Vector3 deltaFlat = Vector3.ProjectOnPlane(-handDelta, Vector3.up); // pulling back = -delta

            float speed = deltaFlat.magnitude / Mathf.Max(Time.deltaTime, 1e-4f);
            if (speed > minHandSpeed)
            {
                //// Only keep the component that aligns with rig forward (prevents lateral drift)
                //float f = Vector3.Dot(deltaFlat.normalized, rigForward);
                //Vector3 push = rigForward * Mathf.Max(0f, f) * deltaFlat.magnitude * pushGain / Mathf.Max(Time.deltaTime, 1e-4f);
                //ApplyVelocity(push * (1f - plantSmoothing));

                if ((!onlyWhilePlanted) || (planted && canPlant && tipIsStill && !tipIsLifting))
                {
                    float f = Vector3.Dot(deltaFlat.normalized, rigForward);
                    Vector3 push = rigForward * Mathf.Max(0f, f) * deltaFlat.magnitude * pushGain / Mathf.Max(Time.deltaTime, 1e-4f);
                    ApplyVelocity(push * (1f - plantSmoothing));
                }
            }

            // Unplant conditions: lost ground, too flat, or user lifts tip noticeably
            if (!canPlant || !goodTilt)
            {
                planted = false;
                unplantUntil = Time.time + replantCooldown;
                //if (releaseUpBoost > 0f)
                //    ApplyVelocity(Vector3.up * releaseUpBoost);

                // 기존 위로 톡 업부스트 대신, hop 시스템으로 전환
                StartHop(rigForward);
            }
        }
    }

    void StartHop(Vector3 rigForward)
    {
        airborne = true;
        hopTimer = hopHoldTime;

        // 초기 속도 : 위로 + 전방
        Vector3 v0 = Vector3.up * Mathf.Max(0f, hopUp) + rigForward * Mathf.Max(0f, hopForward);
        airVelocity = v0;

        //소리 출력
        soundPlay.PlaySound();

        // 식재 해제 중엔 즉시 약간 이동감 주기
        ApplyVelocity(airVelocity);
    }

    void TickAirborne()
    {
        float dt = Mathf.Max(Time.deltaTime, 1e-4f);

        // 약한 공중 조향 : 손의 미세 이동을 전방 성분으로 일부 반영 (선택)
        if (isGrabbed && handTransform != null && playerRig != null && airControl > 0f)
        {
            Vector3 rigForward = Vector3.ProjectOnPlane(playerRig.forward, Vector3.up).normalized;
            Vector3 handMove = (handTransform.position - lastHandWorld);
            lastHandWorld = handTransform.position;

            Vector3 flatPull = Vector3.ProjectOnPlane(-handMove, Vector3.up);
            float f = Vector3.Dot(flatPull.normalized, rigForward);
            Vector3 steer = rigForward * Mathf.Max(0f, f) * flatPull.magnitude * (airControl / Mathf.Max(dt, 1e-4f));
            airVelocity += steer;
        }

        // 일정 시간 동안은 속도 유지(홉 느낌), 이후 감쇠
        if (hopTimer > 0f)
        {
            hopTimer -= dt;
        }
        else
        {
            // 전방/수평 감쇠
            Vector3 horiz = Vector3.ProjectOnPlane(airVelocity, Vector3.up);
            Vector3 vert = airVelocity - horiz;
            horiz *= Mathf.Clamp01(1f - airDamp * dt);
            airVelocity = horiz + vert;
        }

        // 중력 적용
        Vector3 gravity = Physics.gravity * airGravityMultiplier;
        airVelocity += gravity * dt;

        // 이동 적용
        ApplyVelocity(airVelocity);

        #region 수정
        // 바닥에 닿으면 착지
        //if (IsLanding())
        //{
        //    airborne = false;
        //    planted = false; // 착지 직후는 심플하게 비식재, 다음 프레임에 plant 가능
        //    unplantUntil = Time.time + replantCooldown * 0.5f; // 바운스 방지
        //}
        #endregion

        // 바닥에 닿으면 착지
        if (IsLanding())
        {
            airborne = false;
            planted = false; // 착지 직후는 비식재, 다음 프레임부터 plant 가능

            // 재식재(plant) 재시도까지의 기본 쿨 + 이동 쿨다운을 함께 반영
            unplantUntil = Time.time + replantCooldown * 0.5f;

            // <<< 여기 추가 : 이동 쿨다운 1초 >>>
            moveEnableUntil = Time.time + moveCooldown;
        }
    }

    bool IsLanding()
    {
        // CC가 있으면 isGrounded 신뢰, 없으면 팁 근처로 간단 체크
        if (characterController != null && moveType == MoveType.CharacterController)
        {
            return characterController.isGrounded && Vector3.Dot(airVelocity, Vector3.up) <= 0f;
        }
        // 리지드바디/트랜스폼의 경우 팁 아래로 짧게 레이/스피어캐스트
        if (tipTransform != null)
        {
            return Physics.SphereCast(
                tipTransform.position + Vector3.up * 0.01f,
                tipRadius,
                Vector3.down,
                out _,
                plantProbe + 0.02f,
                groundMask,
                QueryTriggerInteraction.Ignore
            );
        }
        return false;
    }

    void UpdateActiveGrabber()
    {
        activeGrabber = null;

        if (grabbable == null || grabbable.HeldByGrabbers == null)
            return;

        var list = grabbable.HeldByGrabbers;
        if (list.Count == 0)
            return;

        // 허용 손(왼/오른/상관없음)에 맞는 Grabber를 하나 선택
        for (int i = 0; i < list.Count; i++)
        {
            var g = list[i];
            if (g != null && IsAllowedHand(g))
            {
                activeGrabber = g;
                return;
            }
        }

        // 허용 손에 맞는 게 없으면 비활성화
        activeGrabber = null;
    }

    bool IsAllowedHand(Grabber grabber)
    {
        if (grabber == null) return false;
        if (allowedHand == AllowedHand.Any) return true;

        // 최신 BNG Grabber에는 HandSide(ControllerHand)가 있음
        var side = grabber.HandSide; // ControllerHand.Left / Right / None
        if (allowedHand == AllowedHand.Left && side == ControllerHand.Left) return true;
        if (allowedHand == AllowedHand.Right && side == ControllerHand.Right) return true;
        return false;
    }

    bool IsTipGrounded(out RaycastHit hit)
    {
        hit = default;
        if (tipTransform == null) return false;
        Vector3 origin = tipTransform.position + Vector3.up * 0.005f;
        // A tiny spherecast downward to feel the floor
        if (Physics.SphereCast(origin, tipRadius, Vector3.down, out hit, plantProbe + 0.01f, groundMask, QueryTriggerInteraction.Ignore))
            return true;
        return false;
    }


    bool IsGoodTilt()
    {
        if (handTransform == null || tipTransform == null) return false;
        // Gate planting when the crutch is nearly horizontal (prevents skating)
        Vector3 shaftDir = (handTransform.position - tipTransform.position).normalized;
        float tilt = Vector3.Angle(shaftDir, Vector3.up); // 0 = vertical
        return tilt <= maxPlantTilt;
    }


    void ApplyVelocity(Vector3 worldVel)
    {
        if (playerBody != null && moveType == MoveType.Rigidbody)
        {
            // Rigidbody는 velocity 사용 (linearVelocity는 ArticulationBody용)
            playerBody.linearVelocity += worldVel;
        }
        else if (characterController != null && moveType == MoveType.CharacterController)
        {
            // If CharacterController: move by displacement this frame
            Vector3 displacement = worldVel * Time.deltaTime;
            // respect grounding by projecting out penetration-heavy vertical when grounded
            if (characterController.isGrounded)
                displacement = Vector3.ProjectOnPlane(displacement, Vector3.up);
            characterController.Move(displacement);
        }
        else if (playerRig != null && moveType == MoveType.Transform)
        {
            // Fallback: translate the rig
            playerRig.position += worldVel * Time.deltaTime;
        }
    }
}