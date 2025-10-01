using System;
using UnityEditor;
using UnityEngine;
using VInspector;


public abstract class Weapon : MonoBehaviour
{    
    public string ownerTag;      // 무기 소유자 태그
    public int level = 0;        // 현재 레벨

    [Header("# Live Stats (Read Only)")]
    [SerializeField] protected BulletData currentData;
    public WeaponStats finalStats;

    [Header("공격 쿨타임/타이머")]
    public bool isAttacking;
    public float speedTimer;

    [Header("참조")]
    protected GameManager gameManager;
    protected PoolManager poolManager;
    protected PlayerMove playerMove;
    protected TargetScanner targetScanner;
    private PlayerData playerStatMultipliers;

    protected virtual void Awake()
    {
        gameManager = GameManager.instance;
        poolManager = PoolManager.instance;
        playerMove = gameManager.player.playerMove;
        targetScanner = gameManager.player.GetComponent<TargetScanner>();
        playerStatMultipliers = gameManager.playerData;
    }

    protected virtual void Update()
    {
        if (!gameManager.isGameActive || finalStats.weaponAttackSpeed < 0)
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

        currentData = Instantiate(originalData);   // 값 복사
        finalStats = new WeaponStats();
        RecalculateStats();                        // 최종 스탯 계산
        level = 1;
        speedTimer = finalStats.weaponAttackSpeed; // 타이머 초기화
        transform.localPosition = Vector3.zero;    // 플레이어 안에서 위치 초기화
        ownerTag = transform.parent.tag;           // 무기 소유자 태그 설정

       /* damage = data.baseDamage;                 // 기본 공격력
        bulletDelay = data.baseDelay;             // 기본 딜레이
        count = data.baseCount;                   // 기본 개수
        bulletMoveSpeed = data.baseBulletMoveSpeed;       // 기본 총알 이동속도
        weaponAttackSpeed = data.baseWeaponAttackSpeed;       // 기본 공격속도
        weaponDuration = data.baseWeaponDuration; // 무기 지속시간 설정  (*현재 ONLY 회전무기)
        rotationSpeed = data.baseRotationSpeed;   // 기본 회전속도       (*현재 ONLY 회전무기)
        attackRange = data.baseRange;             // 기본 범위 
        bulletSize = data.baseScale;              // 기본 사이즈 
        damageInterval = data.baseDamageInterval; // 기본 공격 텀  (*자기장 무기 때문)
        per = data.basePer;                       // 기본 관통력 
        knockBackAmout = data.baseKnockback;                    // 기본 넉벡량        
        bulletPrefab = data.bulletPrefab;

        // 플레이어의 기본 능력치에 따른 설정
        damage = data.baseDamage * gameManager.playerData.damageMult;
        attackRange = data.baseRange * gameManager.playerData.attackRangeMult;
        bulletSize = data.baseScale * gameManager.playerData.attackRangeMult;

        // 공격속도 설정
        damageInterval = data.baseDamageInterval * gameManager.playerData.attackSpeedMult;               // 데미지 간격
        weaponAttackSpeed = (float)System.Math.Round(weaponAttackSpeed / gameManager.playerData.attackSpeedMult, 2); // 무기 속도
        bulletDelay = (float)System.Math.Round(bulletDelay / gameManager.playerData.attackSpeedMult, 2); // 총알 사이 딜레이
        rotationSpeed = (float)System.Math.Round(rotationSpeed / gameManager.playerData.attackSpeedMult, 2); // 회전 속도*/

    }

    public void RecalculateStats()
    {
        if (currentData == null || playerStatMultipliers == null) return;

        // '현재 기본 스탯(currentData)'과 '플레이어 배율'을 곱해서 '최종 스탯(finalStats)'을 계산
        finalStats.damage = currentData.baseDamage * playerStatMultipliers.damageMult;
        finalStats.attackRange = currentData.baseRange * playerStatMultipliers.attackRangeMult;
        finalStats.bulletSize = currentData.baseScale * playerStatMultipliers.attackRangeMult;

        // 공격속도 관련 (배율이 높을수록 수치가 작아져야 함 -> 나누기)
        finalStats.weaponAttackSpeed = currentData.baseWeaponAttackSpeed / playerStatMultipliers.attackSpeedMult;
        finalStats.damageInterval = currentData.baseDamageInterval / playerStatMultipliers.attackSpeedMult;
        finalStats.bulletDelay = currentData.baseDelay / playerStatMultipliers.attackSpeedMult;

        // 공격속도와 정비례하는 값들 (배율이 높을수록 수치가 커져야 함 -> 곱하기)
        finalStats.rotationSpeed = currentData.baseRotationSpeed * playerStatMultipliers.attackSpeedMult;
        finalStats.weaponDuration = currentData.baseWeaponDuration * playerStatMultipliers.attackSpeedMult;

        // 배율의 영향을 받지 않는 값들은 그냥 복사
        finalStats.count = currentData.baseCount;
        finalStats.per = currentData.basePer;
        finalStats.bulletMoveSpeed = currentData.baseBulletMoveSpeed;
        finalStats.knockBackAmout = currentData.baseKnockback;
    }

    public virtual void WeaponLevelUp(float rate, int rateIndex, int currentLevel)
    {
        // 레벨업은 '현재 기본 스탯(currentData)'의 값을 직접 변경
        IBatchable batchable = this as IBatchable;
        switch (rateIndex)
        {
            case 0: // 데미지 증가
                currentData.baseDamage += rate;
                break;
            case 1: // 개수 증가
                currentData.baseCount += (int)rate;
                batchable?.Batch(); // 개수가 바뀌었으니 재배치
                break;
            case 2: // 관통력 증가
                currentData.basePer += (int)rate;
                break;
            case 3: // 크기/범위 증가
                currentData.baseScale += (currentData.baseScale * rate * 0.01f);
                currentData.baseRange += (currentData.baseRange * rate * 0.01f);
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
        bullet.localScale = finalStats.bulletSize;
    }
    [System.Serializable]
    public class WeaponStats
    {
        public float damage;                // 무기 데미지    
        public int count;                   // 무기 개수
        public int per;                     // 관통력
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
}
