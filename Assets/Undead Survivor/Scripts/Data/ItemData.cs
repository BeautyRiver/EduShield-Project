using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptble Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemCategory { Weapon, Gear, Etc }
    public enum ItemType 
    { 
        Shovel, Gun, Cannon, // 무기 류
        Glove, Shoe, // 기어 류
        Heal, Gold // 기타템 류
    }

    [Header("# 아이템 속성")]
    public ItemCategory itemCategory;
    public ItemType itemType;

    [Header("# 핵심 속성")]
    [Header("# 근접: 0 ~ 49 / 원거리: 50 ~ 99\n" +
        "# 기어: 100 ~ 199 / 기타: 200 ~ 299")]
    
    public int itemId;
    public string itemName;
    [TextArea]
    public string itemDesc;
    public Sprite itemIcon;

    [Header("# 레벨 데이터")]
    public float baseDamage;
    public int baseCount;
    public float[] damages;
    public int[] counts;

    [Header("# 무기")]
    public GameObject prefab;
    public Sprite hand;
}

