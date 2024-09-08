using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate; // 공격 속도 증가율

    private GameManager gameManager;
    private Player player;
    private void Awake()
    {
        gameManager = GameManager.instance;
        player = gameManager.player;
    }

    public void Init(ItemData data)
    {
        // 기본 세팅
        gameObject.name = "Apply Gear" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero; // 플레이어 안에서 위치 초기화

        // 속성 세팅                                                                                     
        type = data.itemType;
        switch (type)
        {
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.PowerUp:
                rate = data.gearRates[0];
                break;                
        }
        ApplyGear();
    }

    public void GearLevelUp(ItemData.ItemType gearType, float rate)
    {
        switch (gearType)
        {
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.PowerUp:
                this.rate = rate;
                break;                
        }
        ApplyGear(); // 기어 적용
    }

    private void ApplyGear()
    {
        switch (type)
        {            
            case ItemData.ItemType.Glove:
                AttackSpeedUp();
                break;

            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;            
            case ItemData.ItemType.PowerUp:
                PowerUp();
                break;
        }
    }

    private void PowerUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons)
        {
            float damage = weapon.damage;
            damage += weapon.baseDamage * rate;

            weapon.damage = damage;

            if (weapon.id == 0)
            {
                Bullet[] bullet = weapon.GetComponentsInChildren<Bullet>();
                for (int i = 0; i < weapon.count; i++)
                {
                    bullet[i].damage = damage;
                }
            }
        }
    }

    // 모든 무기 연사력 증가 함수
    private void AttackSpeedUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                // 회전 무기
                case 0: //삽
                    float weaponSpeed = (float)System.Math.Round(weapon.baseSpeed * gameManager.playerData.atkSpeedMult, 2); 
                    weapon.weaponSpeed = weaponSpeed + (weaponSpeed * rate - 1);
                    break;

                // 원거리 무기
                case 50: // 총
                case 51: // 대포
                case 52: // 창
                    weaponSpeed = (float)System.Math.Round(weapon.baseSpeed / gameManager.playerData.atkSpeedMult,2);
                    weapon.weaponSpeed = weaponSpeed * (1f - (rate-1));
                    break;
            }
        }
    }

    private void SpeedUp()
    {
        // defalutSpeed is 3
        float speed = player.defaluSpeed * gameManager.playerData.speedMult; // 캐릭터별 기본 속도 다르기 때문에 체크
        gameManager.player.speed = speed + speed * rate;
    }
}
