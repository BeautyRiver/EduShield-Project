using System;
using System.Collections.Generic;
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
    public BulletData currentBulletData;
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
        targetScanner = gm.player.enemyScanner;
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
        ownerTag = transform.root.tag;           // 무기 소유자 태그 설정

    }

    public void RecalculateStats()
    {
        if (currentBulletData == null) return;

        // PlayerData(gm.playerData)가 없으면 기본값 1로 처리하는 안전장치 추가 추천
        PlayerData pData = gm.playerData;

        // [변경] float 연산으로 계산
        finalStats.damage = currentBulletData.baseDamage * pData.damageMult;

        // 개수와 관통력도 이제 float입니다! (플레이어 스탯에 개수/관통력 배율은 보통 없으므로 기본값 사용)
        finalStats.count = currentBulletData.baseCount;
        finalStats.per = currentBulletData.basePer;

        // 크기
        finalStats.bulletSize = currentBulletData.baseScale * pData.attackRangeMult; // 범위 배율을 크기에도 적용한다고 가정
        finalStats.attackRange = currentBulletData.baseRange * pData.attackRangeMult;

        // 공격속도 (공속이 높을수록 쿨타임은 줄어듬)
        finalStats.weaponAttackSpeed = currentBulletData.baseWeaponAttackSpeed / pData.attackSpeedMult;
        finalStats.damageInterval = currentBulletData.baseDamageInterval / pData.attackSpeedMult;
        finalStats.bulletDelay = currentBulletData.baseDelay / pData.attackSpeedMult;

        // 기타
        finalStats.rotationSpeed = currentBulletData.baseRotationSpeed * pData.attackSpeedMult;
        finalStats.weaponDuration = currentBulletData.baseWeaponDuration * pData.attackSpeedMult;
        finalStats.bulletMoveSpeed = currentBulletData.baseBulletMoveSpeed;
        finalStats.knockBackAmout = currentBulletData.baseKnockback;
    }

    // [신규] 무한 성장 레벨업 함수
    public virtual void LevelUp(Rarity rarity, List<StatType> selectedStats)
    {        
        // 1. 유틸에서 배율 가져오기
        float multiplier = Utils.GetRarityMultiplier(rarity);

        foreach (StatType stat in selectedStats)
        {
            switch (stat)
            {
                case StatType.Damage:
                    currentBulletData.baseDamage += currentBulletData.damageGrowth * multiplier;
                    break;
                case StatType.Count:
                    currentBulletData.baseCount += currentBulletData.countGrowth * multiplier;
                    break;
                case StatType.Per:
                    currentBulletData.basePer += currentBulletData.perGrowth * multiplier;
                    break;
                case StatType.Size:
                    float scaleAdd = currentBulletData.scaleGrowth * multiplier;
                    currentBulletData.baseScale += new Vector3(scaleAdd, scaleAdd, 0);
                    break;
            }
        }

        // 3. 스탯 재계산
        RecalculateStats();

        // 4. 레벨 표기 증가
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

        // 총알 초기화 시, 최종 계산된 'finalStats'를 사용
        bulletComponent.Init(direction, transform.parent.tag,
                             finalStats.per, finalStats.damage,
                             finalStats.bulletMoveSpeed, finalStats.knockBackAmout,
                             finalStats.damageInterval);

        // 총알 크기도 최종 크기로 설정
        //bullet.localScale = finalStats.bulletSize;
    }   
}
