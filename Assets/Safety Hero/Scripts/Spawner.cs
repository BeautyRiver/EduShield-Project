using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint; // 적 소환 위치 배열
    public SpawnData[] spawnData; // 레벨별 소환 데이터 배열
    public float levelTime; // 레벨별 시간 간격

    [SerializeField] private int level; // 현재 레벨
    [SerializeField] private float timer; // 소환 타이머

    private void Awake()
    {
        // 초기 설정
        spawnPoint = GetComponentsInChildren<Transform>();
        level = GameManager.instance.selectStageIdx;
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }

    private void Update()
    {
        if (GameManager.instance.isLive)
        {
            // 소환 로직
            timer += Time.deltaTime;
            //level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);

            // 소환 타이머가 소환 시간을 초과하면 소환
            if (timer > spawnData[level].spawnTime)
            {
                timer = 0f;
                Spawn();
            }            
        }
    }

    private void Spawn()
    {
        // 적 소환
        GameObject enemy = GameManager.instance.pool.Get(PoolManager.PoolType.Enemy, 0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime; // 소환 간격 시간

    public int spriteType; // 스프라이트 종류
    public int health; // 적의 체력
    public float speed; // 적의 속도
}
