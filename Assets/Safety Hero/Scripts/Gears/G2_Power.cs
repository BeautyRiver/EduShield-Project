using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G2_Power : Gear
{
    protected override void ApplyPlayerData()
    {
        gm.playerData.damageMult += rate;
        ApplyToAllWeapons();
    }

    // 데미지 증가 기어
    protected override void ApplyGearToWeapon(Weapon weapon)
    {
        weapon.damage = weapon.data.baseDamage * gm.playerData.damageMult;
        Debug.Log($"{name}현재 배율: {gm.playerData.damageMult}배");
    }
}
