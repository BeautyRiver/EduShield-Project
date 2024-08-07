using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptble Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemCategory { Weapon, Gear, Etc }
    public enum ItemType { Melee, Range, Glove, Shoe, Heal }

    [Header("# 아이템 속성")]
    public ItemCategory itemCategory;
    public ItemType itemType;

    [Header("# 핵심 속성")]
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
    public GameObject projectile;
    public Sprite hand;
}

