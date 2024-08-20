using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;
    

    private Image icon;
    private Text textLevel;
    private Text textName;
    private Text textDesc;

    private void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName;
    }

    private void OnEnable()
    {
        // 설명글 작성
        textLevel.text = "Lv." + (level + 1); // 레벨 표기
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]); // 아이템 설명글
                break;

            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100); // 아이템 설명글
                break;
            default:
                textDesc.text = string.Format(data.itemDesc); // 아이템 설명글
                break;
        }
    }    

    public void OnClick()
    {
        switch (data.itemType)
        {
            // 무기 Setting
            case ItemData.ItemType.Melee:                
            case ItemData.ItemType.Range:
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
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
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

            case ItemData.ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }


        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }

    }
}
