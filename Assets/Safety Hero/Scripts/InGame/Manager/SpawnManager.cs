using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<SpawnProfile> spawnProfiles;

    public float levelTime; // 레벨별 시간 간격
    public int level; // 현재 레벨

    [SerializeField] private int prevLevel; // 이전 레벨 (비교용)

    [Header("# 스폰 위치")]
    public Transform[] spawnPoint; // 공용 스폰 위치 배열

    [Header("# 박스 소환")]
    [SerializeField] private LayerMask collisionMask; // 충돌을 감지할 레이어 (필요에 맞게 설정)
    [SerializeField] private LayerMask groundMask;

    private GameManager gm;

    private void Start()
    {
        InitializeSettings();
        gm = GameManager.instance;
    }

    private void Update()
    {
        if (GlobalManager.instance.gameState != GameState.Playing) return;

        transform.position = GameManager.instance.player.transform.position;

        // # Level Change
        if (prevLevel != level)
        {
            LevelChangeRoutine();
            prevLevel = level;
        }

        foreach (var profile in spawnProfiles)
        {
            if (profile.isActive && profile.timer > profile.spawnInterval)
            {
                // # Set Next Spawn Time
                profile.timer = 0f;
                Spawn(profile);
            }
            else
                profile.timer += Time.deltaTime;
        }

        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), 10); // 최대 레벨 제한 (예시)   
      
    }

    // 초기 설정
    void InitializeSettings()
    {
        levelTime = GameManager.instance.maxGameTime / 10; // 최대 레벨 수로 나누기
        level = 0;
        prevLevel = level;
    }

    // 레벨 변경 시 로직
    void LevelChangeRoutine()
    {
        // 유니크 몬스터 강화 로직 
        foreach (var profile in spawnProfiles)
        {
            if (profile.type == Type.Unique)
            {
                // 스폰 시간 10% 감소 (최소 0.5초)
                profile.spawnInterval = Mathf.Max(0.5f, profile.spawnInterval * 0.9f);
                // 스폰 개수 1 증가
                profile.spawnCount++;
                Debug.Log("유니크 몬스터 강화!");
            }
        }

        // 미니 보스 스폰 로직도 여기에 통합 가능
        foreach (var profile in spawnProfiles)
        {
            // 레벨이 2의 배수가 될 때마다 미니보스 스폰 활성화
            if (profile.type == Type.MiniBoss && level > 0 && level % 2 == 0)
            {
                profile.isActive = true;
                Debug.Log($"레벨 {level}: 미니보스 스폰 시작!");
            }
        }
    }

    void Spawn(SpawnProfile profile)
    {
        for (int i = 0; i < profile.spawnCount; i++)
        {
            GameObject enemy = PoolManager.instance.Get(profile.enemyData.enemyPrefab);
            // 스폰 위치는 필요에 따라 profile별로 다르게 설정할 수도 있음
            
            enemy.transform.position = spawnPoint[Random.Range(0, spawnPoint.Length)].position;
            enemy.GetComponent<Enemy>().Init(profile.enemyData);
        }
    }   
}