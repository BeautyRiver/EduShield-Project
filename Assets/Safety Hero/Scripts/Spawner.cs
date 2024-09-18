using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("# 적 소환 위치")]
    public Transform[] spawnPoint; // 적 소환 위치 배열
    public Transform[] uniqeSpawnPoint; // 특별한 적 소환위치 배열

    [Header("# 노말 적 소환 데이터")]
    public SpawnData[] normalSpawnData; // 레벨별 소환 데이터 배열

    [Header("# 특수 적 소환 데이터")]
    public SpawnData[] uniqeSpawnData; // 레벨별 소환 데이터 배열
    public float levelTime; // 레벨별 시간 간격

    [Header("# 레벨 관련")]
    [SerializeField] private int level; // 현재 레벨
    [SerializeField] private float[] timer; // 소환 타이머

    private void Awake()
    {
        // 초기 설정
        //spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.maxGameTime / normalSpawnData.Length;               
        timer = new float[2];        
        level = 0;
        normalSpawnData[0].spriteType = GameManager.instance.selectStageIdx;

        foreach (var uniqeData in uniqeSpawnData)
        {
            uniqeData.spawnTime = Random.Range(uniqeData.minTime, uniqeData.maxTime);
        }
    }

    private void Update()
    {
        if (GameManager.instance.isLive)
        {
            // 소환 로직
            timer[0] += Time.deltaTime;
            timer[1] += Time.deltaTime;
            level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), normalSpawnData.Length - 1);

            // 소환 타이머가 소환 시간을 초과하면 소환
            if (timer[0] > normalSpawnData[level].spawnTime)
            {
                timer[0] = 0f;
                SpawnNormal();
            }

            // 소환 타이머가 소환 시간을 초과하면 소환
            if (timer[1] > uniqeSpawnData[0].spawnTime)
            {
                timer[1] = 0f;
                SpawnUnique();
            }
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
        
    }

    private void SpawnNormal()
    {
        // 적 소환
        for (int i = 0; i < normalSpawnData[level].spawnCount; i++)
        {
            GameObject enemy = GameManager.instance.pool.Get(PoolManager.PoolType.Enemy, 2);
            enemy.transform.position = spawnPoint[Random.Range(0, spawnPoint.Length)].position;
            enemy.GetComponent<Enemy>().Init(normalSpawnData[0]);
        }
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
}

