using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootTable_", menuName = "Loot/Loot Table")]

public class LootTable : ScriptableObject
{
    public List<LootItem> lootItems;
    
    public GameObject GetRandomItem()
    {
        int totalWeight = 0;

        // 모든 아이템의 가중치를 더함
        foreach (var item in lootItems)
        {
            totalWeight += item.weight;
        }

        // 1부터 총합 사이의 랜덤 숫자를 뽑음
        int randomNumber = Random.Range(1, totalWeight + 1);

        // 랜덤 숫자가 어느 아이템의 가중치 범위 속하는지 찾음
        foreach (var item in lootItems)
        {
            if (randomNumber <= item.weight)
            {
                return item.itemPrefab; // 당첨!
            }
            else
            {
                randomNumber -= item.weight; // 다음 아이템으로 넘어감
            }
        }
        return null; // 비어있는 경우
    }
}
