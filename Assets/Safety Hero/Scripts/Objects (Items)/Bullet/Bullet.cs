using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public int Per { get; private set; }
    [field: SerializeField] public float BulletSpeed { get; private set; }
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public float KnockBackDistance { get; private set; }
    [field: SerializeField] public float DamageInterval { get; private set; }
    [field: SerializeField] public  string OwnerTag { get; set; } // 총알을 발사한 오브젝트
    protected Rigidbody2D rigid;
    protected Collider2D bulletCol;
    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        bulletCol = GetComponent<Collider2D>();
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || Per == -100)
            return;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Bullet Init Method
    /// </summary>    
    public virtual void Init(Vector3 dir, string ownerTag, int per, float damage, float bulletSpeed, float knockBack, float interval)
    {
        OwnerTag = ownerTag;
        Damage = damage;
        Per = per;
        BulletSpeed = bulletSpeed;
        KnockBackDistance = knockBack;
        DamageInterval = interval;

        // 관통력이 있는 무기일 때 Bullet Move
        if (per >= 0)
        {
            rigid.linearVelocity = dir * BulletSpeed;
        }
    }

    protected virtual void PerDown()
    {
        if (Per == -100)
            return;

        Per--;
        if (Per < 0)
        {
            rigid.linearVelocity = Vector2.zero;
            ActiveFalse();
        }
    }

    protected void ActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
