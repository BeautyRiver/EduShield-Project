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
    [Header("# 상태")]
    public EnemyType enemyType;
    public float speed;
    public float health;
    public float maxHealth;
    public float damage;
    public int exp;
    public int id;
    public bool isLive;
    [SerializeField]
    private Dictionary<Collider2D, float> damageCooldowns;

    [Header("# 참조")]
    public TypeControlManager typeControlManager;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    private Collider2D coll;
    private Rigidbody2D rigid;
    private SpriteRenderer spriter;
    private Animator anim;
    private WaitForFixedUpdate wait;
    private SortingGroup sortingGroup;
    private GameManager gm;
    private Vector2 nextVec;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        sortingGroup = GetComponent<SortingGroup>();
        wait = new WaitForFixedUpdate();
        gm = GameManager.instance;
    }

    private void FixedUpdate()
    {
        if (gm.isLive && isLive)
        {
           if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit") && enemyType != EnemyType.MiniBoss)
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
        if (gm.isLive && isLive)
        {
            if (enemyType != EnemyType.Uniqe)
                spriter.flipX = target.position.x < rigid.position.x;
        }
        else
            return;
    }

    private void OnEnable()
    {
        damageCooldowns = new Dictionary<Collider2D, float>();
        target = gm.player.GetComponent<Rigidbody2D>();
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isLive)
            return;

        if (collision.CompareTag("Bullet"))
        {
            Bullet bulletInfo = collision.GetComponent<Bullet>();
            float currentTime = Time.time;
            if (damageCooldowns.ContainsKey(collision))
            {

            }
            DamgedLogic(collision);
        }

        if (collision.CompareTag("Cleaner"))
        {
            isLive = false;
            coll.enabled = false; // 콜라이더 끄기
            rigid.simulated = false;
            anim.SetBool("Dead", true);
        }

    }

    public void DamgedLogic(Collider2D collision)
    {
        Bullet bulletInfo = collision.GetComponent<Bullet>();
        Vector2 hitPos;
        float damage = bulletInfo.damage;

        // 이펙트
        hitPos = collision.ClosestPoint(transform.position); // 충돌한 지점의 정확한 위치를 구하기
        GameObject effect = gm.pool.Get(PoolManager.PoolType.Effect, 0);
        effect.transform.position = hitPos;

        // 기본 타입일 때
        if (gm.typeControll.TypeIndex == -1)
        {
            Damaged(damage.ToString("F1"), damage, hitPos, Color.white); // 기본 데미지 표시 
        }
        // 기본 타입이 아닐 때
        else
        {
            if (gm.typeControll.TypeIndex == id)
            {
                Damaged(damage.ToString("F1"), damage, hitPos, Color.white); // 기본 데미지 표시 
                                                                             // 추가 데미지
                float plusDamage = damage;
                Damaged($"+{(plusDamage).ToString("F1")}", plusDamage, new Vector2(hitPos.x, hitPos.y + 0.5f), Color.red);
            }
            else
            {
                // 데미지 반감 로직
                damage = damage * 0.5f;
                Damaged(damage.ToString("F1"), damage, hitPos, Color.gray); // 기본 데미지 표시 
            }
        }

        anim.SetTrigger("Hit"); // 맞는 애니메이션 재생                                    
        MasterAudio.PlaySound("Hit"); // 사운드 재생

        // 보스는 넉벡 X
        if (enemyType != EnemyType.MiniBoss)
            StartCoroutine(KnockBack(bulletInfo.knockBackDistance)); // 넉백

        // 체력 0 이하 사망
        if (health <= 0)
        {
            // 미니 보스가 아닐때
            if (enemyType != EnemyType.MiniBoss)
            {
                GameObject expObj = gm.pool.Get(PoolManager.PoolType.Enemy, 1); // Exp 드랍시키기
                expObj.transform.position = transform.position;
                expObj.GetComponent<Exp>().exp = this.exp;
            }
            else
            {
                GameObject reward = gm.pool.Get(PoolManager.PoolType.Item, 3);
                reward.transform.position = transform.position;
            }

            isLive = false;
            coll.enabled = false; // 콜라이더 끄기
            rigid.simulated = false;
            anim.SetBool("Dead", true);
            gm.kill++;

            if (gm.isLive)
                MasterAudio.PlaySound("Dead");
        }
    }

    private void Damaged(string text, float damage, Vector2 hitPos, Color color)
    {
        GameObject damageTextobj = gm.pool.Get(PoolManager.PoolType.Enemy, 0);
        TextMeshPro damageText = damageTextobj.GetComponent<TextMeshPro>();

        health -= damage; // 체력 감소            
        damageText.color = color;
        damageTextobj.transform.localPosition = hitPos;
        damageText.text = text;
        damageText.DOScale(1f, 0.1f);
        StartCoroutine(OffDamageText(damageText));
    }

    private IEnumerator OffDamageText(TextMeshPro damageText)
    {
        yield return new WaitForSeconds(0.35f);
        damageText.DOScale(0, 0.35f).OnComplete(()=> damageText.gameObject.SetActive(false));
    }

    private IEnumerator KnockBack(float knockBackDistance)
    {
        yield return wait; // 다음 하나의 물리 프레임 딜레이
        Vector3 playerPos = target.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * knockBackDistance, ForceMode2D.Impulse);
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
