using System.Collections.Generic;
using UnityEngine;

// Pool, PoolType enum은 기존과 동일
public enum PoolType
{
    Bullet, Enemy, EnemyBullet, Drop, Item, Effect, Text,
}

[System.Serializable]
public class Pool
{
    public PoolType poolType; // 정리용으로 남겨두지만, 실제 로직에선 사용 안 함
    public GameObject[] prefabs;
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    public Pool[] pools;

    public SerializableDictionary<GameObject, List<GameObject>> poolDictionary;
    // Dictionary의 키를 GameObject(프리팹)으로 직접 사용
    //private Dictionary<GameObject, List<GameObject>> poolDictionary;

    private void Awake()
    {
        if (instance == null)        
            instance = this;
        
        else
            Destroy(this.gameObject);

        poolDictionary = new SerializableDictionary<GameObject, List<GameObject>>();

        // 등록된 모든 프리팹을 Dictionary의 키로 초기화
        foreach (Pool pool in pools)
        {
            foreach (GameObject prefab in pool.prefabs)
            {
                if (!poolDictionary.ContainsKey(prefab))
                {
                    poolDictionary[prefab] = new List<GameObject>();
                }
                else
                {
                    Debug.LogWarning($"{prefab.name} 프리팹이 중복으로 등록되었습니다.");
                }
            }
        }
    }

    public GameObject Get(GameObject prefab)
    {
        // 1. 요청된 프리팹이 풀에 등록되어 있는지 확인
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning($"Pool for prefab '{prefab.name}' not found!");
            return null;
        }

        // 2. 해당 프리팹의 인스턴스 리스트(풀)를 가져옴
        List<GameObject> selectedPool = poolDictionary[prefab];
        GameObject select = null;

        // 3. 풀 안에서 비활성화된 오브젝트를 찾는다
        foreach (GameObject item in selectedPool)
        {
            if (!item.activeSelf)
            {
                select = item;
                break;
            }
        }

        // 4. 쓸 수 있는 오브젝트가 없으면 새로 생성
        if (select == null)
        {                        
            select = Instantiate(prefab, transform);
            selectedPool.Add(select);
        }

        //Debug.Log("Pooled: " +  select.name);
        select.SetActive(true);
        return select;
    }
}