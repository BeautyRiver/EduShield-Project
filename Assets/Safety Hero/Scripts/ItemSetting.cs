using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static ItemData;

[System.Serializable]
public class S_int
{
    public int[] values;
}

public class ItemSetting : MonoBehaviour
{
    [Header("# 아이템 데이터")]
    public ItemData itemData;
    public Weapon weapon;
    public Gear gear;
    public int level;

    [Header("# 현재 강화Index")]
    public int outsideRateIdx = 0;
    public int insideRateIdx = 0;
    public int maxmumInsideIdx = 0;
    public float increaseRate = 0;

    public int _currentLevel;
    public int _maxLevel;

    public List<S_int> statusRateList = new List<S_int>();
    public List<int> rateIdx = new List<int>();

    public Image icon;
    public Image newIcon;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textDesc;
    public TextMeshProUGUI textLevel;

    private void Awake()
    {
        _maxLevel = itemData.maxLevel;
        // 아이콘 설정
        icon = GetComponentsInChildren<Image>()[1];
        newIcon = GetComponentsInChildren<Image>()[2];

        icon.sprite = itemData.itemIcon;

        // 공통 텍스트 필드 설정
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        textName = texts[0];
        textDesc = texts[1];
        textLevel = texts[2];
        textName.text = itemData.itemName;

        if (itemData is BulletData bulletData)
        {
            // 무기와 기어의 데이터 세팅 (길이가 0 이상인 경우만 추가)
            if (bulletData.damages.Length > 0)
            {
                statusRateList.Add(new S_int { values = bulletData.damages });
                rateIdx.Add(0);
            }
            if (bulletData.counts.Length > 0)
            {
                statusRateList.Add(new S_int { values = bulletData.counts });
                rateIdx.Add(1);
            }
            if (bulletData.pers.Length > 0)
            {
                statusRateList.Add(new S_int { values = bulletData.pers });
                rateIdx.Add(2);
            }
            if (bulletData.sizes.Length > 0)
            {
                statusRateList.Add(new S_int { values = bulletData.sizes });
                rateIdx.Add(3);
            }

            // 최대 인덱스 구하기
            foreach (var item in statusRateList)
                maxmumInsideIdx = Mathf.Max(maxmumInsideIdx, item.values.Length);
        }       
    }

    private void OnEnable()
    {
        // 설명글 세팅
        if (itemData.Category != ItemCategory.Etc && textLevel != null)
        {
            textLevel.text = "Lv." + (level + 1); // 레벨 표기
        }

        switch (itemData.Category)
        {
            // Weapons
            case ItemCategory.Bullet:
                if (level == 0)
                {
                    newIcon.gameObject.SetActive(true);
                    switch (itemData.Type)
                    {
                        case ItemType.M0_Default:
                            textDesc.text = "<color=#99FF8A>새로운 무기!</color>\r\n\r\n<size=90%>좌우로 적을 관통 공격</size>";
                            break;
                        case ItemType.M1_Rotating:
                            textDesc.text = "<color=#99FF8A>새로운 무기!</color>\r\n\r\n<size=90%>주변을 회전하며 공격</size>";
                            break;
                        case ItemType.M2_MagneticField:
                            textDesc.text = "<color=#99FF8A>새로운 무기!</color>\r\n\r\n<size=90%>범위 내 적 지속 공격</size>";
                            break;
                        case ItemType.R0_TargetGun:
                            textDesc.text = "<color=#99FF8A>새로운 무기!</color>\r\n\r\n<size=90%>가장 가까운 적 공격</size>";
                            break;
                        case ItemType.R1_Cannon:
                            textDesc.text = "<color=#99FF8A>새로운 무기!</color>\r\n\r\n<size=90%>반대 방향으로 강력한 관통 공격</size>";
                            break;
                        case ItemType.R2_Throw:
                            textDesc.text = "<color=#99FF8A>새로운 무기!</color>\r\n\r\n<size=90%>바라보는 방향으로 공격</size>";
                            break;
                    }
                }
                else
                {
                    newIcon.gameObject.SetActive(false);
                    while (outsideRateIdx < statusRateList.Count && statusRateList[outsideRateIdx].values.Length == 0)
                    {
                        outsideRateIdx++; // 비어있는 배열을 건너뛰기 위해 증가
                    }

                    if (outsideRateIdx < statusRateList.Count)
                    {
                        increaseRate = statusRateList[outsideRateIdx].values[insideRateIdx];
                        textDesc.text = string.Format(itemData.itemDesc[outsideRateIdx], increaseRate); // 무기 설명글
                    }
                }
                break;

            // Gears
            case ItemCategory.Gear:
                if (level == 0)
                {
                    newIcon.gameObject.SetActive(true);
                    textDesc.text = "<color=#99FF8A>새로운 강화!</color>\r\n\r\n<size=90%>" + string.Format(itemData.itemDesc[0], gearData.gearRates[level]) + "</size>";
                }
                else
                {
                    newIcon.gameObject.SetActive(false);
                    textDesc.text = string.Format(itemData.itemDesc[0], gearData.gearRates[level]); // 기어 설명글    
                }
                break;

            // Etc
            case ItemCategory.Etc:
                newIcon.gameObject.SetActive(false);
                textDesc.text = string.Format(itemData.itemDesc[0]); // 아이템 설명글
                textLevel.fontSize = 40;
                switch (itemData.Type)
                {
                    case ItemType.E0_Heal:
                        textLevel.text = "특별한 맛";
                        break;
                    case ItemType.E1_Gold:
                        textLevel.text = "부자가 되보자";
                        break;
                }
                break;
        }
    }

    // 아이템 클릭 시
    public void OnClick()
    {
        switch (itemData.Category) // 지니고 있는 데이터 타입에 따라
        {
            // 무기 Setting
            case ItemCategory.Bullet:
                LevelUpWeapon();
                break;

            // 기어 Setting
            case ItemCategory.Gear:
                LevelUpGear();
                break;

            case ItemCategory.Etc:
                switch (itemData.Type)
                {
                    case ItemType.E0_Heal:
                        GameManager.instance.health = Mathf.Min(GameManager.instance.maxHealth, GameManager.instance.health + 15f);
                        break;

                    case ItemType.E1_Gold:
                        Debug.Log("15골드 획득");
                        break;
                }
                break;
        }

        EquipmentManager.onItemCurrentState?.Invoke(); // 이벤트 호출
        if (level == itemData.maxLevel)
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
            gear.Init(gearData);
            GameManager.instance.gearCount++; // 기어 개수 추가(최대 5개)
        }
        else
        {
            float newRate = gearData.gearRates[level]; // 공속
            gear.GearLevelUp(newRate);
            gear.level = level;
        }
        level++;
    }

    private void LevelUpWeapon()
    {
        // 무기 레벨이 0일 때: 무기가 없으므로 무기를 새로 생성한다.
        if (level == 0) 
        {
            // 새로운 무기 객체를 생성
            GameObject newWeapon = Instantiate(itemData.weaponType);
            weapon = newWeapon.GetComponent<Weapon>();
            
            weapon.Init(itemData); // 무기 초기화
            GameManager.instance.weaponCount++; // 게임 매니저에서 관리하는 전체 무기 개수 증가 (최대 5개)
        }

        // 무기가 이미 있을 때 (레벨이 0이 아님)
        else
        {
            weapon.WeaponLevelUp(increaseRate, rateIdx[outsideRateIdx], level); // 기존 무기의 레벨을 올림            
            outsideRateIdx++; // 다음 적용할 인덱스를 증가시킴 (ex: Damage -> Count)

            // 인덱스 값이 설정 범위를 넘어가는 경우 계속 조정해주는 로직
            while (outsideRateIdx >= itemData.itemDesc.Length || insideRateIdx >= statusRateList[outsideRateIdx].values.Length)
            {
                // 만약 외부 인덱스가 gearData.itemDesc의 범위를 넘을 경우, 인덱스를 0으로 초기화하고 내부 인덱스를 증가
                if (outsideRateIdx >= itemData.itemDesc.Length)
                {
                    outsideRateIdx = 0;
                    insideRateIdx++;
                }

                // 내부 인덱스가 최대값을 초과하는 경우 루프를 종료
                if (insideRateIdx >= maxmumInsideIdx)
                    break;

                // 내부 인덱스가 현재 외부 인덱스에 해당하는 리스트의 길이를 초과한 경우 외부 인덱스를 증가
                if (insideRateIdx >= statusRateList[outsideRateIdx].values.Length)
                    outsideRateIdx++;
            }
        }
        level++;  // 무기 레벨을 하나 증가
        _currentLevel = level;
    }
}
