using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public abstract class Gear : MonoBehaviour
{
    public GearData gearData;
    public float rate; // 공격 속도 증가율
    public int level;
    [SerializeField] protected float accumulatedRate = 1f; // 누적 증가율
    protected GameManager gm;
    protected Player player;
    protected virtual void Awake()
    {
        gm = GameManager.instance;
        player = gm.player;
    }

    public virtual void Init(GearData newData)
    {
        // 기본 세팅
        gearData = newData;
        gameObject.name = "Apply Gear" + gearData.itemType.ToString();
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero; // 플레이어 안에서 위치 초기화

        rate = newData.gearRates[0];
        GearLevelUp(newData.gearRates[0]);
    }

    public virtual void GearLevelUp(float newRate)
    {
        rate = newRate * 0.01f; // 기어 단위  수정
        accumulatedRate +=  rate;
        ApplyPlayerData();
    }

    /// <summary>
    /// 모든 무기에 무기에 영향을 끼치는 기어 적용
    /// </summary>
    protected virtual void ApplyToAllWeapons()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            ApplyGearToWeapon(weapon);
        }        
    }


    /// <summary>
    /// 플레이어 데이터에 기어 수치 적용하기
    /// </summary>
    protected abstract void ApplyPlayerData();    
    /// <summary>
    /// 무기에 영향이 가는 기어들 적용
    /// </summary>
    protected abstract void ApplyGearToWeapon(Weapon weapon);    
  
}

