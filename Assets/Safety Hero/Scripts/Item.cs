using System.Collections;
using System.Collections.Generic;
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

    private Image icon;
    private Text textLevel;
    private Text textName;
    private Text textDesc;

    private void Awake()
    {
        // 아이콘 설정 및 세팅
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        // 레벨, 이름, 설명 텍스트 설정
        Text[] texts = GetComponentsInChildren<Text>();
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
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]); // 무기 설명글
                break;

            // Gears
            case ItemCategory.Gear:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100); // 기어 설명글
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
                }
                else // 무기가 존재할때
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    nextDamage += data.baseDamage * data.damages[level];
                    nextCount += data.counts[level];

                    weapon.WeaonLevelUp(nextDamage, nextCount);
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
                }
                else
                {
                    float nextRate = data.damages[level];
                    gear.GearLevelUp(nextRate);
                }
                level++;
                break;

            case ItemCategory.Etc:
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }

        
        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }

    }
}
