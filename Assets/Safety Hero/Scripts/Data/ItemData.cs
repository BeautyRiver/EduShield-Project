using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptble Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemCategory { Weapon, Gear, Etc }
    public enum WeaponType { Melee, Ranged }
    public enum ItemType 
    {    
        // 무기 류
        Shovel, // 근접
        Gun = 10, Cannon, Spear, // 원거리

        Glove = 100, Shoe, PowerUp,// 기어 류

        Heal = 200, Gold // 기타템 류
    }

    [Header("# 아이템 속성")]
    public ItemCategory itemCategory;
    [Header("# Melee -근접 / Ranged -원거리")]
    public WeaponType weaponType;
    public ItemType itemType;

    [Header("# 아이템 최대 레벨")]
    public int maxLevel;

    [Header("# 근접: 0 ~ 49 / 원거리: 50 ~ 99\n" +
        "# 기어: 100 ~ 199 / 기타: 200 ~ 299")]
    [Header("기본 정보")]

    public int itemId;
    public string itemName;
    [TextArea]
    public string[] itemDesc;
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
    [Header("개수")]
    public int[] counts; // 개수
    [Header("관통력")]
    public int[] pers; // 관통력
    [Header("기어 능력치")]
    [Header("배율방식 / 0.5 = 1.5배 (50%)증가")]
    [Header("\b*현재 상태에서 곱해지는 방식이므로\n 큰 수를 곱할시 값이 매우 커짐 주의*")]
    public float[] gearRates;

    [Header("무기 관련")]
    public GameObject prefab;
    public Sprite hand;

    // 에디터에서 값이 변경될 때 자동으로 호출
    private void OnValidate()
    {
        // 각 배열의 최대 길이를 구해 maxLevel로 설정
        maxLevel = damages.Length + counts.Length + pers.Length + gearRates.Length;
    }
}

