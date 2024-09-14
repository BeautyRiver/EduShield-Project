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
    public int maxmumInsideIdx = 0;
    private float increaseRate = 0;

    [SerializeField]
    private List<int[]> statusRateList = new List<int[]>();

    private Image icon;
    private Image newIcon;
    private TextMeshProUGUI textName;
    private TextMeshProUGUI textDesc;
    private TextMeshProUGUI textLevel;

    private void Awake()
    {
        // 아이콘 설정
        icon = GetComponentsInChildren<Image>()[1];
        newIcon = GetComponentsInChildren<Image>()[2];
        
        icon.sprite = data.itemIcon;

        // 공통 텍스트 필드 설정
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        textName = texts[0];
        textDesc = texts[1];
        textLevel = texts[2];

        if (data.itemCategory == ItemCategory.Weapon || data.itemCategory == ItemCategory.Gear)
        {
                // 무기와 기어의 데이터 세팅
                statusRateList.Add(data.damages);

                statusRateList.Add(data.counts);

                statusRateList.Add(data.pers);

            // 최대 인덱스 구하기
            foreach (var item in statusRateList)
                maxmumInsideIdx = Mathf.Max(maxmumInsideIdx, item.Length);
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
                    newIcon.gameObject.SetActive(true);
                    switch (data.itemType)
                    {
                        case ItemType.Smoke:
                            textDesc.text = "전방의 적을 공격";
                            break;

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
                    newIcon.gameObject.SetActive(false);
                    while (outsideRateIdx < statusRateList.Count && statusRateList[outsideRateIdx].Length == 0)
                    {
                        outsideRateIdx++; // 비어있는 배열을 건너뛰기 위해 증가
                    }

                    if (outsideRateIdx < statusRateList.Count)
                    {
                        increaseRate = statusRateList[outsideRateIdx][insideRateIdx];
                        textDesc.text = string.Format(data.itemDesc[outsideRateIdx], increaseRate); // 무기 설명글
                    }
                }
                break;

            // Gears
            case ItemCategory.Gear:
                newIcon.gameObject.SetActive(false);
                textDesc.text = string.Format(data.itemDesc[0], data.gearRates[level] * 100); // 기어 설명글    
                break;

            // Etc
            case ItemCategory.Etc:
                newIcon.gameObject.SetActive(false);
                textDesc.text = string.Format(data.itemDesc[0]); // 아이템 설명글
                textLevel.fontSize = 40;
                switch (data.itemType)
                {                       
                    case ItemType.Heal:
                        textLevel.text = "특별한 맛";
                        break;
                    case ItemType.Gold:
                        textLevel.text = "부자가 되보자";
                        break;
                }
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
                LevelUpWeapon();
                break;

            // 기어 Setting
            case ItemCategory.Gear:
                LevelUpGear();
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

    private void LevelUpGear()
    {
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
    }

    private void LevelUpWeapon()
    {
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
            // 인덱스 값이 범위를 넘는 경우 계속 조정
            outsideRateIdx++;
            while (outsideRateIdx >= data.itemDesc.Length || insideRateIdx >= statusRateList[outsideRateIdx].Length)
            {
                if (outsideRateIdx >= data.itemDesc.Length)
                {
                    outsideRateIdx = 0;
                    insideRateIdx++;
                }
                if (insideRateIdx >= maxmumInsideIdx)
                    break;

                if (insideRateIdx >= statusRateList[outsideRateIdx].Length)
                    outsideRateIdx++;
            }
        }
        level++;
    }
}
