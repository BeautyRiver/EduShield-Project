using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G1_Speed : Gear
{
    protected override void ApplyPlayerData()
    {
        gm.playerData.speedMult += rate;
        PlayerSpeedUp();
    }

    protected void PlayerSpeedUp()
    {
        gm.player.speed = gm.player.baseSpeed * gm.playerData.speedMult;
        Debug.Log($"{name}현재 배율: {gm.playerData.speedMult}배");
    }

    // Player Speed 기어는 Weapon에 적용 안하므로 사용 X 
    protected override void ApplyGearToWeapon(Weapon weapon)
    {
        
    }


}
