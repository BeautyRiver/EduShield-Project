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
    private GameManager gameManager;
    private Player player;
    private void Awake()
    {
        gameManager = GameManager.instance;
        player = gameManager.player;
    }

    public void Init(ItemData newData)
    {
        // 기본 세팅
        data = newData;
        gameObject.name = "Apply Gear" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero; // 플레이어 안에서 위치 초기화

        // 속성 세팅                                                                                     
        type = data.itemType;
        /*switch (type)
        {
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.PowerUp:
                rate = newData.gearRates[0];
                break;
        }*/
        rate = newData.gearRates[0];
        GearLevelUp(newData.gearRates[0]);
    }

    public void GearLevelUp(float newRate)
    {
        rate = newRate;
        accumulatedRate *= (1 + rate);
        ApplyGearEffect();
    }
    public void ApplyGearEffect()
    {
        switch (type)
        {
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.PowerUp:
            case ItemData.ItemType.RangeUp:
                ApplyToAllWeapons();
                break;
            case ItemData.ItemType.Shoe:
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
            case ItemData.ItemType.Glove:
                ApplyAttackSpeedUp(weapon);
                break;
            case ItemData.ItemType.PowerUp:
                ApplyPowerUp(weapon);
                break;
            case ItemData.ItemType.RangeUp:
                ApplyRangeUp(weapon);
                break;
        }
    }
    /// <summary>
    /// 공격 범위 증가 기어
    /// </summary>   
    private void ApplyRangeUp(Weapon weapon)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 공격속도 증가 기어
    /// </summary>    
    private void ApplyAttackSpeedUp(Weapon weapon)
    {
        switch (weapon.data.itemType)
        {
            case ItemData.ItemType.Shovel: // 회전 무기
                weapon.weaponSpeed *= (1 + rate);
                Debug.Log($"{weapon.name} To {type.ToString()}업그레이드. x {1 + rate}배");
                break;

            case ItemData.ItemType.Smoke: // 가스
            case ItemData.ItemType.Gun: // 총
            case ItemData.ItemType.Cannon: // 대포
            case ItemData.ItemType.Spear: // 창
                weapon.weaponSpeed /= (1 + rate);
                Debug.Log($"{weapon.name} To {type.ToString()}업그레이드. / {1 + rate}");
                break;
        }

    }

    /// <summary>
    /// 이동속도 증가 기어
    /// </summary>
    private void ApplySpeedUp()
    {
        float speed = player.speed;
        gameManager.player.speed *= (1 + rate);
        Debug.Log($"{type.ToString()}업그레이드. x {1 + rate}배");
    }

    /// <summary>
    /// 데미지 증가 기어
    /// </summary>
    private void ApplyPowerUp(Weapon weapon)
    {
        weapon.damage *= (1 + rate);
        if (weapon.data.itemType == ItemData.ItemType.Shovel)
        {
            foreach (Bullet bullet in weapon.GetComponentsInChildren<Bullet>())
            {
                bullet.damage = weapon.damage;
            }
        }
        Debug.Log($"{weapon.name} To {type.ToString()}업그레이드. x {1 + rate}배");
    }
}

