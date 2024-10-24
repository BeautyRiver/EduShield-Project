using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per;
    public int id;
    public float knockBackDistance;
    public float damageInterval;
    public Weapon weaponType;
    private Rigidbody2D rigid;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage">총알 데미지</param>
    /// <param name="per">총알 관통력</param>
    /// <param name="dir">총알 방향</param>
    /// <param name="id">아이디</param>
    /// <param name="knockBack">넉백량</param>
    /// <param name="interval">가해지는 공격 간격</param>
    public void Init(float damage, int per, Vector3 dir, int id, float knockBack = 1.5f, float interval = 0.2f)
    {
        this.damage = damage;
        this.id = id;
        this.per = per;
        this.knockBackDistance = knockBack;
        this.damageInterval = interval;

        // 근접 무기 아닐때 (관통 제한 있을때) 
        // 속도 세팅
        if (per >= 0)
        {
            rigid.velocity = dir * 15f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || per == -100)
            return;

        gameObject.SetActive(false);
    }

    public void PerDown()
    {
        if (per == -100)
            return;

        per--;
        if (per < 0)
        {
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

    private void ActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
