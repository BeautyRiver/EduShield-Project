using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate; // 증가량

    public void Init(ItemData data)
    {
        // 기본 세팅
        gameObject.name = "Apply Gear" + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero; // 플레이어 안에서 위치 초기화

        // 속성 세팅
        type = data.itemType;
        rate = data.damages[0];
        ApplyGear();
    }

    public void GearLevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
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
                case 0:
                    float speed = 150 * GameManager.instance.playerData.atkSpeedMult; 
                    weapon.speed = speed + (speed * rate);
                    break;

                 // 원거리 무기
                default:
                    speed = 0.5f * GameManager.instance.playerData.atkDelayMult;
                    weapon.speed = speed * (1f - rate);
                    break;
            }
        }
    }

    private void SpeedUp()
    {
        float speed = 3 * GameManager.instance.playerData.speedMult; // 캐릭터별 기본 속도 다르기 때문에 체크
        GameManager.instance.player.speed = speed + speed * rate;
    }
}
