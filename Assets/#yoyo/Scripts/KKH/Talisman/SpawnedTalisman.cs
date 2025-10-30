using System.Collections;
using UnityEngine;

//2025-07-15 OJY
public class SpawnedTalisman : MonoBehaviour
{
    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //날아가는 방향 보기
        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            rb.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
        }
    }

    /// <summary>
    /// 속력 줘서 날림
    /// </summary>
    /// <param name="forward">날아가는 방향</param>
    /// <param name="speed">속도</param>
    /// <param name="scale">탄의 크기</param>
    public void ShotSetting(Vector3 forward,float speed, float scale)
    {
        rb.linearVelocity = forward * speed;
        transform.localScale *= scale;

        Destroy(gameObject, 5.0f); 
    }

    /// <summary>
    /// isTrigger쓸거면 이거
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log(other.name + "명중 (트리거)");

            //몬스터가 가진 클래스 호출 및 이벤트 실행

            Destroy(gameObject);
            other.GetComponent<MonsterAI>().Stren();
        }
    }



    ///// <summary>
    ///// isTrigger 안쓸거면 이거
    ///// </summary>
    ///// <param name="collision"></param>
    //private void OnCollisionEnter(Collision collision)
    //{
    //    GameObject hitTarget = collision.gameObject;
    //    if (hitTarget.CompareTag("Monster"))
    //    {
    //        Debug.Log(hitTarget.name + "명중 (콜리전)");
    //        //몬스터가 가진 클래스 호출 및 이벤트 실행
    //        Destroy(gameObject);
    //    }
    //}
}
