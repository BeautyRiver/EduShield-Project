using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ItemData;
public class Item : MonoBehaviour
{
    [Header("# 아이템 데이터")]
    public ItemData data;
    public Weapon weapon;
    public Gear gear;
    public int level;
    public int rateIndex;

    private Image icon;
    private TextMeshProUGUI textLevel;
    private TextMeshProUGUI textName;
    private TextMeshProUGUI textDesc;

    private void Awake()
    {
        // 아이콘 설정 및 세팅
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        // 레벨, 이름, 설명 텍스트 설정
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        switch (data.itemCategory)
        {
            case ItemCategory.Weapon:
            case ItemCategory.Gear:
                textLevel = texts[0];
                textName = texts[1];
                textDesc = texts[2];
                break;

            case ItemCategory.Etc:
                textLevel = null;
                textName = texts[0];
                textDesc = texts[1];
                break;
        }

        textName.text = data.itemName;
    }

    private void OnEnable()
    {
        // 설명글 세팅
        if (data.itemCategory != ItemCategory.Etc && textLevel != null)
        {
            textLevel.text = "Lv." + (level + 1); // 레벨 표기
        }

        switch (data.itemCategory)
        {
            // Weapons
            case ItemCategory.Weapon:
                if (level == 0)
                {
                    switch (data.itemType)
                    {
                        case ItemType.Shovel:
                            textDesc.text = "회전하며 적을 공격";
                            break;
                        case ItemType.Gun:
                            textDesc.text = "적을 자동 조준하는 총 발사";
                            break;
                        case ItemType.Cannon:
                            textDesc.text = "바라보는 방향으로 크게 관통하는 대포 발사";
                            break;
                        case ItemType.Spear:
                            textDesc.text = "바라보는 방향으로 무기 투척";
                            break;
                    }
                }
                else
                {
                    float selectRate = 0f;
                    switch (rateIndex)
                    {
                        case 0:
                            selectRate = data.damages[rateIndex];
                            break;
                        case 1:
                            selectRate = data.counts[rateIndex];
                            break;
                        case 2:
                            selectRate = data.pers[rateIndex];
                            break;
                    }
                    textDesc.text = string.Format(data.itemDesc[rateIndex], selectRate); // 무기 설명글
                }
                break;

            // Gears
            case ItemCategory.Gear:
                textDesc.text = string.Format(data.itemDesc[0], data.gearRates[level] * 100); // 기어 설명글
                break;

            // Etc
            case ItemCategory.Etc:
                textDesc.text = string.Format(data.itemDesc[0]); // 아이템 설명글
                break;
        }
    }

    // 아이템 클릭 시
    public void OnClick()
    {
        switch (data.itemCategory) // 지니고 있는 데이터 타입에 따라
        {
            // 무기 Setting
            case ItemCategory.Weapon:
                if (level == 0) // 무기가 없을때 초기화 시키기 (생성)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                    GameManager.instance.weaponCount++; // 무기 개수 추가(최대 5개)
                }
                else // 무기가 존재할때
                {
                    float nextDamage = 0;
                    int nextCount = (data.counts.Length != 0 && level < data.maxLevel) ? data.counts[level] : 0;
                    int nextPer = (data.pers.Length != 0 && level < data.maxLevel) ? data.pers[level] : 0;
                    switch (rateIndex)
                    {
                        case 0:
                            nextDamage = (data.damages.Length != 0 && level < data.maxLevel) ? data.damages[level] : 0;
                            break;
                    }

                    weapon.WeaonLevelUp(nextDamage, nextCount, nextPer, level);
                }
                level++;
                break;

            // 기어 Setting
            case ItemCategory.Gear:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                    GameManager.instance.gearCount++; // 기어 개수 추가(최대 5개)
                }
                else
                {
                    float newRate = data.gearRates[level]; // 공속
                    gear.GearLevelUp(newRate);
                    gear.level = level;
                }
                level++;
                break;

            case ItemCategory.Etc:
                switch (data.itemType)
                {
                    case ItemType.Heal:
                        GameManager.instance.health += 15f;
                        break;

                    case ItemType.Gold:
                        Debug.Log("15골드 획득");
                        break;
                }
                break;
        }

        if (level == data.maxLevel)
        {
            GetComponent<Button>().interactable = false;
        }

    }
}
