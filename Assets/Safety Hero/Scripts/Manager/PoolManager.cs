using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 프리펩 보관 변수
    public GameObject[] prefabs;

    // 풀 담당 리스트
    private List<GameObject>[] pools;

    private void Awake()
    {
        // 배열 초기화
        pools = new List<GameObject>[prefabs.Length];
        // 리스트 초기화
        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        // 선택한 풀의 비활성화 된 게임오브젝트 접근!
        foreach (GameObject item in pools[index])
        {
            if (item.activeSelf == false)
            {        
                select = item; // 발견하면 select 변수에 할당
                select.SetActive(true);
                break;
            }
        }
        // 못 찾았으면?
        if (select == null)
        {
            // 새롭게 생성하고 select 변수에 할당
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }

        return select;
    }
}
