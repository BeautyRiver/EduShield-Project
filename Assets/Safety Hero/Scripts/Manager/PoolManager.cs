using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public Pool[] pools;

    private Dictionary<(PoolType, int), List<GameObject>> poolDictionary;
    private Dictionary<PoolObjectType, (PoolType, int)> poolMappings;

    private void Awake()
    {
        poolDictionary = new Dictionary<(PoolType, int), List<GameObject>>();
        poolMappings = new Dictionary<PoolObjectType, (PoolType, int)>();
        InitializePoolMappings();

        // 각 풀 초기화
        foreach (Pool pool in pools)
        {
            for (int i = 0; i < pool.prefabs.Length; i++)
            {
                var poolKey = (pool.poolType, i);

                if (!poolDictionary.ContainsKey(poolKey))
                {
                    poolDictionary[poolKey] = new List<GameObject>();
                }
            }
        }
        // 초기화 후 Debug.Log로 확인
        Debug.Log("Pool Dictionary initialized successfully.");
        if (poolDictionary.Count == 0)
        {
            Debug.LogWarning("Pool Dictionary is empty. Initialization might have failed.");
        }

        Debug.Log("Pool Mappings initialized successfully.");
        if (poolMappings.Count == 0)
        {
            Debug.LogWarning("Pool Mappings is empty. Initialization might have failed.");
        }
        else
        {
            foreach (var mapping in poolMappings)
            {
                Debug.Log($"PoolObjectType: {mapping.Key}, PoolType: {mapping.Value.Item1}, Index: {mapping.Value.Item2}");
            }
        }
    }

    private void InitializePoolMappings()
    {
        foreach (var pool in pools)
        {
            for (int i = 0; i < pool.prefabs.Length; i++)
            {
                GameObject prefab = pool.prefabs[i];
                string formattedName = prefab.name.Replace(" ", ""); // 공백 제거
                if (Enum.TryParse(formattedName, out PoolObjectType poolObjectType))
                {
                    poolMappings[poolObjectType] = (pool.poolType, i);
                }
                else
                {
                    Debug.LogWarning($"Prefab name '{prefab.name}' does not match any PoolObjectType enum.");
                }                
            }
        }
    }

    public GameObject Get(PoolObjectType poolObjectType)
    {
        if (!poolMappings.ContainsKey(poolObjectType))
        {
            Debug.LogWarning($"PoolObjectType {poolObjectType} not found!");
            return null;
        }

        var (poolType, index) = poolMappings[poolObjectType];
        var poolKey = (poolType, index);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            Debug.LogWarning($"Pool {poolType} with index {index} not found!");
            return null;
        }

        List<GameObject> selectedPool = poolDictionary[poolKey];
        GameObject[] selectedPrefabs = Array.Find(pools, p => p.poolType == poolType)?.prefabs;

        if (selectedPrefabs == null || index >= selectedPrefabs.Length) return null;

        GameObject select = null;

        // 비활성화된 게임 오브젝트 탐색
        foreach (GameObject item in selectedPool)
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // 새로 생성하여 풀에 추가
        if (select == null)
        {
            Debug.Log(selectedPrefabs[index].name);
            select = Instantiate(selectedPrefabs[index], transform);
            selectedPool.Add(select);
            select.SetActive(false);
        }

        return select;
    }
}

public enum PoolType { Bullet, Enemy, Item, Effect, Text }  // 풀 타입 enum

public enum PoolObjectType
{
    // Bullet
    Bullet0, Bullet1, Bullet2, Bullet50, Bullet51, Bullet52,
    // Item
    BoxField, BoxReward, Exp, Heal, Magnet,
    // Enemy
    Enemy0, EnemyU0, EnemyMiniBoss,
    // Effect
    EffectPlayer, EffectEnemy,
    // Text
    TextEnemyDamaged,
}

[System.Serializable]
public class Pool
{
    public PoolType poolType;
    public GameObject[] prefabs;
}
