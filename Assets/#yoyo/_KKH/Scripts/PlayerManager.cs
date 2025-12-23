using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    static public PlayerManager instance;

    [Header("플레이어 죽음")]
    [SerializeField] private GameObject go_Blood;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void OnEnable()
    {
        if (go_Blood.activeInHierarchy)
        {
            go_Blood.SetActive(false);
        }
    }

    public void Die()
    {
        go_Blood.SetActive(true);
    }
}
