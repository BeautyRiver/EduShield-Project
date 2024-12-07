using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{    
    [field: SerializeField]
    public float Damage { get; private set; }    

    [field: SerializeField]
    public int Per { get; private set; }  

    [field: SerializeField]
    public int Id { get; private set; }   

    [field: SerializeField]
    public float KnockBackDistance { get; private set; }

    [field: SerializeField]
    public float DamageInterval { get; private set; }

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
    public virtual void Init(float damage, int per, Vector3 dir, int id, float knockBack, float interval)
    {
        Damage = damage;
        Per = per;
        Id = id;
        KnockBackDistance = knockBack;
        DamageInterval = interval;

        // 관통력이 있는 무기일 때 Bullet Move
        if (per >= 0)
        {
            rigid.linearVelocity = dir * 15f;
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
