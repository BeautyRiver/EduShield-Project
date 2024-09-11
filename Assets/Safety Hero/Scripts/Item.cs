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

    [Header("# 현재 강화Index")]
    public int outsideRateIdx = 0;
    public int insideRateIdx = 0;
    public float increaseRate = 0;

    [SerializeField]
    private List<int[]> statusRateList = new List<int[]>();

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
                if (data.damages.Length > 0)
                    statusRateList.Add(data.damages);

                if (data.counts.Length > 0)
                    statusRateList.Add(data.counts);

                if (data.pers.Length > 0)
                    statusRateList.Add(data.pers);

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
                // 레벨이 0이 아닐때
                else
                {
                    increaseRate = statusRateList[outsideRateIdx][insideRateIdx];                    
                    textDesc.text = string.Format(data.itemDesc[outsideRateIdx], increaseRate); // 무기 설명글
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
                    weapon.WeaonLevelUp(increaseRate, outsideRateIdx, level);

                    outsideRateIdx += 1;

                    if (outsideRateIdx >= data.itemDesc.Length)
                    {
                        outsideRateIdx = 0;
                        insideRateIdx += 1;
                    }

                    // 스탯레벨업이 최대에 도달하면 리스트에서 삭제
                    if (insideRateIdx >= statusRateList[outsideRateIdx].Length)
                    {
                        Debug.Log($"Removed : statusRateList[{outsideRateIdx}]");
                        statusRateList.RemoveAt(outsideRateIdx);                        
                    }
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
