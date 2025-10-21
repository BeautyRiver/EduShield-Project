using System.Collections.Generic;
using UnityEngine;
using VInspector;

public enum Type
{
    Normal,
    Range,
    Unique,
    MiniBoss,
    Boss
}

[System.Serializable] 
public class SpawnProfile
{
    public string description;     // 인스펙터에서 알아보기 위한 설명
    public bool isActive = true;   // 이 스폰을 활성화할지 여부

    [Header("# 스폰 설정")]
    public Type type;              // 몬스터 타입 (노말, 유니크 등 구분용)
    public float spawnInterval;    // 스폰 간격 (초)
    public int spawnCount;         // 한 번에 스폰할 개수
    [ReadOnly]
    public float timer;
    [Header("# 몬스터 스탯")]
    public EnemyData enemyData;    // 몬스터의 체력, 속도 등 데이터
}