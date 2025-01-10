using DarkTonic.MasterAudio;
using System;
using System.Collections;
using UnityEngine;

public class RangeEnemy : Enemy, IRepositionable, IKnockBackable, IMovable, IAttackable
{
    private TargetScanner targetScanner; // 적 탐색기        
    [SerializeField] private BulletData bulletData; // 총알 데이터
    [SerializeField] private bool isAttacking = false; // 현재 공격 중인지 여부
    [SerializeField] private bool isCoolTimeOn = false; // 쿨다운 중인지 여부
    [SerializeField] private float attackTimer;
    [SerializeField] private float variableSpeed;
    private bool isBackStep = false;
    private Vector3 targetPos;
    private Vector3 dir;
    protected override void Awake()
    {
        base.Awake();
        targetScanner = GetComponent<TargetScanner>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        FlipX();
        Move();
        UpdateAttackCooldown();
        Detection();
    }
    protected override void FlipX()
    {
        spriter.flipX = targetRb.position.x < rigid.position.x;
    }

    // 이동
    public void Move()
    {
        if (isBackStep)
            return;

        if (isAttacking)
        {
            dirVec = Vector3.zero;
            variableSpeed = 0f;
        }

        else if (targetScanner.nearestTarget == null)
        {
            dirVec = targetRb.position - rigid.position;
            variableSpeed = speed;
        }

        else if (targetScanner.nearestTarget != null && !isBackStep)
        {
            StartCoroutine(BackStep());
        }

        nextVec = dirVec.normalized * variableSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private IEnumerator BackStep()
    {        
        dirVec = rigid.position - targetRb.position;
        variableSpeed = speed * 0.5f;
        yield return new WaitForSeconds(1f);
        isBackStep = true;
    }

    protected override void DropReward()
    {
        GameObject expObj = GameManager.instance.poolManager.Get(PoolType.Item, 0); // expCount 생성
        expObj.transform.position = transform.position;
        expObj.GetComponent<Exp>().exp = this.exp;
    }  

    private void UpdateAttackCooldown()
    {
        if (isCoolTimeOn)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= bulletData.baseAttackSpeed)
            {
                isCoolTimeOn = false;
                attackTimer = 0;
            }
        }
    }

    public void Detection()
    {
        if (targetScanner.nearestTarget == null || isAttacking || isCoolTimeOn)
            return;        

        StartCoroutine(AttackRoutine());
        isAttacking = true;
    }

    // 공격
    public void Attack()
    {        
        Transform bullet = GameManager.instance.poolManager.GetByPrefab(PoolType.EnemyBullet, bulletData.prefab).transform;
        bullet.parent = transform;

        targetPos = targetRb.position;
        dir = (targetPos - transform.position).normalized;

        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        // 불렛 초기화
        bullet.GetComponent<Bullet>().Init(
                      dir,
                      this.tag,
                      bulletData.basePer,
                      bulletData.baseDamage,
                      bulletData.baseBulletSpeed,
                      bulletData.baseKnockback,
                      bulletData.baseDamageInterval
                  );

        // 발사 사운드
        MasterAudio.PlaySound("R50_TargetGun");
    }

    public IEnumerator AttackRoutine()
    {
        for (int i = 0; i < bulletData.baseCount; i++)
        {
            while (IsPlayingAttackAnim())
            {
                yield return null;
            }

            anim.SetTrigger("doAttack");
            yield return new WaitForSeconds(bulletData.baseDelay);
        }
        yield return new WaitForSeconds(0.1f);

        isAttacking = false;
        isCoolTimeOn = true;
    }

    // "doAttack" 상태인지 체크하는 메서드
    private bool IsPlayingAttackAnim()
    {
        // 0번 레이어(기본 레이어) 상태 정보 가져오기
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // doAttack 스테이트가 재생 중이고,
        // normalizedTime(0 ~ 1 사이)가 아직 1 이상이 되지 않았으면(즉 종료되지 않았으면) true
        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime < 1f)
        {
            return true;
        }

        return false;
    }
}
