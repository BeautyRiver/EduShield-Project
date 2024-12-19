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
        weapon.weaponSpeed = weapon.data.baseAttackSpeed / gm.playerData.attackSpeedMult;
        weapon.damageInterval = weapon.data.baseDamageInterval / gm.playerData.attackSpeedMult;
        weapon.bulletDelay = weapon.data.baseDelay / gm.playerData.attackSpeedMult;
        weapon.rotationSpeed = weapon.data.baseRotationSpeed * gm.playerData.attackSpeedMult;

   
        Debug.Log($"{name}현재 배율: {gm.playerData.attackSpeedMult}배");
    }
}
