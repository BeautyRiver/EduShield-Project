using System;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    Bullet,
    Enemy, 
    EnemyBullet, 
    Drop,
    Item, 
    Effect, 
    Text,
}  // 풀 타입 enum

[System.Serializable]
public class Pool
{
    public PoolType poolType;
    public GameObject[] prefabs;
}

public class PoolManager : MonoBehaviour
{
    public Pool[] pools;

    private Dictionary<(PoolType, int), List<GameObject>> poolDictionary;

    private void Awake()
    {
        poolDictionary = new Dictionary<(PoolType, int), List<GameObject>>();

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
    }

    public GameObject Get(PoolType poolType, int index)
    {
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
            select = Instantiate(selectedPrefabs[index], transform);
            selectedPool.Add(select);
            select.SetActive(true);
        }

        return select;
    }

    public GameObject GetByPrefab(PoolType poolType, GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("GetByPrefab - 전달된 prefab이 null입니다.");
            return null;
        }

        // 풀 타입에 해당하는 Pool 찾기
        Pool targetPool = Array.Find(pools, p => p.poolType == poolType);
        if (targetPool == null)
        {
            Debug.LogWarning($"PoolManager - 해당 PoolType({poolType})을 찾을 수 없습니다.");
            return null;
        }

        // targetPool.prefabs 배열에서 prefab의 인덱스 찾기
        int prefabIndex = Array.IndexOf(targetPool.prefabs, prefab);
        if (prefabIndex == -1)
        {
            Debug.LogWarning(
                $"PoolManager - PoolType({poolType})에 등록되지 않은 prefab({prefab.name})입니다."
            );
            return null;
        }

        // 찾은 인덱스로 기존 Get() 메서드 호출
        return Get(poolType, prefabIndex);
    }
}

