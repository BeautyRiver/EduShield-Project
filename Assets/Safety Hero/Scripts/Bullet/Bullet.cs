using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [field: SerializeField]
    public float Damage { get; private set; }     // 읽기 전용

    [field: SerializeField]
    public int Per { get; private set; }  // 읽기 전용

    [field: SerializeField]
    public int Id { get; private set; }           // 읽기 전용

    [field: SerializeField]
    public float KnockBackDistance { get; private set; }

    [field: SerializeField]
    public float DamageInterval { get; private set; }

    protected Rigidbody2D rigid;
    protected Collider2D bulletCol;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        bulletCol = GetComponent<Collider2D>();
    }
    /// <summary>
    /// 초기화 함수
    /// </summary>
    /// <param name="damage">총알 데미지</param>
    /// <param name="per">총알 관통력</param>
    /// <param name="dir">총알 방향</param>
    /// <param name="id">아이디</param>
    /// <param name="knockBack">넉백량</param>
    /// <param name="interval">가해지는 공격 간격</param>
    public virtual void Init(float damage, int per, Vector3 dir, int id, float knockBack, float interval)
    {
        Damage = damage;
        Per = per;
        Id = id;
        KnockBackDistance = knockBack;
        DamageInterval = interval;

        // 근접 무기 아닐때 (관통 제한 있을때) 
        // 속도 세팅
        if (per >= 0)
        {
            rigid.velocity = dir * 15f;
        }
    }

    protected virtual void PerDown()
    {
        if (Per == -100)
            return;

        Per--;
        if (Per < 0)
        {
            rigid.velocity = Vector2.zero;
            ActiveFalse();
        }
    }

    protected void ActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
