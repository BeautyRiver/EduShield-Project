using DarkTonic.MasterAudio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    protected EnemyData myData;
    public int id;
    public float damage;
    public float currentHealth;
    public float maxHealth;
    public float speed;
    public int exp;
    public bool isLive;
    [SerializeField] protected Vector2 nextVec;
    protected Vector2 dirVec;

    [Header("# 참조")]
    [SerializeField] protected RuntimeAnimatorController animCon;
    protected GameObject effect;
    protected Rigidbody2D targetRb;
    protected Collider2D coll;
    protected Rigidbody2D rigid;
    protected SpriteRenderer spriter;
    protected Animator anim;
    protected SortingGroup sortingGroup;
    protected GameManager gm;

    protected virtual void Awake()
    {
        // 초기 할당        
        gm = GameManager.instance;
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        sortingGroup = GetComponent<SortingGroup>();
    }

    protected virtual void FixedUpdate()
    {
        if (!gm.isGameActive || !isLive)
            return;

        rigid.linearVelocity = Vector2.zero;

    }

    protected abstract void FlipX();

    protected virtual void OnEnable()
    {
        // 초기화
        targetRb = gm.player.GetComponent<Rigidbody2D>();
        sortingGroup.sortingOrder = 1;
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        anim.SetBool("isDead", false);
        currentHealth = maxHealth;
    }
    public virtual void Init(EnemyData data)
    {
        myData = data;

        id = myData.id;
        speed = myData.speed;
        currentHealth = myData.health;
        exp = myData.exp;
        damage = myData.damage;
        animCon = myData.animCon;
        maxHealth = myData.health;
        anim.runtimeAnimatorController = animCon;
    }


    // 데미지 받는 로직
    public void DamagedLogic(Collider2D collision, float damage)
    {
        Debug.Log("DamagedLogic");
        Bullet bulletInfo = collision.GetComponent<Bullet>();
        Vector2 hitPos;

        // 충돌한 지점의 정확한 위치를 구하기
        hitPos = collision.ClosestPoint(transform.position);
        GameObject effect = PoolManager.instance.Get(myData.hitEffectPrefab); // Enemy 이팩트 생성
        effect.transform.position = hitPos;

        float fontSize = 7f;
        // 기본 데미지 표시 
        Damaged(damage, hitPos, Color.white, false, fontSize);

        /*
        // 기본 타입일 때
        if (gm.typeControll.TypeIndex == -1)
        {
            // 기본 데미지 표시 
            Damaged(damage, hitPos, Color.white, false, fontSize);
        }
        // 기본 타입이 아닐 때
        else
        {
            if (gm.typeControll.TypeIndex == id)
            {
                // 기본 데미지 표시 + 추가 데미지
                Damaged(damage, hitPos, Color.white, true, fontSize);
            }
            else
            {
                // 데미지 반감
                damage = damage * 0.5f;
                // 기본 데미지 표시 
                Damaged(damage, hitPos, Color.gray, false, fontSize);
            }
        }*/

        MasterAudio.PlaySound("Hit"); // 사운드 재생
        anim.SetTrigger("doHit"); // 맞는 애니메이션 재생

        // 보스는 넉백 X
        if (this is IKnockBackable)
            StartCoroutine(KnockBack(bulletInfo.KnockBackDistance)); // 넉백

        // 체력 0 이하 사망
        if (currentHealth <= 0)
        {
            DropReward(); // 보상
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            anim.SetBool("isDead", true);
            gm.playerKill++;
            MasterAudio.PlaySound("Dead");
        }
    }

    protected virtual void DropReward()
    {
        GameObject expObj = PoolManager.instance.Get(myData.dropExpPrefab); // exp 생성
        expObj.transform.position = transform.position;
        expObj.GetComponent<Exp>().exp = this.exp;
    }
    private void Damaged(float damage, Vector2 hitPos, Color color, bool isPlusDamage, float fontSize)
    {
        GameObject damageTextobj = PoolManager.instance.Get(myData.damageTextPrefab); // 데미지 텍스트 생성
        TextMeshPro damageText = damageTextobj.GetComponent<TextMeshPro>();

        currentHealth -= damage; // 체력 감소            
        damageText.fontSize = fontSize; // 폰트 사이즈 설정
        damageText.text = damage.ToString("F1");
        damageText.color = color;
        damageTextobj.transform.localPosition = hitPos;

        // 데미지 텍스트 이펙트 위로 이동하면서 투명해지면서 사라지게        
        StartCoroutine(TextAnimationCor(damageText));
        if (isPlusDamage)
        {
            // hitPos.y를 증가시켜 위로 조금 더 올라가게 함
            Damaged(damage, hitPos + Vector2.up * 0.5f, Color.red, false, fontSize * 0.9f);
        }
    }
    private IEnumerator TextAnimationCor(TextMeshPro text)
    {
        text.transform.DOMove(text.transform.position + Vector3.up * 1f, 1f);
        yield return new WaitForSeconds(0.5f);
        text.DOFade(0, 0.5f).OnComplete(() => text.gameObject.SetActive(false));
    }

    private IEnumerator OffDamageTextCor(TextMeshPro damageText)
    {
        yield return new WaitForSeconds(0.5f);
        damageText.DOScale(0, 0.5f).OnComplete(() => damageText.gameObject.SetActive(false));
    }

    private IEnumerator KnockBack(float knockBackDistance)
    {
        yield return null; // 다음 하나의 물리 프레임 딜레이
        Vector3 playerPos = targetRb.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * knockBackDistance, ForceMode2D.Impulse);
    }

    // For Event
    private void SetOrderLayerDownAtDead()
    {
        sortingGroup.sortingOrder = 0;
    }
    private void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
