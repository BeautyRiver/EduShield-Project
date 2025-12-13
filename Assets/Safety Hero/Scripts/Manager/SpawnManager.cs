using System.Collections.Generic;
using UnityEngine;
using VInspector; // VInspector 사용

// [핵심] 레벨별 스폰 구성을 담을 껍데기 클래스
[System.Serializable]
public class LevelDesign
{
    [Header("레벨 설명 (예: 1레벨 - 초기 웨이브)")]
    public string note;
    [Header("이 레벨에서 등장할 스폰 목록")]
    public List<SpawnProfile> activeProfiles;
}

public class SpawnManager : MonoBehaviour
{

    [Header("# 스폰 위치")]
    public Transform[] spawnPoint;

    [Tab("# 레벨 디자인 설정")]
    public List<LevelDesign> levelDesigns;

    [ReadOnly] public float levelTime; // 레벨별 시간 간격
    [ReadOnly] public int currentLevelIndex; // 현재 레벨 (0부터 시작)
    private int maxLevelCount; // 최대 레벨 개수

    [SerializeField] private int prevLevelIndex; // 이전 레벨 감지용

    [Tab("# 상자 스폰 설정")]
    public GameObject chestPrefab;          // 스폰할 상자 프리팹
    public int chestSpawnCount = 30;        // 스폰할 상자 개수
    public Vector2 spawnAreaSize = new Vector2(20, 20); // 스폰 영역 크기
    public float minDistance = 2.0f;        // 최소 거리  
    public LayerMask obstacleMask;          // 장애물 레이어 마스크

    private GameManager gm;

    private void Start()
    {
        gm = GameManager.instance;

        // 설정된 레벨 디자인의 개수가 곧 최대 레벨
        maxLevelCount = levelDesigns.Count;

        // 전체 게임 시간을 레벨 개수로 나누어 구간 설정
        if (maxLevelCount > 0)
            levelTime = GameManager.instance.maxGameTime / maxLevelCount;
        else
            levelTime = 60f; // 안전장치

        currentLevelIndex = 0;
        prevLevelIndex = -1; // 시작하자마자 로직 실행을 위해 -1로 설정

        // 상자 스폰 
        SpawnChests();
    }

    private void Update()
    {
        if (GlobalManager.instance.gameState != GameState.Playing) return;

        // 1. 현재 시간 기반 레벨 인덱스 계산      
        int calculatedIndex = Mathf.FloorToInt(GameManager.instance.gameTime / levelTime);
        currentLevelIndex = Mathf.Min(calculatedIndex, maxLevelCount - 1);

        // 2. 레벨 변경 감지
        if (prevLevelIndex != currentLevelIndex)
        {
            LevelChangeLog();
            prevLevelIndex = currentLevelIndex;
            
        }

        // 3. 현재 레벨에 해당하는 스폰 프로필들만 실행
        if (maxLevelCount > 0)
        {
            List<SpawnProfile> currentProfiles = levelDesigns[currentLevelIndex].activeProfiles;

            foreach (var profile in currentProfiles)
            {
                // 프로필별 타이머 체크
                if (profile.timer > profile.spawnInterval)
                {
                    profile.timer = 0f;
                    Spawn(profile);
                }
                else
                {
                    profile.timer += Time.deltaTime;
                }
            }
        }
    }

    // 디버그용 로그 (하드코딩 로직 제거됨)
    private void LevelChangeLog()
    {
        Debug.Log($"[Level Change] {prevLevelIndex} -> {currentLevelIndex} : {levelDesigns[currentLevelIndex].note}");
        Debug.Log($"[Current Level]: {levelDesigns[currentLevelIndex].note}");
    }

    private void Spawn(SpawnProfile profile)
    {
        for (int i = 0; i < profile.spawnCount; i++)
        {
            if (profile.enemyData == null || profile.enemyData.enemyPrefab == null) continue;

            GameObject enemy = PoolManager.instance.Get(profile.enemyData.enemyPrefab);

            // 랜덤 위치 스폰
            enemy.transform.position = spawnPoint[Random.Range(0, spawnPoint.Length)].position;
            enemy.GetComponent<Enemy>().Init(profile.enemyData);
        }
    }

    private void SpawnChests()
    {
        int currentCount = 0;
        int maxAttempts = chestSpawnCount * 10; // 최대 시도 횟수

        while (currentCount < chestSpawnCount)
        {
            bool isSpawned = false;

            for (int i = 0; i < maxAttempts; i++)
            {
                Vector2 randomPos = new Vector2(
                    Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                    Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2));

                Collider2D hit = Physics2D.OverlapCircle(randomPos, minDistance, obstacleMask);

                if (hit == null)
                {
                    GameObject chest = PoolManager.instance.Get(chestPrefab);
                    chest.transform.position = randomPos;

                    currentCount++;
                    isSpawned = true;
                    break;
                }
            }
            if (!isSpawned)
            {
                Debug.LogWarning($"박스 스폰 실패! 자리가 너무 좁거나 장애물이 많습니다. (현재 {currentCount}/{chestSpawnCount})");
                break; // 더 이상 생성 불가능하다고 판단하고 종료
            }
        }
    }
}