using UnityEngine;
using TMPro;

//2025-07-17 OJY
public enum ChangeType
{
    increase,
    decrease
}

public class TalismanManager : MonoBehaviour
{
    static TalismanManager instance;
    public static TalismanManager Instance
    {
        get { return instance; }
    }

    public int talismanCount;
    ThrowTalisman throwTalisman;

    [Header("UI")]
    [SerializeField]TextMeshProUGUI countText;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 초기화
    /// </summary>
    void Init()
    {
        talismanCount = 0;
        countText.text = talismanCount.ToString();
    }

    /// <summary>
    /// 가진 개수 반환
    /// </summary>
    /// <returns></returns>
    public int GetCount()
    {
        return talismanCount;
    }

    /// <summary>
    /// 보유 부적 개수 및 텍스트 갱신
    /// </summary>
    /// <param name="type">증가 / 감소</param>
    public void CountRenew(ChangeType type)
    {
        switch (type) 
        {
            case ChangeType.increase:
                talismanCount++;
                break;
            case ChangeType.decrease:
                talismanCount--;
                break;
            default:
                break;
        }

        countText.text = talismanCount.ToString();
    }
}
