using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Bullet : MonoBehaviour
{
    [field: SerializeField] public float damage { get; private set; }
    [field: SerializeField] public float per { get; private set; }
    [field: SerializeField] public float bulletSpeed { get; private set; }
    [field: SerializeField] public int id { get; private set; }
    [field: SerializeField] public float knockBackDistance { get; private set; }
    [field: SerializeField] public float damageInterval { get; private set; }
    [field: SerializeField] public  string ownerTag { get; set; } // 총알을 발사한 오브젝트
    protected Rigidbody2D rigid;
    protected Collider2D bulletCol;
    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        bulletCol = GetComponent<Collider2D>();
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || per == -100)
            return;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Bullet Init Method
    /// </summary>    
    public virtual void Init(Vector3 dir, string ownerTag, float per, float damage, float bulletSpeed, float knockBack, float interval)
    {
        this.ownerTag = ownerTag;
        this.damage = damage;
        this.per = per;
        this.bulletSpeed = bulletSpeed;
        knockBackDistance = knockBack;
        damageInterval = interval;

        // 관통력이 있는 무기일 때 Bullet Move
        if (per >= 0)
        {
            rigid.linearVelocity = dir * this.bulletSpeed;
        }
    }

    protected virtual void PerDown()
    {
        if (per == -100)
            return;

        per--;
        if (per < 0)
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
