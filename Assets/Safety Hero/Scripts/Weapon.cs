using DarkTonic.MasterAudio;
using DG.Tweening;
using System.Collections;
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

    public float attackRange = 1.3f; // 공격 범위
    public Vector3 bulletSize;// 총알(무기) 크기

    public float buletDelay; // 총알 사이 딜레이 (Range)   
    public float weaponSpeed; // 무기 속도    
    public float roationTime = 3f;
    [SerializeField] private bool isAttacking; // 공격중인지 체크
    [SerializeField] private float speedTimer; // 원거리 무기 타이머
    private GameManager gameManager;
    private Player player;

    private void Awake()
    {
        gameManager = GameManager.instance;
        player = gameManager.player;
    }
    private void Update()
    {
        if (gameManager.isLive)
        {
            switch (data.itemType)
            {
                case ItemData.ItemType.MWeapon_1: // 회전무기                                               
                    transform.Rotate(Vector3.back * buletDelay * Time.deltaTime); // 무기 회전
                    UpdateTimer(); 
                    break;

                case ItemData.ItemType.MWeapon_0: // 가스
                case ItemData.ItemType.RWeapon_0: // 단발총                   
                case ItemData.ItemType.RWeapon_1: // 대포                   
                case ItemData.ItemType.RWeapon_2: // 창던지기
                    UpdateTimer();
                    break;
            }
        }
    }
    // 초기 설정 함수
    public void Init(ItemData data)
    {
        // 기본 세팅
        this.data = data;
        gameObject.name = "Equip Weapon: " + data.itemType.ToString();
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero; // 플레이어 안에서 위치 초기화

        // 속성 세팅
        buletDelay = data.baseDelay; // 기본 딜레이 저장
        bulletSize = data.baseScale; // 기본 사이즈 저장
        weaponSpeed = data.baseSpeed; // 기본 공격속도 저장
        damage = data.baseDamage; // 기본 공격력 저장
        count = data.baseCount; // 기본 개수 설정
        per = data.basePer; // 기본 관통력 설정
        for (int index = 0; index < GameManager.instance.pool.weaponPrefabs.Length; index++)
        {
            if (data.prefab == gameManager.pool.weaponPrefabs[index])
            {
                prefabId = index;
                break;
            }
        }
        // 기본 데미지 설정
        damage *= gameManager.playerData.damageMult;
        speedTimer = weaponSpeed;

        // 기본 공격 속도 설정
        switch (data.itemType)
        {
            case ItemData.ItemType.MWeapon_1: // 삽
                // 캐릭터별 무기 회전 속도 설정
                buletDelay = (float)System.Math.Round(buletDelay * gameManager.playerData.atkSpeedMult, 2);
                //Batch(); // 회전 무기 배치
                break;

            // 무기 딜레이
            case ItemData.ItemType.MWeapon_0:
            case ItemData.ItemType.RWeapon_0:
            case ItemData.ItemType.RWeapon_1:
            case ItemData.ItemType.RWeapon_2:
                // 캐릭터별 무기 연사속도 설정
                weaponSpeed = (float)System.Math.Round(weaponSpeed / gameManager.playerData.atkSpeedMult, 2);
                break;
        }

        Gear[] gears = transform.parent.GetComponentsInChildren<Gear>();
        if (gears != null)
        {
            foreach (Gear gear in gears)
            {
                gear.rate = gear.accumulatedRate - 1;
                gear.ApplyGearToWeapon(this);
            }
        }

        level++;
        /* // 손 무기 세팅
         Hand hand = player.hands[(int)data.itemType];
         hand.spriter.sprite = data.hand;
         hand.gameObject.SetActive(true);*/
    }


    public void WeaonLevelUp(float rate, int rateIndex, int currentLevel)
    {
        // RaTE 업데이트
        switch (rateIndex)
        {
            case 0:
                damage += rate;
                Debug.Log($"{this.name}: Damage {rate}만큼 증가했습니다.");
                break;
            case 1:
                count += (int)rate;
                // 회전 무기는 다시 자연스럽게 추가시키기 위해서 재배치
                if (data.itemType == ItemData.ItemType.MWeapon_1)
                    Batch();
                Debug.Log($"{this.name}: Count {rate}만큼 증가했습니다.");

                break;
            case 2:
                per += (int)rate;
                Debug.Log($"{this.name}: Per {rate}만큼 증가했습니다.");

                break;
        }
        level = currentLevel;
    }

    // 불릿 배치 함수 (회전 무기)
    private void Batch()
    {
        for (int index = 0; index < count; index++) // 불릿 수만큼 반복
        {
            Transform bullet;
            if (index < transform.childCount) // 자식 수보다 인덱스가 작으면
            {
                bullet = transform.GetChild(index); // 기존 자식 사용
            }
            else
            {
                bullet = gameManager.pool.Get(PoolManager.PoolType.Weapon, prefabId).transform;
                bullet.parent = transform; // 부모 설정
            }

            bullet.localPosition = Vector3.zero; // 로컬 위치 초기화
            bullet.localRotation = Quaternion.identity; // 로컬 회전 초기화

            // 초기 회전 설정
            Vector3 rotVec = Vector3.forward * 360 * index / count; // 불릿 회전 벡터 계산
            bullet.Rotate(rotVec); // 불릿 회전                        
            bullet.localScale = Vector3.zero;
            bullet.Translate(bullet.up * attackRange * 1.5f, Space.World); // 지정된 거리만큼 이동
            bullet.DOScale(bulletSize, 0.5f).SetEase(Ease.OutBounce); // 크기를 0.5초 동안 자연스럽게 확장
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero, data.itemId); // 불릿 초기화 (데미지 설정 및 관통 설정 -100은 무한 관통)
        }
    }

    private void UpdateTimer()
    {
        if (!isAttacking)
        {
            speedTimer += Time.deltaTime;

            if (speedTimer >= weaponSpeed)
            {
                bool shouldAttack = data.itemType != ItemData.ItemType.RWeapon_0 ||
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
            case ItemData.ItemType.MWeapon_0:
                StartCoroutine(Melee_00());
                break;
            case ItemData.ItemType.MWeapon_1:
                StartCoroutine(Melee_01());
                break;
            case ItemData.ItemType.RWeapon_0:
                StartCoroutine(FireAuto());
                break;
            case ItemData.ItemType.RWeapon_1:
                StartCoroutine(FireDir_00());
                break;
            case ItemData.ItemType.RWeapon_2:
                StartCoroutine(FireDir_01());
                break;
        }
    }
    private IEnumerator Melee_00()
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
            bullet.GetComponent<Bullet>().Init(damage, per, Vector3.zero, data.itemId);
            MasterAudio.PlaySound("Weapon0");
            // 발사 후 딜레이 추가
            yield return new WaitForSeconds(buletDelay);  // 각 공격 사이의 딜레이 설정
        }

        // 공격이 끝나면 상태 초기화
        isAttacking = false;
        //AudioManager.instance.PlaySfx(AudioManager.Sfx.Range); // 공격 사운드 재생
    }
    private IEnumerator Melee_01()
    {
        Debug.Log("Melee_01 접속");
        Batch();
        yield return new WaitForSeconds(roationTime);
        for (int index = 0; index < count; index++) // 불릿 수만큼 반복
        {
            Transform bullet;
            if (index < transform.childCount) // 자식 수보다 인덱스가 작으면
            {
                bullet = transform.GetChild(index); // 기존 자식 사용
            }
            else
            {
                bullet = gameManager.pool.Get(PoolManager.PoolType.Weapon, prefabId).transform;
                bullet.parent = transform; // 부모 설정
            }

            bullet.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBounce); // 크기를 0.5초 동안 자연스럽게 축소
        }
        isAttacking = false;
    }
    private IEnumerator FireAuto()
    {
        if (player.scanner.nearestTarget == null)
            yield break;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;

        for (int i = 0; i < count; i++)
        {
            // 총알 발사
            Transform bullet = GameManager.instance.pool.Get(PoolManager.PoolType.Weapon, prefabId).transform;
            bullet.parent = transform;

            bullet.localScale = bulletSize;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            bullet.GetComponent<Bullet>().Init(damage, per, dir, data.itemId);

            // 발사 사운드
            MasterAudio.PlaySound("Weapon50");

            // 발사 후 약간의 딜레이 추가
            yield return new WaitForSeconds(buletDelay); // 총알 사이의 딜레이 설정 (0.1초, 필요에 따라 조정 가능)
        }
        isAttacking = false;
        //AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
    // 대포
    private IEnumerator FireDir_00()
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 dir = new Vector3(-player.lastInputVec.x, -player.lastInputVec.y, 0).normalized; // 플레이어 반대 방향으로 발사
            Transform bullet = GameManager.instance.pool.Get(PoolManager.PoolType.Weapon, prefabId).transform;
            bullet.parent = transform;

            // 발사체의 시작 위치를 조정
            Vector3 startPosition = transform.position + dir * 1.5f;
            bullet.localScale = bulletSize;
            bullet.position = startPosition;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);  // 발사 방향에 맞게 회전 설정

            bullet.GetComponent<Bullet>().Init(damage, per, dir, data.itemId);

            // 발사 사운드
            MasterAudio.PlaySound("Weapon51");

            // 발사 후 약간의 딜레이 추가
            yield return new WaitForSeconds(buletDelay);  // 총알 사이의 딜레이 설정 (0.1초)
        }
        isAttacking = false;
        //AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }

    // 창
    private IEnumerator FireDir_01()
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

            bullet.GetComponent<Bullet>().Init(damage, per, dir, data.itemId);
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);

            // 발사 사운드
            MasterAudio.PlaySound("Weapon52");

            // 발사 후 약간의 딜레이 추가
            yield return new WaitForSeconds(buletDelay);  // 총알 사이의 딜레이 설정 (0.1초)
        }
        isAttacking = false;

    }

}
