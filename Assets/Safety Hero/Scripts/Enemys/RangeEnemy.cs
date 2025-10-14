using DarkTonic.MasterAudio;
using System;
using System.Collections;
using UnityEngine;

public class RangeEnemy : Enemy, IRepositionable, IKnockBackable, IMovable, IAttackable
{
    [Header("# Range Enemy Settings")]
    [SerializeField] private float retreatDistance = 4f; // 이 거리보다 가까워지면 도망감
    [SerializeField] private float attackCooldown = 2f; // 공격 쿨다운
    [SerializeField] private BulletData bulletData;
    private TargetScanner targetScanner;
    // 상태 관리를 위한 Enum
    public enum State { Chasing, Attacking, Retreating }
    public State currentState;

    private float lastAttackTime; // 마지막 공격 시간
    private bool isAttackReady = true; // 공격 애니메이션 및 쿨다운 관리용 플래그
    private Vector2 moveDirection; // 이동할 방향

    protected override void Awake()
    {
        base.Awake();
        targetScanner = GetComponent<TargetScanner>();
    }
    private void Update()
    {
        if (!isLive || targetRb == null) return;

        // 1. 플레이어와의 거리에 따라 현재 상태 결정
        UpdateState();

        // 2. 결정된 상태에 따라 행동 개시
        ActByState();
    }

    protected override void FixedUpdate()
    {
        // FixedUpdate에서는 물리적인 이동만 담당
        base.FixedUpdate();
        if (!isLive) return;
        FlipX();
        Move();
    }

    /// <summary>
    /// 플레이어와의 거리에 따라 상태(State)를 결정합니다.
    /// </summary>
    private void UpdateState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, targetRb.position);

        if (distanceToPlayer < retreatDistance)
        {
            currentState = State.Retreating;
        }
        else if (distanceToPlayer <= targetScanner.scanRange)
        {
            currentState = State.Attacking;
        }
        else
        {
            currentState = State.Chasing;
        }
    }

    /// <summary>
    /// 현재 상태(State)에 따라 행동 방향을 정하거나 공격을 시작합니다.
    /// </summary>
    private void ActByState()
    {
        switch (currentState)
        {
            case State.Retreating:
                // 플레이어로부터 멀어지는 방향 설정
                moveDirection = ((Vector2)transform.position - targetRb.position).normalized;
                Attack(); // 도망가면서 공격 시도
                break;
            case State.Chasing:
                // 플레이어를 향해 다가가는 방향 설정
                moveDirection = (targetRb.position - (Vector2)transform.position).normalized;
                isAttackReady = true; // 추격 중에는 항상 공격 준비 상태
                break;
            case State.Attacking:
                // 제자리에 멈춤
                moveDirection = Vector2.zero;
                Attack(); // 공격 시도
                break;
        }
    }

    // IMovable 인터페이스 구현
    public void Move()
    {
        // 공격 애니메이션이 재생 중일 때는 움직이지 않음
        if (!isAttackReady)
        {
            rigid.linearVelocity = Vector2.zero;
            return;
        }

        // ActByState에서 결정된 방향으로 이동
        nextVec = moveDirection * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    // IAttackable 인터페이스 구현
    public void Attack()
    {
        // 공격 준비가 되었고, 쿨다운이 지났는지 확인
        if (isAttackReady && Time.time > lastAttackTime + attackCooldown)
        {
            isAttackReady = false; // 공격 시작, 더 이상 움직이거나 공격하지 않음
            lastAttackTime = Time.time;

            // 공격 애니메이션 트리거
            anim.SetTrigger("doAttack");
        }
    }

    // --- 애니메이션 이벤트에서 호출될 함수들 ---

    /// <summary>
    /// 이 함수는 공격 애니메이션의 특정 프레임에 이벤트로 등록되어야 합니다.
    /// </summary>
    public void FireBulletFromAnimation()
    {
        if (bulletData == null)
        {
            Debug.LogError($"{myData.name} 에 weaponData가 설정되지 않았습니다!");
            return;
        }
        if (targetScanner == null || targetScanner.nearestTarget == null) return;

        // 발사 로직 (코루틴 없이 단발로 처리)
        Vector3 targetPos = targetScanner.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;

        Transform bullet = PoolManager.instance.Get(bulletData.bulletPrefab).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        bullet.GetComponent<Bullet>()?.Init(
            dir,
            this.tag,
            bulletData.basePer,
            myData.damage, // 몬스터의 기본 데미지 사용
            bulletData.baseBulletMoveSpeed,
            bulletData.baseKnockback,
            bulletData.baseDamageInterval
        );

        // TODO: 몬스터 발사 사운드
        // MasterAudio.PlaySound("EnemyShoot");
    }

    /// <summary>
    /// 이 함수는 공격 애니메이션의 마지막 프레임에 이벤트로 등록되어야 합니다.
    /// </summary>
    public void FinishAttackAnimation()
    {
        isAttackReady = true; // 공격이 끝났으니 다시 움직이거나 공격할 수 있음
    }

    protected override void FlipX()
    {
        if (!isAttackReady) return; // 공격 중에는 방향 전환 안함

        // 플레이어의 위치를 기준으로 방향 전환
        spriter.flipX = targetRb.position.x > rigid.position.x;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }

}
