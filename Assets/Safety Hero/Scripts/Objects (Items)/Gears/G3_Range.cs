using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G3_Range : Gear
{
    protected override void ApplyPlayerData()
    {
        gm.playerData.atkRangeMult += rate;
        ApplyToAllWeapons();
    }

    // 범위 증가 기어
    protected override void ApplyGearToWeapon(Weapon weapon)
    {
        weapon.bulletSize = weapon.data.baseScale * gm.playerData.atkRangeMult;
        weapon.attackRange = weapon.data.baseRange * gm.playerData.atkRangeMult;

        if (weapon is IBatchable batchableWeapon)
        {
            batchableWeapon.Batch();
        }
        Debug.Log($"{name}현재 배율: {gm.playerData.atkRangeMult}배");
    }
}
