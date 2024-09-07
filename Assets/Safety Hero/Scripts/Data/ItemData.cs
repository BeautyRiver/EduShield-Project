using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptble Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemCategory { Weapon, Gear, Etc }
    public enum ItemType 
    {    
        // 무기 류
        Shovel, // 근접
        Gun = 10, Cannon, Spear, // 원거리

        Glove = 100, Shoe, // 기어 류

        Heal = 200, Gold // 기타템 류
    }

    [Header("# 아이템 속성")]
    public ItemCategory itemCategory;
    public ItemType itemType;

    [Header("# 근접: 0 ~ 49 / 원거리: 50 ~ 99\n" +
        "# 기어: 100 ~ 199 / 기타: 200 ~ 299")]
    [Header("기본 정보")]

    public int itemId;
    public string itemName;
    [TextArea]
    public string itemDesc;
    public Sprite itemIcon;

    [Header("# 기본 스탯")]
    public float baseDamage;
    public int baseCount;
    public int basePer;
    public float baseDelay;
    public float baseSpeed;

    [Header("# 레벨별 스탯")]
    [Header("데미지")]
    public float[] damages; // 데미지
    [Header("공격 속도")]
    public float[] weaponSpeeds; // 무기 공격속도
    [Header("개수")]
    public int[] counts; // 개수
    [Header("관통력")]
    public int[] pers; // 관통력

    public float[] speeds; // 플레이어 이동속도

    [Header("무기 관련")]
    public GameObject prefab;
    public Sprite hand;
}

