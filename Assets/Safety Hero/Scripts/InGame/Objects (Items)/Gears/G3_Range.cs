using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G3_Range : Gear
{
    protected override void ApplyPlayerData()
    {
        gm.playerData.attackRangeMult += rate;
        ApplyToAllWeapons();
    }

    // 범위 증가 기어
    protected override void ApplyGearToWeapon(Weapon weapon)
    {
        weapon.RecalculateStats();

        if (weapon is IBatchable batchableWeapon)
        {
            batchableWeapon.Batch();
        }
        Debug.Log($"{name}현재 배율: {gm.playerData.attackRangeMult}배");
    }
}
