using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("# 레벨 관련")]
    public float levelTime; // 레벨별 시간 간격
    public int level; // 현재 레벨
    [SerializeField] private int prevLevel; // 이전 레벨 (비교용)
    [SerializeField] private float[] timer; // 소환 타이머

    [Header("# 적 소환 위치")]
    public Transform[] spawnPoint; // 적 소환 위치 배열
    public Transform[] uniqeSpawnPoint; // 특별한 적 소환위치 배열

    [Header("# 노말 적 소환 데이터")]
    public SpawnData[] normalSpawnData; // 레벨별 소환 데이터 배열

    [Header("# 특수 적 소환 데이터")]
    public SpawnData[] uniqeSpawnData; // 레벨별 소환 데이터 배열

    [Header("# 미니 보스 소환 데이터")]
    public SpawnData[] miniBossSpawnData; // 레벨별 소환 데이터 배열

    [Header("# 박스 소환 시간")]
    public float boxSpawnTime; // 레벨별 소환 데이터 배열
    private void Awake()
    {
        // 초기 설정
        levelTime = GameManager.instance.maxGameTime / normalSpawnData.Length;                              
        timer = new float[4];        
        level = 0;
        prevLevel = level;
        normalSpawnData[0].spriteType = GameManager.instance.selectStageIdx;

        uniqeSpawnData[0].spawnTime = Random.Range(uniqeSpawnData[0].minTime, uniqeSpawnData[0].maxTime);

        miniBossSpawnData[0].spawnTime = Random.Range(miniBossSpawnData[0].minTime, miniBossSpawnData[0].maxTime);
        normalSpawnData[0].spriteType = GameManager.instance.selectStageIdx;        
    }

    private void Update()
    {
        if (GameManager.instance.isLive)
        {
            // 소환 로직
            timer[0] += Time.deltaTime; // Normal timer
            timer[1] += Time.deltaTime; // Unique timer
            timer[3] += Time.deltaTime; // Box timer

            level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), normalSpawnData.Length - 1);   
            
            // 레벨 변화 체크
            if (prevLevel != level)
            {
                GameManager.instance.RandomStageIndex();
                StartCoroutine(GameManager.instance.AIMsgShowAndHide());
                normalSpawnData[level].spriteType = GameManager.instance.selectStageIdx;
                prevLevel = level; // 이전 레벨을 현재 레벨로 업데이트
                foreach (var uniqeData in uniqeSpawnData)
                {
                    uniqeData.minTime -= 5f;
                    uniqeData.maxTime -= 5f;
                }

                miniBossSpawnData[level-1].spriteType = GameManager.instance.selectStageIdx;
                SpawnMiniBoss(); // 웨이브 변환시 미니 보스 한마리씩 등장
                Debug.Log("Level Change");
            }

            // 기본 몬스터 소환
            if (timer[0] > normalSpawnData[level].spawnTime)
            {
                timer[0] = 0f;
                SpawnNormal();
            }

            // 유니크 몬스터 소환
            if (timer[1] > uniqeSpawnData[0].spawnTime)
            {
                timer[1] = 0f;
                SpawnUnique();
            }


            // 박스 소환
            if (timer[3] > boxSpawnTime)
            {
                timer[3] = 0f;
                SpawnBox();
                SpawnBox();
            }


        }
    }

    private void SpawnMiniBoss()
    {
        // 적 소환
        for (int i = 0; i < miniBossSpawnData[level-1].spawnCount; i++)
        {
            GameObject enemy = GameManager.instance.pool.Get(PoolManager.PoolType.Enemy, 4);
            enemy.transform.position = spawnPoint[Random.Range(0, spawnPoint.Length)].position;
            enemy.GetComponent<Enemy>().Init(miniBossSpawnData[level - 1]);
        }
    }

    private void SpawnUnique()
    {
        // 적 소환
        int ran = Random.Range(0, uniqeSpawnPoint.Length);
        for (int i = 0; i < uniqeSpawnData[0].spawnCount; i++)
        {
            GameObject enemy = GameManager.instance.pool.Get(PoolManager.PoolType.Enemy, 3);
            Vector3 ranPos = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
            enemy.transform.position = uniqeSpawnPoint[ran].position + ranPos;
            enemy.GetComponent<Enemy>().Init(uniqeSpawnData[0]);
        }
        foreach (var uniqeData in uniqeSpawnData)
        {
            uniqeData.spawnTime = Random.Range(uniqeData.minTime, uniqeData.maxTime);
        }

    }

    private void SpawnNormal()
    {
        // 적 소환
        for (int i = 0; i < normalSpawnData[level].spawnCount; i++)
        {
            GameObject enemy = GameManager.instance.pool.Get(PoolManager.PoolType.Enemy, 2);
            enemy.transform.position = spawnPoint[Random.Range(0, spawnPoint.Length)].position;
            enemy.GetComponent<Enemy>().Init(normalSpawnData[level]);
        }
    }
    private void SpawnBox()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool isSafePosition = false; // 충돌 없는 안전한 위치인지 확인하는 변수
        float boxRadius = 0.5f; // 박스의 크기에 맞는 반지름으로 설정
        LayerMask collisionMask = LayerMask.GetMask("GroundPhyscis"); // 충돌을 감지할 레이어 (필요에 맞게 설정)

        // 충돌 없는 위치를 찾을 때까지 반복
        while (!isSafePosition)
        {
            spawnPosition = spawnPoint[Random.Range(0, spawnPoint.Length)].position;

            // 충돌 검사: 박스가 스폰될 위치에 다른 콜라이더가 있는지 확인 (OverlapCircle 사용)
            if (Physics2D.OverlapCircle(spawnPosition, boxRadius, collisionMask) == null)
            {
                isSafePosition = true; // 충돌이 없으면 안전한 위치로 설정
            }
        }

        // 안전한 위치가 확인되면 박스 생성
        GameObject box = GameManager.instance.pool.Get(PoolManager.PoolType.Item, 0);
        box.transform.position = spawnPosition;
    }   

}
[System.Serializable]
public class SpawnData
{
    [Header("# 스폰 시간 조절")]
    public float minTime; // 소환 간격 시간 최소
    public float maxTime; // 소환 간격 시간 최대
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

