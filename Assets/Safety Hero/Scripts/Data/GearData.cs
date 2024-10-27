using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gear", menuName = "Scriptble Object/GearData")]
public class GearData : ItemData
{
    public new enum ItemType
    {        
        G0_WeaponSpeed = 100, G1_Speed, G2_Power, G3_Range,// 기어 류    
    }    


    [Header("기어 능력치")]
    [Header("배율방식 / 50 = 50%증가")]
    public int[] gearRates;

    // 에디터에서 값이 변경될 때 자동으로 호출
    protected override void OnValidate()
    {
        maxLevel = gearRates.Length;        
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
