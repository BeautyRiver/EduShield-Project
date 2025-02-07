using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using VInspector;

public class Spawner : MonoBehaviour
{
    public float levelTime; // 레벨별 시간 간격
    public int level; // 현재 레벨
    [SerializeField] private int prevLevel; // 이전 레벨 (비교용)

    [Tab("# 스폰 위치 ")]
    public Transform[] spawnPoint; // 적 소환 위치 배열
    public Transform[] uniqeSpawnPoint; // 특별한 적 소환위치 배열
    [EndTab]

    [Tab("# 노말 적 소환 데이터")]
    [SerializeField] private float normalTimer; // 소환 타이머
    public SpawnData[] normalSpawnData; // 레벨별 소환 데이터 배열
    [EndTab]

    [Tab("# 특수 적 소환 데이터")]
    [SerializeField] private float uniqeTimer; // 소환 타이머
    public UniqueSpawnData[] uniqeSpawnData; // 레벨별 소환 데이터 배열
    [EndTab]

    [Tab("# 미니 보스 소환 데이터")]
    // [SerializeField] private float miniBossTimer; // 소환 타이머
    public SpawnData[] miniBossSpawnData; // 레벨별 소환 데이터 배열
    [EndTab]

    [Tab("# 박스 소환 시간")]
    [SerializeField] private float boxTimer; // 소환 타이머
    public Vector2 boxSpawnTime; // 레벨별 소환 데이터 배열
    [EndTab]

    private void Start()
    {
        // 초기 설정
        InitializeSettings();
    }
 
    private void Update()
    {
        if (GameManager.instance.isGameActive)
        {
            // 소환 로직
            normalTimer += Time.deltaTime; // Normal timer
            uniqeTimer += Time.deltaTime; // Unique timer
            boxTimer += Time.deltaTime; // Box timer

            level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), normalSpawnData.Length - 1);   
            
            // 레벨 변화 체크
            if (prevLevel != level)
            {
                // 레벨 변환시 실행되는 로직
                StartCoroutine(LevelChangeRoutine());
                prevLevel = level; // 이전 레벨을 현재 레벨로 업데이트
            }

            // 기본 몬스터 소환
            if (normalTimer > normalSpawnData[level].spawnTime)
            {
                normalTimer = 0f;
                SpawnNormal();
            }

            // 유니크 몬스터 소환
            if (uniqeTimer > uniqeSpawnData[0].spawnTime)
            {
                uniqeTimer = 0f;
                SpawnUnique();
            }

            // 박스 소환
            if (boxTimer > Random.Range(boxSpawnTime.x, boxSpawnTime.y))
            {
                boxTimer = 0f;
                SpawnBox();
                SpawnBox();
            }
        }
    }

    private void FixedUpdate()
    {
        transform.position = GameManager.instance.player.transform.position;
    }

    // 초기 설정
    private void InitializeSettings()
    {
        levelTime = GameManager.instance.maxGameTime / normalSpawnData.Length;
        level = 0;
        prevLevel = level;

        normalSpawnData[0].spriteType = GameManager.instance.selectStageIdx;
        uniqeSpawnData[0].spawnTime = Random.Range(uniqeSpawnData[0].ranSpawnTime.x, uniqeSpawnData[0].ranSpawnTime.y);
        uniqeSpawnData[0].spawnCount = (int)Random.Range(uniqeSpawnData[0].ranSpawnCount.x, uniqeSpawnData[0].ranSpawnCount.y);
    }

    // 레벨 변경 루틴 (레벨업)
    private IEnumerator LevelChangeRoutine()
    {
        yield return StartCoroutine(GameManager.instance.RandomStageIndex()); // StageIndex 변경이 완료될 때까지 대기
        normalSpawnData[level].spriteType = GameManager.instance.selectStageIdx;

        StartCoroutine(GameManager.instance.AIMsgShowAndHide());

        StartCoroutine(SpawnMiniBoss());

        // uniqe몬스터 스폰률 증가
        foreach (var uniqeData in uniqeSpawnData)
        {
            // 스폰 시간 감소
            uniqeData.ranSpawnTime.x -= 5f;
            uniqeData.ranSpawnTime.y -= 5f;

            // 스폰 개수 증가
            uniqeData.ranSpawnCount.x += 5;
            uniqeData.ranSpawnCount.y += 5;
        }
    }
  
    // 노말 몬스터 소환
    private void SpawnNormal()
    {
        // 적 소환
        for (int i = 0; i < normalSpawnData[level].spawnCount; i++)
        {
            GameObject enemy = GameManager.instance.poolManager.Get(PoolType.Enemy, 0); // 기본 Enemy 소환
            enemy.transform.position = spawnPoint[Random.Range(0, spawnPoint.Length)].position;
            enemy.GetComponent<Enemy>().Init(normalSpawnData[level]);
        }
    }

    // 유니크 몬스터 소환
    private void SpawnUnique()
    {
        // 적 소환
        int ran = Random.Range(0, uniqeSpawnPoint.Length);
        int index = Mathf.Min(level, uniqeSpawnData.Length);
        for (int i = 0; i < uniqeSpawnData[index].spawnCount; i++)
        {
            GameObject enemy = GameManager.instance.poolManager.Get(PoolType.Enemy, 2); // 유니크 몬스터 소환
            Vector3 ranPos = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
            enemy.transform.position = uniqeSpawnPoint[ran].position + ranPos;
            enemy.GetComponent<Enemy>().Init(uniqeSpawnData[index]);
        }
        var uniqeData = uniqeSpawnData[index];
        uniqeData.spawnTime = Random.Range(uniqeData.ranSpawnTime.x, uniqeData.ranSpawnTime.y);
        uniqeData.spawnCount = (int)Random.Range(uniqeData.ranSpawnCount.x, uniqeData.ranSpawnCount.y);
    }

    // 미니 보스 소환 루틴
    private IEnumerator SpawnMiniBoss()
    {
        yield return new WaitForSeconds(2f);
        // 웨이브 변환시 미니 보스 한마리씩 등장
        miniBossSpawnData[level - 1].spriteType = GameManager.instance.selectStageIdx;

        // 적 소환
        for (int i = 0; i < miniBossSpawnData[level - 1].spawnCount; i++)
        {
            GameObject enemy = GameManager.instance.poolManager.Get(PoolType.Enemy, 2); // 미니 보스 소환
            enemy.transform.position = spawnPoint[Random.Range(0, spawnPoint.Length)].position;
            enemy.GetComponent<Enemy>().Init(miniBossSpawnData[level - 1]);
        }
    }

    // 박스 소환
    private void SpawnBox()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool isSafePosition = false; // 충돌 없는 안전한 위치인지 확인하는 변수
        float boxRadius = 0.5f; // 박스의 크기에 맞는 반지름으로 설정
        LayerMask collisionMask = LayerMask.GetMask("GroundPhyscis"); // 충돌을 감지할 레이어 (필요에 맞게 설정)
        LayerMask groundMask = LayerMask.GetMask("Ground");
        Transform parentTransform = transform;

        int loopNo = 0;
        // 충돌 없는 위치를 찾을 때까지 반복
        while (!isSafePosition)
        {
            if (loopNo >= 1000)
            {
                Debug.LogError("무한루프 방지 탈출");
                return;
            }
            spawnPosition = spawnPoint[Random.Range(0, spawnPoint.Length)].position;

            // 충돌 검사: 박스가 스폰될 위치에 다른 콜라이더가 있는지 확인 (OverlapCircle 사용)
            if (Physics2D.OverlapCircle(spawnPosition, boxRadius, collisionMask) == null)
            {
                isSafePosition = true; // 충돌이 없으면 안전한 위치로 설정
            }            

            if (isSafePosition)
            {
                parentTransform = Physics2D.OverlapCircle(spawnPosition, boxRadius, groundMask).transform;
                //Debug.Log("부모 설정 완료 : " + parentTransform.name);
            }
            loopNo++;
        }

        // 안전한 위치가 확인되면 박스 생성
        //Debug.Log("생성 완료");
        GameObject box = GameManager.instance.poolManager.Get(PoolType.Item, 1); // Box 생성
        box.transform.parent = parentTransform;
        box.transform.position = spawnPosition;
    }   

}
public enum Type
{
    Normal,
    Range,
    Unique,
    MiniBoss,
    Boss
}
[System.Serializable]
public class SpawnData
{
    public  Type type;
    [Header("# 스폰 시간 조절")]
    public float spawnTime;
    [Header("# 소환 개수")]
    public int spawnCount; // 몇 마리 소환
    [Header("# 스프라이트 타입")]
    public int spriteType; // 스프라이트 종류
    [Header("# 몬스터 기본 스탯")]
    public int health; // 적의 체력
    public float speed; // 적의 속도
    public float damage; // 적의 데미지
    public int exp; // 적의 획득 경험치량
}

[System.Serializable]
public class UniqueSpawnData : SpawnData
{
    public Vector2 ranSpawnTime;    
    public Vector2 ranSpawnCount;    

}

