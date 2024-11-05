using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G0_WeaponSpeed : Gear
{
    protected override void ApplyPlayerData()
    {
        gm.playerData.atkSpeedMult += rate;
        ApplyToAllWeapons(); 
    }

    // 이동속도 증가 기어
    protected override void ApplyGearToWeapon(Weapon weapon)
    {
        weapon.weaponSpeed = weapon.data.baseSpeed / gm.playerData.atkSpeedMult;
        weapon.damageInterval = weapon.data.baseDamageInterval / gm.playerData.atkSpeedMult;

        // 회전 무기는 dealy(회전속도) 증가시키기
        if (weapon is IRotatingable rotatingableWeapon)
        {
            weapon.bulletDelay = weapon.data.baseDelay * gm.playerData.atkSpeedMult;
        }
        Debug.Log($"{name}현재 배율: {gm.playerData.atkSpeedMult}배");
    }
}
