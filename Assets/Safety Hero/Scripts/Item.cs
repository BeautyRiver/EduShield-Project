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

    private static int weaponCount = 0; // 획득한 무기 개수
    private static int gearCount = 0;   // 획득한 기어 개수
    private static int maxItemCount = 5; // 무기와 기어의 최대 개수

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

                }
                else
                {
                    textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]); // 무기 설명글
                }
                break;

            // Gears
            case ItemCategory.Gear:
                if (ItemType.Glove == data.itemType)
                {
                    textDesc.text = string.Format(data.itemDesc, data.weaponSpeeds[level] * 100); // 기어 설명글
                }
                else if (ItemType.Shoe == data.itemType)
                {
                    textDesc.text = string.Format(data.itemDesc, data.speeds[level] * 100); // 기어 설명글
                }
                break;

            // Etc
            case ItemCategory.Etc:
                textDesc.text = string.Format(data.itemDesc); // 아이템 설명글
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
                    
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;
                    int nextPer = 0;
                    nextDamage += data.baseDamage * data.damages[level];
                    nextCount += data.counts[level];
                    nextPer += data.pers[level];
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
                    switch (data.itemType)
                    {
                        case ItemType.Glove:
                            float atkSpdMult = data.weaponSpeeds[level]; // 공속
                            gear.GearLevelUp(data.itemType, atkSpdMult);
                            break;
                        case ItemType.Shoe:
                            {
                                float spdMult = data.speeds[level];
                                gear.GearLevelUp(data.itemType, spdMult);
                                break;
                            }
                    }
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
