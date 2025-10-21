using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G0_WeaponSpeed : Gear
{
    protected override void ApplyPlayerData()
    {
        gm.playerData.attackSpeedMult += rate;
        ApplyToAllWeapons(); 
    }

    // 이동속도 증가 기어
    protected override void ApplyGearToWeapon(Weapon weapon)
    {
        weapon.RecalculateStats();
        Debug.Log($"{name}현재 배율: {gm.playerData.attackSpeedMult}배");
    }
}
