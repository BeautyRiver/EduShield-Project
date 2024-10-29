using System;
using UnityEditor;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("# 무기 세팅")]
    public BulletData data;
    public int prefabId;         // 생성할 불릿의 프리팹 ID    
    public int level = 0;        // 현재 레벨
    public float damage;         // 무기 데미지    
    public int count;            // 무기 개수
    public int per;              // 관통력
    public float weaponSpeed;    // 무기 속도    
    private float rotationSpeed;
    public float bulletDelay;    // 총알 사이 딜레이 (Range)   
    public float damageInterval; // 데미지 줄 수 있는 텀
    public float durationTime;   // 지속 시간(회전 무기만 일단)
    public float attackRange;    // 공격 범위
    public float knockBackAmout; // 몬스터 넉백량
    public Vector3 bulletSize;   // 총알(무기) 크기

    [SerializeField] protected bool isAttacking;
    [SerializeField] protected float speedTimer;
    protected GameManager gm;
    protected Player player;

    protected virtual void Awake()
    {
        gm = GameManager.instance;
        player = gm.player;
    }

    protected virtual void Update()
    {
        if (!gm.isLive)
            return;

        UpdateTimer();
    }

    protected virtual void OnEnable()
    {
        StopAllCoroutines();
        isAttacking = false;
        speedTimer = weaponSpeed;
    }

    // 초기 설정 함수
    public virtual void Init(BulletData data)
    {
        // 공통 초기화 로직        
        // 기본 속성 세팅
        this.data = Instantiate(data);            // 값 복사                                                
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;   // 플레이어 안에서 위치 초기화

        prefabId = SetPrefabID(data);             // prefabID 설정
        durationTime = 3f;                        // 무기 지속시간 설정 (*현재 ONLY 회전무기)
        damage = data.baseDamage;                 // 기본 공격력
        bulletDelay = data.baseDelay;             // 기본 딜레이
        count = data.baseCount;                   // 기본 개수
        weaponSpeed = data.baseSpeed;             // 기본 공격속도
        rotationSpeed = data.baseRotationSpeed;   // 기본 회전속도 (*현재 ONLY 회전무기)
        attackRange = data.baseRange;             // 기본 범위 
        bulletSize = data.baseScale;              // 기본 사이즈 
        damageInterval = data.baseDamageInterval; // 기본 공격 텀  (*자기장 무기 때문)
        per = data.basePer;                       // 기본 관통력 
        knockBackAmout = 1.5f;                    // 기본 넉벡량        


        // 플레이어의 기본 능력치에 따른 설정
        damage = data.baseDamage * gm.playerData.damageMult;
        attackRange = data.baseRange * gm.playerData.atkRangeMult;
        bulletSize = data.baseScale * gm.playerData.atkRangeMult;        

        // 공격속도 설정
        damageInterval = data.baseDamageInterval * gm.playerData.atkSpeedMult;
        weaponSpeed = (float)System.Math.Round(weaponSpeed / gm.playerData.atkSpeedMult, 2);
        rotationSpeed = (float)System.Math.Round(rotationSpeed / gm.playerData.atkSpeedMult, 2);

        speedTimer = weaponSpeed;
        level++;
    }

    public virtual void WeaponLevelUp(float rate, int rateIndex, int currentLevel)
    {
        // 공통 레벨업 로직
        // 무기 업그레이드
        switch (rateIndex)
        {
            case 0: // 데미지 증가
                data.baseDamage += rate;
                damage = data.baseDamage * gm.playerData.damageMult;
                Debug.Log($"{this.name}: Damage {rate}만큼 증가했습니다.");
                break;

            case 1: // 카운트 증가
                count += (int)rate;
                Debug.Log($"{this.name}: Count {rate}만큼 증가했습니다.");
                break;

            case 2: // 관통력 증가
                per += (int)rate;
                Debug.Log($"{this.name}: Per {rate}만큼 증가했습니다.");
                break;

            case 3: // 크기[범위] 증가
                data.baseScale += (data.baseScale * rate * 0.01f);                
                bulletSize = data.baseScale * gm.playerData.atkRangeMult;


                attackRange = data.baseRange * gm.playerData.atkRangeMult;

                Debug.Log($"{this.name}: Range {rate}만큼 증가했습니다.");
                break;
        }
       
        LevelUpException(rateIndex);
        level = currentLevel;
    }

    protected virtual void UpdateTimer()
    {
        if (!isAttacking)
        {
            speedTimer += Time.deltaTime;

            if (speedTimer >= weaponSpeed)
            {
                speedTimer = 0f;
                isAttacking = true;
                Attack();
            }
        }
    }

    // 공격기능
    protected abstract void Attack();

    protected virtual void BulletInit(Transform bullet, Vector3? dir = null)
    {
        Vector3 direction = dir ?? Vector3.zero;
        Bullet bulletComponent = bullet.GetComponent<Bullet>();

        // 공통된 불릿 초기화 로직
        bulletComponent.Init(damage, per, direction, data.itemId, knockBackAmout, damageInterval);
    }

   

    // 프리펩 아이디 찾기
    protected int SetPrefabID(BulletData data)
    {
        for (int index = 0; index < GameManager.instance.pool.bulletPrefabs.Length; index++)
        {
            if (data.prefab == gm.pool.bulletPrefabs[index])
            {
                prefabId = index;
                Debug.Log($"PrefabID : [{prefabId}]");
                return index;
            }            
        }            
        return -1;
    }

    /// <summary>
    ///  LevelUp Logic: 레벨업 예외처리
    /// </summary>
    protected abstract void LevelUpException(int rateIndex);
}
