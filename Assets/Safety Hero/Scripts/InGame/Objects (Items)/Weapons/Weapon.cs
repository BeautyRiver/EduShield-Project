using System;
using UnityEditor;
using UnityEngine;
using VInspector;

[System.Serializable]
public class WeaponStats
{
    public float damage;                // 무기 데미지    
    public float count;                   // 무기 개수
    public float per;                     // 관통력
    public float bulletMoveSpeed;       // 총알 이동 속도
    public float weaponAttackSpeed;     // 무기 속도    
    public float rotationSpeed;         // 회전속도       (*현재 ONLY 회전무기)
    public float bulletDelay;           // 총알 사이 딜레이 (Range)   
    public float damageInterval;        // 데미지 줄 수 있는 텀
    public float weaponDuration;        // 지속 시간(회전 무기만 일단)
    public float attackRange;           // 공격 범위
    public float knockBackAmout;        // 몬스터 넉백량
    public Vector3 bulletSize;          // 총알(무기) 크기
}
public abstract class Weapon : MonoBehaviour
{    
    public string ownerTag;      // 무기 소유자 태그
    public int level = 0;        // 현재 레벨

    [Header("# Live Stats (Read Only)")]
    [SerializeField] protected BulletData currentBulletData;
    public WeaponStats finalStats;

    [Header("공격 쿨타임/타이머")]
    public bool isAttacking;
    public float speedTimer;

    [Header("참조")]
    protected GameManager gm;
    protected PoolManager poolManager;
    protected PlayerMove playerMove;
    protected TargetScanner targetScanner;
    private PlayerData playerStatMultipliers;

    protected virtual void Awake()
    {
        gm = GameManager.instance;
        poolManager = PoolManager.instance;
        playerMove = gm.player.GetComponent<PlayerMove>();
        targetScanner = gm.player.scanner;
        playerStatMultipliers = gm.playerData;
    }

    protected virtual void Update()
    {
        if (GlobalManager.instance.gameState != GameState.Playing || finalStats.weaponAttackSpeed < 0)
            return;

        UpdateTimer();
    }

    protected virtual void OnEnable()
    {
        StopAllCoroutines();
        isAttacking = false;

        if (finalStats != null)
            speedTimer = finalStats.weaponAttackSpeed;
    }

    // 초기 설정 함수
    public virtual void Init(BulletData originalData)
    {
        // # 공통 초기화 로직        
        // # 기본 속성 세팅

        currentBulletData = Instantiate(originalData);   // 값 복사
        finalStats = new WeaponStats();
        RecalculateStats();                        // 최종 스탯 계산
        level = 1;
        speedTimer = finalStats.weaponAttackSpeed; // 타이머 초기화
        transform.localPosition = Vector3.zero;    // 플레이어 안에서 위치 초기화
        ownerTag = transform.parent.tag;           // 무기 소유자 태그 설정

    }

    public void RecalculateStats()
    {
        if (currentBulletData == null || playerStatMultipliers == null) return;

        // '현재 기본 스탯(currentData)'과 '플레이어 배율'을 곱해서 '최종 스탯(finalStats)'을 계산
        finalStats.damage = currentBulletData.baseDamage * playerStatMultipliers.damageMult;
        finalStats.attackRange = currentBulletData.baseRange * playerStatMultipliers.attackRangeMult;
        finalStats.bulletSize = currentBulletData.baseScale * playerStatMultipliers.attackRangeMult;

        // 공격속도 관련 (배율이 높을수록 수치가 작아져야 함 -> 나누기)
        finalStats.weaponAttackSpeed = currentBulletData.baseWeaponAttackSpeed / playerStatMultipliers.attackSpeedMult;
        finalStats.damageInterval = currentBulletData.baseDamageInterval / playerStatMultipliers.attackSpeedMult;
        finalStats.bulletDelay = currentBulletData.baseDelay / playerStatMultipliers.attackSpeedMult;

        // 공격속도와 정비례하는 값들 (배율이 높을수록 수치가 커져야 함 -> 곱하기)
        finalStats.rotationSpeed = currentBulletData.baseRotationSpeed * playerStatMultipliers.attackSpeedMult;
        finalStats.weaponDuration = currentBulletData.baseWeaponDuration * playerStatMultipliers.attackSpeedMult;

        // 배율의 영향을 받지 않는 값들은 그냥 복사
        finalStats.count = currentBulletData.baseCount;
        finalStats.per = currentBulletData.basePer;
        finalStats.bulletMoveSpeed = currentBulletData.baseBulletMoveSpeed;
        finalStats.knockBackAmout = currentBulletData.baseKnockback;
    }

    public virtual void WeaponLevelUp(float rate, int rateIndex, int currentLevel)
    {
        // 레벨업은 '현재 기본 스탯(currentData)'의 값을 직접 변경
        IBatchable batchable = this as IBatchable;
        switch (rateIndex)
        {
            case 0: // 데미지 증가
                currentBulletData.baseDamage += rate;
                break;
            case 1: // 개수 증가
                currentBulletData.baseCount += (int)rate;
                batchable?.Batch(); // 개수가 바뀌었으니 재배치
                break;
            case 2: // 관통력 증가
                currentBulletData.basePer += (int)rate;
                break;
            case 3: // 크기/범위 증가
                currentBulletData.baseScale += (currentBulletData.baseScale * rate * 0.01f);
                currentBulletData.baseRange += (currentBulletData.baseRange * rate * 0.01f);
                batchable?.Batch(); // 크기가 바뀌었으니 재배치
                break;
        }

        // 기본 스탯이 바뀌었으니, 최종 스탯을 다시 계산
        RecalculateStats();
        level = currentLevel;
    }

    protected virtual void UpdateTimer()
    {
        if (isAttacking)
            return;

        speedTimer += Time.deltaTime;

        if (speedTimer >= finalStats.weaponAttackSpeed)
        {
            speedTimer = 0f;
            isAttacking = true;
            Attack();
        }

    }

    // 공격기능
    public abstract void Attack();

    protected virtual void BulletInit(Transform bullet, Vector3? dir = null)
    {
        Vector3 direction = dir ?? Vector3.zero;
        Bullet bulletComponent = bullet.GetComponent<Bullet>();

        // 총알 초기화 시, 최종 계산된 'finalStats'를 사용
        bulletComponent.Init(direction, transform.parent.tag,
                             finalStats.per, finalStats.damage,
                             finalStats.bulletMoveSpeed, finalStats.knockBackAmout,
                             finalStats.damageInterval);

        // 총알 크기도 최종 크기로 설정
        //bullet.localScale = finalStats.bulletSize;
    }   
}
