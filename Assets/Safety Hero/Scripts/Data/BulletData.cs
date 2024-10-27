using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Scriptble Object/BulletData")]
public class BulletData : ItemData
{
    public new enum ItemType 
    {    
        // 무기 류
        M0_Default, M1_Rotating, M2_MagneticField, // 근접
        R0_TargetGun = 50, R1_Cannon, R2_Throw, // 원거리
    }

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

    public override string GetDescription(int level, int increaseRate)
    {
        throw new System.NotImplementedException();
    }

    public override string GetLevelText(int level)
    {
        throw new System.NotImplementedException();
    }

    public override bool IsNewIconActive(int level)
    {
        throw new System.NotImplementedException();
    }
}

