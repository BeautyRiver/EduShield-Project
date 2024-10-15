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
        M0_Default, M1_Rotating, M2_MagneticField, // 근접
        R0_TargetGun = 50, R1_Cannon, R2_Throw, // 원거리

        G0_WeaponSpeed = 100, G1_Speed, G2_Power, G3_Range,// 기어 류

        E0_Heal = 200, E1_Gold // 기타템 류
    }

    [Header("# 아이템 속성")]
    public ItemCategory itemCategory;

    [Header("# 아이템 타입")]
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
    public float baseDamageInterval = 2f;
    public int baseCount;
    public int basePer;
    public float baseDelay;
    public float baseSpeed;
    public float baseRange;
    [Header("# Scale은 Prefab에서 변경!")]
    public Vector3 baseScale;

    [Header("# 레벨별 스탯")]

    [Header("데미지")]
    public int[] damages; // 데미지
    [Header("개수")]
    public int[] counts; // 개수
    [Header("관통력")]
    public int[] pers; // 관통력    

    [Header("크기 [10 = 10%]")]
    public int[] sizes;
    

    [Header("기어 능력치")]
    [Header("배율방식 / 50 = 50%증가")]
    public int[] gearRates;

    [Header("무기 관련")]
    public GameObject prefab;
    public Sprite hand;

    // 에디터에서 값이 변경될 때 자동으로 호출
    private void OnValidate()
    {
        // 각 배열의 최대 길이를 구해 maxLevel로 설정
        switch (itemCategory)
        {
            case ItemCategory.Weapon:
                maxLevel = damages.Length + counts.Length + pers.Length + sizes.Length + gearRates.Length + 1;
                break;
            case ItemCategory.Gear:
                maxLevel = damages.Length + counts.Length + pers.Length + sizes.Length + gearRates.Length;
                break;
        }

        if (prefab != null ) 
            baseScale = prefab.transform.localScale;
    }
}

