using UnityEngine;

public class IdCard : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] int index;

    public void GetCard()
    {
        GameManager.instace.HasIdCard(name, index);
        SoundManager.I.PlaySFXAt("GetCard", transform.position);
        Destroy(gameObject);
    }
}
