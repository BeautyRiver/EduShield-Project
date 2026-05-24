using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponStats
{
    public float damage;                // 무기 데미지
    public float count;                 // 무기 개수
    public float per;                   // 관통력
    public float bulletMoveSpeed;       // 총알 이동 속도
    public float weaponAttackSpeed;     // 무기 쿨다운(버스트 사이 간격)
    public float bulletDelay;           // 한 버스트 내 총알 사이 딜레이
    public float damageInterval;        // 도트 데미지 줄 수 있는 텀
    public float knockBackAmout;        // 몬스터 넉백량
    public Vector3 bulletSize;          // 총알(무기) 크기
}

public abstract class Weapon : MonoBehaviour
{
    // 공속 캡. 이보다 빠른 쿨다운은 허용 안 함 (무한 발사 방지).
    private const float MinWeaponAttackSpeed = 0.1f;

    // burst 내 발사 간격 캡. base가 0이면 동시 발사 의도로 보고 0 유지.
    private const float MinBulletDelay = 0.05f;

    public string ownerTag;      // 무기 소유자 태그
    public int level = 0;        // 현재 레벨

    [Header("# 무기 데이터")]
    // 원본 SO 참조. 절대 mutating 금지. 레벨업 보너스는 아래 bonus 필드에 누적.
    public BulletData currentBulletData;

    [Header("# Live Stats (Read Only)")]
    public WeaponStats finalStats;

    [Header("# 레벨업 누적 보너스 (Read Only)")]
    public float damageBonus;
    public float countBonus;
    public float perBonus;
    public Vector3 scaleBonus;

    [Header("공격 쿨타임/타이머")]
    public bool isAttacking;
    public float speedTimer;

    [Header("참조")]
    protected GameManager gm;
    protected PoolManager poolManager;
    protected PlayerMove playerMove;
    protected TargetScanner targetScanner;

    protected virtual void Awake()
    {
        gm = GameManager.instance;
        poolManager = PoolManager.instance;
        playerMove = gm.player.GetComponent<PlayerMove>();
        targetScanner = gm.player.enemyScanner;
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
        // 원본 SO 참조만 보관. 클론하지 않는다 = 원본을 절대 mutating 하지 않음.
        currentBulletData = originalData;
        finalStats = new WeaponStats();

        // 새 무기는 보너스 0부터 시작
        damageBonus = 0f;
        countBonus = 0f;
        perBonus = 0f;
        scaleBonus = Vector3.zero;

        RecalculateStats();
        level = 1;
        speedTimer = finalStats.weaponAttackSpeed;
        transform.localPosition = Vector3.zero;
        ownerTag = transform.root.tag;
    }

    public virtual void RecalculateStats()
    {
        if (currentBulletData == null) return;

        PlayerData pData = gm.playerData;

        // final = (base + levelUpBonus) × playerMultiplier
        finalStats.damage = (currentBulletData.baseDamage + damageBonus) * pData.damageMult;
        finalStats.count = currentBulletData.baseCount + countBonus;
        finalStats.per = currentBulletData.basePer + perBonus;

        // 크기는 sizeMult로 스케일 (attackRangeMult는 사거리/궤도 전용)
        Vector3 effectiveScale = currentBulletData.baseScale + scaleBonus;
        finalStats.bulletSize = effectiveScale * pData.sizeMult;

        // 공격속도 (공속이 높을수록 쿨타임은 줄어듬). 최소 0.1초로 캡 → 무한 발사 방지.
        finalStats.weaponAttackSpeed = Mathf.Max(MinWeaponAttackSpeed, currentBulletData.baseWeaponAttackSpeed / pData.attackSpeedMult);
        finalStats.damageInterval = currentBulletData.baseDamageInterval / pData.attackSpeedMult;

        // baseDelay == 0이면 동시 발사(샷건) 의도 → 캡 없이 0 유지.
        // > 0이면 순차 발사 의도 → 최소 캡 적용.
        finalStats.bulletDelay = currentBulletData.baseDelay > 0f
            ? Mathf.Max(MinBulletDelay, currentBulletData.baseDelay / pData.attackSpeedMult)
            : 0f;

        finalStats.bulletMoveSpeed = currentBulletData.baseBulletMoveSpeed;
        finalStats.knockBackAmout = currentBulletData.baseKnockback;
    }

    // 무한 성장 레벨업: 원본 SO는 그대로 두고, bonus 필드에만 누적
    public virtual void LevelUp(Rarity rarity, List<StatType> selectedStats)
    {
        float multiplier = Utils.GetRarityMultiplier(rarity);

        foreach (StatType stat in selectedStats)
        {
            switch (stat)
            {
                case StatType.Damage:
                    damageBonus += currentBulletData.damageGrowth * multiplier;
                    break;
                case StatType.Count:
                    countBonus += currentBulletData.countGrowth * multiplier;
                    break;
                case StatType.Per:
                    perBonus += currentBulletData.perGrowth * multiplier;
                    break;
                case StatType.Size:
                    float scaleAdd = currentBulletData.scaleGrowth * multiplier;
                    scaleBonus += new Vector3(scaleAdd, scaleAdd, 0);
                    break;
            }
        }

        RecalculateStats();
        level++;
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

        // ownerTag는 Init에서 transform.root.tag(플레이어)로 이미 세팅됨.
        // 과거 transform.parent.tag는 중간 컨테이너(weaponObject)의 태그를 잡아 잘못된 값이었음.
        bulletComponent.Init(direction, ownerTag,
                             finalStats.per, finalStats.damage,
                             finalStats.bulletMoveSpeed, finalStats.knockBackAmout,
                             finalStats.damageInterval);
    }
}
