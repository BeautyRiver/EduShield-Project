using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class Gear : MonoBehaviour
{
    public ItemData data;
    public ItemData.ItemType type;
    public float rate; // 공격 속도 증가율
    public int level;
    public float accumulatedRate = 1f; // 누적 증가율
    private GameManager gm;
    private Player player;
    private void Awake()
    {
        gm = GameManager.instance;
        player = gm.player;
    }

    public void Init(ItemData newData)
    {
        // 기본 세팅
        data = newData;
        gameObject.name = "Apply Gear" + data.itemType.ToString();
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero; // 플레이어 안에서 위치 초기화

        // 속성 세팅                                                                                     
        type = data.itemType;
        /*switch (type)
        {
            case ItemData.ItemType.G0_WeaponSpeed:
            case ItemData.ItemType.G1_Speed:
            case ItemData.ItemType.G2_Power:
                rate = newData.gearRates[0];
                break;
        }*/
        rate = newData.gearRates[0];
        GearLevelUp(newData.gearRates[0]);
    }

    public void GearLevelUp(float newRate)
    {
        rate = newRate * 0.01f; // 기어 단위  수정
        accumulatedRate +=  rate;
        ApplyGearEffect();
    }
    public void ApplyGearEffect()
    {
        switch (type)
        {
            case ItemData.ItemType.G0_WeaponSpeed:
                gm.playerData.atkSpeedMult += rate;
                ApplyToAllWeapons();
                break;
            case ItemData.ItemType.G2_Power:
                gm.playerData.damageMult += rate;
                ApplyToAllWeapons();
                break;
            case ItemData.ItemType.G3_Range:
                gm.playerData.atkRangeMult += rate;
                ApplyToAllWeapons();
                break;
            case ItemData.ItemType.G1_Speed:
                gm.playerData.speedMult += rate;
                ApplySpeedUp();
                break;
        }        
    }
    /// <summary>
    /// 모든 무기에 무기에 영향을 끼치는 기어 적용
    /// </summary>
    private void ApplyToAllWeapons()
    {
        tempWeapon[] weapons = transform.parent.GetComponentsInChildren<tempWeapon>();
        foreach (tempWeapon weapon in weapons)
        {
            ApplyGearToWeapon(weapon);
        }        
    }

    /// <summary>
    /// 무기에 영향이 가는 기어들 적용
    /// </summary>
    public void ApplyGearToWeapon(tempWeapon weapon)
    {
        switch (type)
        {
            case ItemData.ItemType.G0_WeaponSpeed:
                ApplyAttackSpeedUp(weapon);
                break;
            case ItemData.ItemType.G2_Power:
                ApplyPowerUp(weapon);
                break;
            case ItemData.ItemType.G3_Range:
                ApplyRangeUp(weapon);
                break;
        }
    }

    /// <summary>
    /// 이동속도 증가 기어
    /// </summary>
    private void ApplySpeedUp()
    {
        gm.player.speed = gm.player.baseSpeed * gm.playerData.speedMult;
        Debug.Log($"{name}현재 배율: {gm.playerData.speedMult}배");
    }

    /// <summary>
    /// 데미지 증가 기어
    /// </summary>
    private void ApplyPowerUp(tempWeapon weapon)
    {
        weapon.damage = weapon.data.baseDamage * gm.playerData.damageMult;
        if (weapon.data.itemType == ItemData.ItemType.M1_Rotating)
        {
            foreach (Bullet bullet in weapon.GetComponentsInChildren<Bullet>())
            {
                bullet.damage = weapon.damage;
            }
        }
        Debug.Log($"{name}현재 배율: {gm.playerData.damageMult}배");
    }
    /// <summary>
    /// 공격속도 증가 기어
    /// </summary>    
    private void ApplyAttackSpeedUp(tempWeapon weapon)
    {
        switch (weapon.data.itemType)
        {            
            case ItemData.ItemType.M1_Rotating: // 회전 무기
                weapon.weaponSpeed = weapon.data.baseSpeed / gm.playerData.atkSpeedMult;       
                weapon.bulletDelay = weapon.data.baseDelay * gm.playerData.atkSpeedMult;
                weapon.damageInterval = weapon.data.baseDamageInterval / gm.playerData.atkSpeedMult;
                Debug.Log($"{name}현재 배율: {gm.playerData.atkSpeedMult}배");
                break;

            case ItemData.ItemType.M0_Default: // 가스
            case ItemData.ItemType.M2_MagneticField: // 가스
            case ItemData.ItemType.R0_TargetGun: // 총
            case ItemData.ItemType.R1_Cannon: // 대포
            case ItemData.ItemType.R2_Throw: // 창
                weapon.weaponSpeed = weapon.data.baseSpeed / gm.playerData.atkSpeedMult;
                weapon.damageInterval = weapon.data.baseDamageInterval / gm.playerData.atkSpeedMult;
                Debug.Log($"{name}현재 배율: {gm.playerData.atkSpeedMult}배");
                break;                
        }

    }
    /// <summary>
    /// 공격 범위 증가 기어
    /// </summary>   
    private void ApplyRangeUp(tempWeapon weapon)
    {
        switch (weapon.data.itemType)
        {
            case ItemData.ItemType.M1_Rotating: // 회전 무기
                weapon.bulletSize = weapon.data.baseScale * gm.playerData.atkRangeMult;
                weapon.attackRange = weapon.data.baseRange * gm.playerData.atkRangeMult;
                weapon.M1_Batch();
                Debug.Log($"{name}현재 배율: {gm.playerData.atkRangeMult}배");
                break;

            case ItemData.ItemType.M2_MagneticField: // 자기장
                weapon.bulletSize = weapon.data.baseScale * gm.playerData.atkRangeMult;
                weapon.M2_Batch(weapon.transform.GetChild(0));
                Debug.Log($"{name}현재 배율: {gm.playerData.atkRangeMult}배");
                break;

            default:
                weapon.bulletSize = weapon.data.baseScale * gm.playerData.atkRangeMult;
                Debug.Log($"{name}현재 배율: {gm.playerData.atkRangeMult}배");
                break;
        }
    }
}

