using BNG;
using System.Collections.Generic;
using UnityEngine;

//2025-07-15 OJY
public class ThrowTalisman : MonoBehaviour
{
    [Header("부적 프리펩")]
    [SerializeField] GameObject bulletPrefab;

    [Header("세팅")]
    [SerializeField] float speed;
    [SerializeField] float scale;

    [Header("머리")]
    [SerializeField] Transform forwardTargetRight; //눈
    [SerializeField] Transform forwardTargetLeft; //눈

    TalismanManager manager;
    public List<ControllerBinding> TalismanInput = new List<ControllerBinding>() { ControllerBinding.None };
    public ControllerBinding cancelnput = ControllerBinding.None; //취소 입력, 디버그용

    float MaxCoolTime = 1.0f; //부적 쿨타임
    [SerializeField] float CoolTime = 0.0f; //부적 쿨타임 카운트

    [SerializeField] int clcikCount = 0; //클릭 횟수, 디버그용

    [SerializeField] GameObject rMark; //오른쪽 마크
    [SerializeField] GameObject lMark; //왼쪽 마크

    [SerializeField] private WallOpenTrigger wallOpenTrigger; //벽 열기 트리거, 벽이 있는 경우에만 사용

    private void Awake()
    {
        manager = TalismanManager.Instance;

        if (!manager) enabled = false;

        CoolTime = MaxCoolTime;

        MarkOnOff(false); //마크 비활성화
    }

    private void Start()
    {
    }

    private void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnBullet(ControllerBinding.RightTriggerDown);
        }
#endif

        if (CoolTime >= MaxCoolTime)
        {
            if (manager.GetCount() > 0)
            {
                ClickTalisman();
            }
        }
        else if (CoolTime < MaxCoolTime)
        {
            CoolTime += Time.deltaTime;
        }

        if (InputBridge.Instance.GetControllerBindingValue(cancelnput))
        {
            if (clcikCount > 0)
            {
                Debug.Log("부적 취소 입력.");
                MarkOnOff(false); //마크 비활성화
                clcikCount = 0; //클릭 횟수 초기화
            }
        }
    }


    private void ClickTalisman()
    {
        if (!wallOpenTrigger.isBookOpen)
        {
            return; //책이 열려있지 않으면 부적을 던질 수 없음
        }

        for (int x = 0; x < TalismanInput.Count; x++)
        {
            if (InputBridge.Instance.GetControllerBindingValue(TalismanInput[x]))
            {
                if (clcikCount == 0)
                {
                    MarkOnOff(true); //마크 비활성화
                    clcikCount++; //클릭 횟수 증가
                    CoolTime = 0.0f;
                }
                else
                {
                    if (SpawnBullet(TalismanInput[x]))
                    {
                        CoolTime = 0.0f; //쿨타임 초기화
                        clcikCount = 0;
                    }
                    else
                    {
                    }
                }
                break;
            }
        }
    }

    /// <summary>
    /// 총알 생성
    /// </summary>
    public bool SpawnBullet(ControllerBinding controllerBinding)
    {


        if (manager.GetCount() > 0)
        {
            GameObject obj = Instantiate(bulletPrefab);
            if (controllerBinding == ControllerBinding.LeftTriggerDown)
            {
                obj.transform.position = forwardTargetLeft.position;// - (forwardTargetLeft.up * 0.1f); //중점에서 살짝 아래로 위치 조정
                obj.transform.rotation = forwardTargetLeft.rotation;

                SpawnedTalisman talisman = obj.GetComponent<SpawnedTalisman>();
                if (talisman) talisman.ShotSetting(forwardTargetLeft.forward, speed, scale);//보는 방향으로 맞춤
            }
            else if (controllerBinding == ControllerBinding.RightTriggerDown)
            {
                obj.transform.position = forwardTargetRight.position;// - (forwardTargetRight.up * 0.1f); //중점에서 살짝 아래로 위치 조정
                obj.transform.rotation = forwardTargetRight.rotation;                             //보는 방향으로 맞춤

                SpawnedTalisman talisman = obj.GetComponent<SpawnedTalisman>();
                if (talisman) talisman.ShotSetting(forwardTargetRight.forward, speed, scale);
            }
            else
            {
                return false; //지원하지 않는 컨트롤러 바인딩
            }

            manager.CountRenew(ChangeType.decrease);

            MarkOnOff(false); //마크 비활성화
            return true; //발사 성공
        }
        return false; //발사 실패, 부적이 없음
    }

    private void MarkOnOff(bool isOn)
    {
        if (rMark)
        {
            rMark.SetActive(isOn); //오른쪽 마크 비활성화
        }

        if (lMark)
        {
            lMark.SetActive(isOn); //왼쪽 마크 비활성화
        }
    }
}