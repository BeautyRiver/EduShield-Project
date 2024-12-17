using System;
using System.Collections.Generic;
using UnityEngine;

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
}

public enum PoolType { Bullet, Enemy, Item, Effect, Text}  // 풀 타입 enum

[System.Serializable]
public class Pool
{
    public PoolType poolType;
    public GameObject[] prefabs;
}