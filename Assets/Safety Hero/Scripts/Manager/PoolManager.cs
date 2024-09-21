using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public enum PoolType { Weapon, Enemy, Item, Effect }  // 풀 타입을 구분하는 enum
    public GameObject[] weaponPrefabs;
    public GameObject[] enemyPrefabs;
    public GameObject[] itemPrefabs;
    public GameObject[] effectPrefabs;

    private List<GameObject>[] weaponPools;
    private List<GameObject>[] enemyPools;
    private List<GameObject>[] itemPools;
    private List<GameObject>[] effectPools;

    private void Awake()
    {
        // WeaponPool 초기화
        weaponPools = new List<GameObject>[weaponPrefabs.Length];
        for (int index = 0; index < weaponPools.Length; index++)
        {
            weaponPools[index] = new List<GameObject>();
        }

        // EnemyPool 초기화
        enemyPools = new List<GameObject>[enemyPrefabs.Length];
        for (int index = 0; index < enemyPools.Length; index++)
        {
            enemyPools[index] = new List<GameObject>();
        }

        // ItemPool 초기화
        itemPools = new List<GameObject>[itemPrefabs.Length];
        for (int index = 0;index < itemPools.Length; index++)
        {
            itemPools[index] = new List<GameObject>();
        }

        // EfectPool 초기화
        effectPools = new List<GameObject>[effectPrefabs.Length];
        for (int index = 0; index < effectPools.Length; index++)
        {
            effectPools[index] = new List<GameObject>();
        }
    }

    public GameObject Get(PoolType poolType, int index)
    {
        List<GameObject>[] selectedPool = null;
        GameObject[] selectedPrefabs = null;

        // 풀 타입에 따라 풀과 프리팹 배열 선택

        switch (poolType)
        {
            case PoolType.Weapon:
                selectedPool = weaponPools;
                selectedPrefabs = weaponPrefabs;
                break;

            case PoolType.Enemy:
                selectedPool = enemyPools;
                selectedPrefabs = enemyPrefabs;
                break;

            case PoolType.Item:
                selectedPool = itemPools;
                selectedPrefabs = itemPrefabs;
                break;

            case PoolType.Effect:
                selectedPool = effectPools;
                selectedPrefabs = effectPrefabs;
                break;
        }


        if (selectedPool == null || selectedPrefabs == null) return null;

        GameObject select = null;

        // 선택한 풀의 비활성화 된 게임 오브젝트 접근
        foreach (GameObject item in selectedPool[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // 풀에서 비활성화된 오브젝트를 찾지 못한 경우, 새로 생성
        if (select == null)
        {
            select = Instantiate(selectedPrefabs[index], transform);
            selectedPool[index].Add(select);
        }

        return select;
    }
}
