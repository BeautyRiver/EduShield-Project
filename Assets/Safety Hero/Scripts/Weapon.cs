using DarkTonic.MasterAudio;
using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    [Header("# 무기 세팅")]
    public ItemData data;
    public int prefabId; // 생성할 불릿의 프리팹 ID    
    public int level = 0; // 현재 레벨
    public float damage; // 무기 데미지    
    public int count; // 무기 개수
    public int per; // 관통력
    public float weaponSpeed; // 무기 속도    
    public float bulletDelay; // 총알 사이 딜레이 (Range)   
    public float damageInterval; // 데미지 간격
    public float durationTime = 3f; // 지속 시간(회전 무기만 일단)
    public float attackRange = 1.3f; // 공격 범위
    public Vector3 bulletSize;// 총알(무기) 크기
    private bool check; // 체크전용 변수
    [SerializeField] private bool isAttacking; // 공격중인지 체크
    [SerializeField] private float speedTimer; // 원거리 무기 타이머
    [SerializeField] private GameManager gm;
    [SerializeField] private Player player;

    private void Awake()
    {
        gm = GameManager.instance;
        player = gm.player;
    }

    private void Update()
    {
        if (!gm.isLive)
            return;

        switch (data.itemType)
        {
            case ItemData.ItemType.M1_Rotating: // 회전무기                                                               
                transform.Rotate(Vector3.back * bulletDelay * Time.deltaTime); // 무기 회전
                UpdateTimer();
                break;

            case ItemData.ItemType.M0_Default: // 가스
            case ItemData.ItemType.M2_MagneticField: // 가스
            case ItemData.ItemType.R0_TargetGun: // 단발총                   
            case ItemData.ItemType.R1_Cannon: // 대포                   
            case ItemData.ItemType.R2_Throw: // 창던지기
                UpdateTimer();
                break;
        }

    }
    private void OnEnable()
    {
        StopAllCoroutines();
        isAttacking = false;
        speedTimer = weaponSpeed - 0.01f;
    }
    // 초기 설정 함수
    public void Init(ItemData data)
    {
        // 기본 세팅
        this.data = Instantiate(data);  // 값 복사                                                
        gameObject.name = "Equip Weapon: " + data.itemType.ToString();
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero; // 플레이어 안에서 위치 초기화

        // 속성 세팅
        durationTime = 3f;
        bulletDelay = data.baseDelay; // 기본 딜레이 저장
        damage = data.baseDamage; // 기본 공격력 저장
        count = data.baseCount; // 기본 개수 설정
        weaponSpeed = data.baseSpeed; // 기본 공격속도 저장
        attackRange = data.baseRange; // 기본 범위 저장
        bulletSize = data.baseScale; // 기본 사이즈 저장
        damageInterval = data.baseDamageInterval;

        per = data.basePer; // 기본 관통력 설정
        for (int index = 0; index < GameManager.instance.pool.weaponPrefabs.Length; index++)
        {
            if (data.prefab == gm.pool.weaponPrefabs[index])
            {
                prefabId = index;
                Debug.Log($"PrefabID : [{prefabId}]");
                break;
            }
        }
        speedTimer = weaponSpeed - 0.01f;

        // 기본 데미지 설정
        damage = data.baseDamage * gm.playerData.damageMult;
        attackRange = data.baseRange * gm.playerData.atkRangeMult;
        bulletSize = data.baseScale * gm.playerData.atkRangeMult;
        damageInterval = data.baseDamageInterval * gm.playerData.atkSpeedMult;

        // 기본 공격 속도 설정
        switch (data.itemType)
        {
            case ItemData.ItemType.M1_Rotating: // 회전무기
                // 캐릭터별 무기 회전 속도 설정
                bulletDelay = (float)System.Math.Round(bulletDelay * gm.playerData.atkSpeedMult, 2);
                weaponSpeed = (float)System.Math.Round(weaponSpeed / gm.playerData.atkSpeedMult, 2);
                break;

            // 무기 딜레이
            case ItemData.ItemType.M0_Default:
            case ItemData.ItemType.M2_MagneticField:  
            case ItemData.ItemType.R0_TargetGun:
            case ItemData.ItemType.R1_Cannon:
            case ItemData.ItemType.R2_Throw:
                // 캐릭터별 무기 연사속도 설정
                weaponSpeed = (float)System.Math.Round(weaponSpeed / gm.playerData.atkSpeedMult, 2);
                break;
        }

        /*switch (data.itemType)
        {
            case ItemData.ItemType.M2_MagneticField:
                Attack();
                break;
        }*/

        /* Gear[] gears = transform.parent.GetComponentsInChildren<Gear>();
         if (gears != null)
         {
             foreach (Gear gear in gears)
             {
                 gear.rate = gear.accumulatedRate - 1;
                 gear.ApplyGearToWeapon(this);
             }
         }*/

        level++;
    }


    public void WeaonLevelUp(float rate, int rateIndex, int currentLevel)
    {
        // RaTE 업데이트
        switch (rateIndex)
        {
            case 0: // 데미지 증가
                data.baseDamage += rate;
                damage = data.baseDamage * gm.playerData.damageMult;
                Debug.Log($"{this.name}: Damage {rate}만큼 증가했습니다.");
                break;

            case 1: // 카운트 증가
                count += (int)rate;
                // 회전 무기는 다시 자연스럽게 추가시키기 위해서 재배치
                if (data.itemType == ItemData.ItemType.M1_Rotating)
                    M1_Batch();
                Debug.Log($"{this.name}: Count {rate}만큼 증가했습니다.");
                break;

            case 2: // 관통력 증가
                per += (int)rate;
                Debug.Log($"{this.name}: Per {rate}만큼 증가했습니다.");
                break;

            case 3: // 크기[범위] 증가
                data.baseScale += data.baseScale * rate * 0.01f;
                switch (data.itemType)
                {
                    case ItemData.ItemType.M1_Rotating: // 회전 무기
                        bulletSize = data.baseScale * gm.playerData.atkRangeMult;
                        attackRange = data.baseRange * gm.playerData.atkRangeMult;
                        M1_Batch();
                        break;
                    case ItemData.ItemType.M2_MagneticField:
                        bulletSize = data.baseScale * gm.playerData.atkRangeMult;
                        M2_Batch(transform.GetChild(0));
                        break;
                    default:
                        bulletSize = data.baseScale * gm.playerData.atkRangeMult;
                        break;
                }
                Debug.Log($"{this.name}: Range {rate}만큼 증가했습니다.");

                break;
        }

        level = currentLevel;
    } 

    private void UpdateTimer()
    {
        if (!isAttacking)
        {
            speedTimer += Time.deltaTime;

            if (speedTimer >= weaponSpeed)
            {
                bool shouldAttack = data.itemType != ItemData.ItemType.R0_TargetGun ||
                                    player.scanner.nearestTarget != null;

                if (shouldAttack)
                {
                    speedTimer = 0f;
                    isAttacking = true;
                    Attack();
                }
            }
        }
    }

    private void Attack()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.M0_Default:
                StartCoroutine(M0_Bullet());
                break;
            case ItemData.ItemType.M1_Rotating:
                StartCoroutine(M1_Bullet());
                break;
            case ItemData.ItemType.M2_MagneticField:
                StartCoroutine(M2_Bullet());
                break;
            case ItemData.ItemType.R0_TargetGun:
                StartCoroutine(R0_Bullet());
                break;
            case ItemData.ItemType.R1_Cannon:
                StartCoroutine(R1_Bullet());
                break;
            case ItemData.ItemType.R2_Throw:
                StartCoroutine(R2_Bullet());
                break;
        }
    }
    private IEnumerator M0_Bullet()
    {
        // 첫 번째 공격은 플레이어가 바라보는 방향, 두 번째는 반대 방향으로 발사
        for (int i = 0; i < count; i++)
        {
            // 첫 번째 발사 방향: 플레이어가 바라보는 방향
            Vector3 dir = (i % 2 == 0) ? new Vector3(player.lastXInputVec, 0, 0).normalized : new Vector3(-player.lastXInputVec, 0, 0).normalized;

            // 새로운 발사체 생성
            Transform bullet = GameManager.instance.pool.Get(PoolManager.PoolType.Weapon, prefabId).transform;
            bullet.parent = transform;

            // 발사체 위치 설정 (약간의 높이 차이 추가)
            bullet.localScale = bulletSize;
            bullet.position = transform.position + new Vector3(0, i * 1f, 0); // 무기 개수에 따라 높이 증가
            bullet.Translate(bullet.right * dir.x * 0.2f); // 지정된 거리만큼 이동


            // 발사 방향에 따라 발사체 회전 설정 (왼쪽으로 발사될 때는 180도 회전)
            if (dir.x < 0)
                bullet.localRotation = Quaternion.Euler(0, 180, 0); // 왼쪽을 바라보게 회전

            else
                bullet.localRotation = Quaternion.identity; // 오른쪽을 기본 방향으로 유지

            // 발사체 초기화
            BulletInit(bullet);
            MasterAudio.PlaySound("M0_Default");
            // 발사 후 딜레이 추가
            yield return new WaitForSeconds(bulletDelay);  // 각 공격 사이의 딜레이 설정
        }

        // 공격이 끝나면 상태 초기화
        isAttacking = false;
        //AudioManager.instance.PlaySfx(AudioManager.Sfx.Range); // 공격 사운드 재생
    }
    // 회전 무기
    private IEnumerator M1_Bullet()
    {
        M1_Batch();
        yield return new WaitForSeconds(durationTime);
        Transform bullet;
        for (int index = 0; index < count; index++) // 불릿 수만큼 반복
        {
            bullet = transform.GetChild(index); // 기존 자식 사용
            bullet.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBounce); // 크기를 0.5초 동안 자연스럽게 축소
        }
        isAttacking = false;
    }

    // 자기장
    private IEnumerator M2_Bullet()
    {
        while (!gm.isGameRealEnd)
        {
            yield return null;
            Transform bullet;
            if (transform.childCount <= 0)
            {
                bullet = gm.pool.Get(PoolManager.PoolType.Weapon, prefabId).transform;
                bullet.parent = transform; // 부모 설정
                bullet.localPosition = Vector3.zero; // 로컬 위치 초기화
                bullet.localRotation = Quaternion.identity; // 로컬 회전 초기화
                bullet.localScale = Vector3.zero; // 로컬 크기 0으로 초기화
                M2_Batch(bullet);
            }
            else
            {
                bullet = transform.GetChild(0);
            }
            BulletInit(bullet);
        }        
    }

    public void M2_Batch(Transform bullet)
    {
        bullet.DOScale(bulletSize, 0.5f).SetEase(Ease.OutBack);
    }

    // 총
    private IEnumerator R0_Bullet()
    {           
        for (int i = 0; i < count; i++)
        {            
            if (player.scanner.nearestTarget == null)
                yield break;

            Vector3 targetPos = player.scanner.nearestTarget.position;
            Vector3 dir = (targetPos - transform.position).normalized;

            // 총알 발사
            Transform bullet = GameManager.instance.pool.Get(PoolManager.PoolType.Weapon, prefabId).transform;
            bullet.parent = transform;

            bullet.localScale = bulletSize;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);

            // 불렛 초기화
            BulletInit(bullet, dir);

            // 발사 사운드
            MasterAudio.PlaySound("R0_TargetGun");

            // 발사 후 약간의 딜레이 추가
            yield return new WaitForSeconds(bulletDelay); // 총알 사이의 딜레이 설정 (0.1초, 필요에 따라 조정 가능)
        }
        isAttacking = false;
        //AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
    // 대포
    private IEnumerator R1_Bullet()
    {
        bool isReverse = false;
        for (int i = 0; i < count; i++)
        {
            // isReverse 플래그에 따라 발사 방향 결정 (true면 정방향, false면 반대 방향)
            Vector3 dir = isReverse ? new Vector3(player.lastInputVec.x, player.lastInputVec.y, 0).normalized : new Vector3(-player.lastInputVec.x, -player.lastInputVec.y, 0).normalized;


            Transform bullet = GameManager.instance.pool.Get(PoolManager.PoolType.Weapon, prefabId).transform;
            bullet.parent = transform;
            Vector3 spreadOffset = Vector3.zero;

            float random = Random.Range(-4, 5) * 0.2f;
            // 발사 방향에 따라 발사체 간격을 조절 (오른쪽/왼쪽, 위쪽/아래쪽 모두 지원)
            spreadOffset = Vector3.Cross(dir, Vector3.forward) * ((i - (count / 2)) * random);

            // 발사체의 시작 위치를 조정
            Vector3 startPosition = transform.position + spreadOffset;

            bullet.localScale = bulletSize;
            bullet.position = startPosition;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);  // 발사 방향에 맞게 회전 설정

            // 불렛 초기화
            BulletInit(bullet, dir);

            // 발사 사운드
            MasterAudio.PlaySound("R1_Cannon");

            // 발사 후 약간의 딜레이 추가
            yield return new WaitForSeconds(bulletDelay);  // 총알 사이의 딜레이 설정 (0.1초)        

            isReverse = !isReverse; // 매번 방향을 반대로 변경
        }

        isAttacking = false;       
    }

    // 창
    private IEnumerator R2_Bullet()
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 dir = new Vector3(player.lastInputVec.x, player.lastInputVec.y, 0).normalized;
            Transform bullet = GameManager.instance.pool.Get(PoolManager.PoolType.Weapon, prefabId).transform;
            bullet.parent = transform;
            Vector3 spreadOffset = Vector3.zero;

            float random = Random.Range(-4, 5) * 0.05f;
            // 발사 방향에 따라 발사체 간격을 조절 (오른쪽/왼쪽, 위쪽/아래쪽 모두 지원)
            spreadOffset = Vector3.Cross(dir, Vector3.forward) * ((i - (count / 2)) * random);


            // 발사체의 시작 위치를 조정
            Vector3 startPosition = transform.position + spreadOffset;

            bullet.localScale = bulletSize;
            bullet.position = startPosition;
            //bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);  // 발사 방향에 맞게 회전 설정
            bullet.rotation = Quaternion.Euler(new Vector3(bullet.transform.eulerAngles.x, bullet.transform.eulerAngles.y, Random.Range(0, 360f)));
            
            //불렛 초기화
            BulletInit(bullet, dir);            

            // 발사 사운드
            MasterAudio.PlaySound("R2_Throw");

            // 발사 후 약간의 딜레이 추가
            yield return new WaitForSeconds(bulletDelay);  // 총알 사이의 딜레이 설정 (0.1초)
        }
        isAttacking = false;
    }
    // 불릿 배치 함수 (회전 무기)
    public void M1_Batch()
    {
        for (int index = 0; index < count; index++) // 불릿 수만큼 반복
        {
            Transform bullet;
            if (index < transform.childCount) // 자식 존재 시
            {
                bullet = transform.GetChild(index); // 기존 자식 사용
            }
            else
            {
                bullet = gm.pool.Get(PoolManager.PoolType.Weapon, prefabId).transform;
                bullet.parent = transform; // 부모 설정
            }

            bullet.localPosition = Vector3.zero; // 로컬 위치 초기화
            bullet.localRotation = Quaternion.identity; // 로컬 회전 초기화

            // 초기 회전 설정
            Vector3 rotVec = Vector3.forward * 360 * index / count; // 불릿 회전 벡터 계산
            bullet.Rotate(rotVec); // 불릿 회전                        
            bullet.localScale = Vector3.zero;
            bullet.Translate(bullet.up * attackRange, Space.World); // 지정된 거리만큼 이동               
            bullet.DOScale(bulletSize, 0.5f).SetEase(Ease.OutBounce); // 크기를 0.5초 동안 자연스럽게 확장

            BulletInit(bullet);
        }
    }
 
    /// <summary>
    /// 불렛  초기화 함수
    /// </summary>
    /// <param name="bul"></param>
    /// <param name="dir"></param>
    private void BulletInit(Transform bul,  Vector3? dir = null)
    {
        Vector3 direction = dir ?? Vector3.zero;
        Bullet bullet = bul.GetComponent<Bullet>();
        switch (data.itemType)
        {
            case ItemData.ItemType.M0_Default:
                bullet.Init(damage, per, direction, data.itemId);
                break;

            case ItemData.ItemType.M1_Rotating:
                bullet.Init(damage, per, direction, data.itemId, 1.5f, 0.5f); // 불릿 초기화 (데미지 설정 및 관통 설정 -100은 무한 관통)
                break;

            case ItemData.ItemType.M2_MagneticField:
                bullet.Init(damage, per, direction, data.itemId, 0, damageInterval);
                break;

            case ItemData.ItemType.R0_TargetGun:
            case ItemData.ItemType.R1_Cannon:
            case ItemData.ItemType.R2_Throw:
                bullet.Init(damage, per, direction, data.itemId, 1.5f, 0.2f);
                break;
        }
    }
}
