using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

[CreateAssetMenu(fileName = "Bullet", menuName = "Scriptble Object/BulletData")]
public class BulletData : ItemData
{
    public new enum ItemType
    {
        // 무기 류
        M0_Default, M1_Rotating, M2_MagneticField, // 근접
        R0_TargetGun = 50, R1_Cannon, R2_Throw, // 원거리
    }
    public new ItemType Type;

    [Header("# 기본 스탯")]
    public float baseDamage;
    public float baseDamageInterval = 2f;
    public int baseCount;
    public int basePer;
    public float baseDelay;
    public float baseSpeed;
    public float baseRange;
    public float baseRotationSpeed;
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

    [Header("무기 관련")]
    public GameObject prefab;
    public GameObject weaponType;

    // 에디터에서 값이 변경될 때 자동으로 호출
    protected override void OnValidate()
    {
        maxLevel = damages.Length + counts.Length + pers.Length + sizes.Length + 1;

        if (prefab != null)
            baseScale = prefab.transform.localScale;
    }

    public override void OnEnableSetting(ItemSetting itemSetting)
    {
        itemSetting.textLevel.text = "Lv." + (itemSetting.level + 1); // 레벨 표기
        if (itemSetting.level == 0)
        {
            itemSetting.newIcon.gameObject.SetActive(true);
            itemSetting.textDesc.text = "<color=#99FF8A>새로운 무기!</color>\n\n<size=90%>" + GetWeaponDescription() + "</size>";
        }
        else
        {
            itemSetting.newIcon.gameObject.SetActive(false);
            // 레벨에 따른 설명 업데이트
            itemSetting.textDesc.text = string.Format(itemDesc[itemSetting.outsideRateIdx], itemSetting.increaseRate);
        }

    }

    public override void OnClickSetting(ItemSetting itemSetting)
    {
        throw new System.NotImplementedException();
    }

    private string GetWeaponDescription()
    {
        // 아이템 타입에 따른 설명 반환
        switch (Type)
        {
            case ItemType.M0_Default:
                return "좌우로 적을 관통 공격";

            case ItemType.M1_Rotating:
                return "주변을 회전하며 공격";

            case ItemType.M2_MagneticField:
                return "범위 내 적 지속 공격";

            case ItemType.R0_TargetGun:
                return "가장 가까운 적 공격";

            case ItemType.R1_Cannon:
                return "반대 방향으로 강력한 관통 공격";

            case ItemType.R2_Throw:
                return "바라보는 방향으로 공격";
            default:
                return "무기 설명 없음";
        }
    }
}

