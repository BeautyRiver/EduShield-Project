using DarkTonic.MasterAudio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Normal, Uniqe, MiniBoss,
    }
    [Header("적 상태")]
    public EnemyType enemyType;
    public float speed;
    public float health;
    public float maxHealth;
    public float damage;
    public int exp;
    public int id;

    private bool isLive;
    [Header("참조")]
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    private Collider2D coll;
    private Rigidbody2D rigid;
    private SpriteRenderer spriter;
    private Animator anim;
    private WaitForFixedUpdate wait;
    private SortingGroup sortingGroup;
    private Vector2 nextVec;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        sortingGroup = GetComponent<SortingGroup>();
        wait = new WaitForFixedUpdate();
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.isLive && isLive)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
                return;

            // 유니크 몬스터가 아닐때 기본 이동
            if (enemyType != EnemyType.Uniqe)
            {
                Vector2 dirVec = target.position - rigid.position; // 타겟 방향
                nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
                rigid.MovePosition(rigid.position + nextVec);
            }

            // 유니크 몬스터
            else
            {
                rigid.MovePosition(rigid.position + (nextVec * speed * Time.fixedDeltaTime));
            }

            rigid.velocity = Vector2.zero;
        }
        else
            return;
    }

    private void LateUpdate()
    {
        if (GameManager.instance.isLive && isLive)
        {
            if (enemyType != EnemyType.Uniqe)
                spriter.flipX = target.position.x < rigid.position.x;
        }
        else
            return;
    }

    private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        // 초기화
        sortingGroup.sortingOrder = 1;
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        anim.SetBool("Dead", false);
        health = maxHealth;

        if (enemyType == EnemyType.Uniqe)
        {
            StartCoroutine(UniqueEnemyMove());
        }
    }

    private IEnumerator UniqueEnemyMove()
    {
        yield return null;
        yield return null;
        nextVec = (target.position - rigid.position).normalized;
        spriter.flipX = target.position.x < rigid.position.x;
    }

    public void Init(SpawnData data)
    {
        id = data.spriteType;
        anim.runtimeAnimatorController = animCon[id];
        speed = data.speed;
        maxHealth = data.health;
        health = maxHealth;
        exp = data.exp;
        damage = data.damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLive)
            return;

        if (collision.CompareTag("Cleaner"))
        {
            isLive = false;
            coll.enabled = false; // 콜라이더 끄기
            rigid.simulated = false;
            anim.SetBool("Dead", true);
        }

        if (collision.CompareTag("Bullet"))
        {            
            Bullet bulletInfo = collision.GetComponent<Bullet>();
            Vector2 hitPos;
            float damage = bulletInfo.damage;

            // 이펙트
            GameObject effect = GameManager.instance.pool.Get(PoolManager.PoolType.Effect, 0);
            effect.transform.position = collision.ClosestPoint(transform.position);

            health -= damage; // 체력 감소            
            hitPos = collision.ClosestPoint(transform.position); // 충돌한 지점의 정확한 위치를 구하기
            ShowDamageText(damage.ToString("F1"), damage, hitPos, Color.white); // 기본 데미지

            // 추가 데미지 구현 로직
            if (bulletInfo.id == id)
            {
                health -= damage * 0.5f;
                // 충돌한 지점의 정확한 위치를 구하기
                hitPos = collision.ClosestPoint(transform.position);
                ShowDamageText($"+{(damage* 0.5f).ToString("F1")}", damage * 0.5f, new Vector2(hitPos.x,hitPos.y + 0.5f), Color.red);
            }
            StartCoroutine(KnockBack()); // 넉백
            anim.SetTrigger("Hit"); // 맞는 애니메이션 재생                                    
            MasterAudio.PlaySound("Hit"); // 사운드 재생

            // 체력 0 이하 사망
            if (health <= 0) 
            {
                // 미니 보스가 아닐때
                if (enemyType != EnemyType.MiniBoss)
                {
                    GameObject expObj = GameManager.instance.pool.Get(PoolManager.PoolType.Enemy, 1); // Exp 드랍시키기
                    expObj.transform.position = transform.position;
                    expObj.GetComponent<Exp>().exp = this.exp;
                }
                else
                {
                    GameObject reward = GameManager.instance.pool.Get(PoolManager.PoolType.Item, 3);
                    reward.transform.position = transform.position;
                }

                isLive = false;
                coll.enabled = false; // 콜라이더 끄기
                rigid.simulated = false;
                anim.SetBool("Dead", true);
                GameManager.instance.kill++;

                if (GameManager.instance.isLive)
                    MasterAudio.PlaySound("Dead");
            }
        }
        else
            return;
    }

    private void ShowDamageText(string text, float damage, Vector2 hitPos, Color color)
    {
        GameObject damageTextobj = GameManager.instance.pool.Get(PoolManager.PoolType.Enemy, 0);
        TextMeshPro damageText = damageTextobj.GetComponent<TextMeshPro>();

        damageText.color = color;
        damageTextobj.transform.localPosition = hitPos;
        damageText.text = text;
        damageText.DOScale(1f, 0.3f);
        StartCoroutine(OffDamageText(damageText));
    }

    private IEnumerator OffDamageText(TextMeshPro damageText)
    {
        yield return new WaitForSeconds(0.35f);
        damageText.DOScale(0, 0.5f).OnComplete(()=> damageText.gameObject.SetActive(false));
    }

    private IEnumerator KnockBack()
    {
        yield return wait; // 다음 하나의 물리 프레임 딜레이
        Vector3 playerPos = target.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 1.5f, ForceMode2D.Impulse);
    }

    public void SetOrderLayer()
    {
        sortingGroup.sortingOrder = 0;
    }
    private void Dead()
    {        
        gameObject.SetActive(false);
    }
}

