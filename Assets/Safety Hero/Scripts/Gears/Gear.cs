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
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            ApplyGearToWeapon(weapon);
        }        
    }

    /// <summary>
    /// 무기에 영향이 가는 기어들 적용
    /// </summary>
    public void ApplyGearToWeapon(Weapon weapon)
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
    private void ApplyPowerUp(Weapon weapon)
    {
        weapon.damage = weapon.data.baseDamage * gm.playerData.damageMult;
        if (weapon is IRotatingable rotatingableWeapon)
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
    private void ApplyAttackSpeedUp(Weapon weapon)
    {
        weapon.weaponSpeed = weapon.data.baseSpeed / gm.playerData.atkSpeedMult;
        weapon.damageInterval = weapon.data.baseDamageInterval / gm.playerData.atkSpeedMult;

        if (weapon is IRotatingable rotatingableWeapon)
        {
            weapon.bulletDelay = weapon.data.baseDelay * gm.playerData.atkSpeedMult;
        }
        Debug.Log($"{name}현재 배율: {gm.playerData.atkSpeedMult}배");      
    }
    /// <summary>
    /// 공격 범위 증가 기어
    /// </summary>   
    private void ApplyRangeUp(Weapon weapon)
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

