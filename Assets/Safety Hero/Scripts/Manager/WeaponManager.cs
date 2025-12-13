using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("# 무기 관리")]
    public List<Weapon> activeWeapons = new List<Weapon>(); // 현재 보유 중인 무기 리스트

    // 1. 새 무기 추가 (생성)
    public Weapon AddWeapon(BulletData data)
    {
        // 프리팹 생성
        GameObject weaponObj = Instantiate(data.weaponType, transform);
        Weapon newWeapon = weaponObj.GetComponent<Weapon>();

        // 초기화
        newWeapon.Init(data);

        // 리스트에 등록
        activeWeapons.Add(newWeapon);
        GameManager.instance.curretWeaponCount++; 

        return newWeapon;
    }

    // 2. 기존 무기 레벨업
    public void UpgradeWeapon(Weapon weapon, Rarity rarity, List<StatType> options)
    {
        if (weapon == null) return;

        weapon.LevelUp(rarity, options);
    }

    // 모든 무기 공격 중지/재개 
    public void SetAttackState(bool canAttack)
    {
        foreach (var weapon in activeWeapons)
        {
            weapon.isAttacking = !canAttack; 
            weapon.enabled = canAttack;
        }
    }
}