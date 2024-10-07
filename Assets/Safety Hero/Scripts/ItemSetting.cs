using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ItemData;

[System.Serializable]
public class IntSerialize
{
    public int[] values;
}

public class ItemSetting : MonoBehaviour
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

    public int _currentLevel;
    public int _maxLevel;

    [SerializeField]
    private List<IntSerialize> statusRateList = new List<IntSerialize>();

    [SerializeField]
    private List<int> rateIdx = new List<int>();

    private Image icon;
    private Image newIcon;
    private TextMeshProUGUI textName;
    private TextMeshProUGUI textDesc;
    private TextMeshProUGUI textLevel;

    private void Awake()
    {
        _maxLevel = data.maxLevel;
        // 아이콘 설정
        icon = GetComponentsInChildren<Image>()[1];
        newIcon = GetComponentsInChildren<Image>()[2];

        icon.sprite = data.itemIcon;

        // 공통 텍스트 필드 설정
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        textName = texts[0];
        textDesc = texts[1];
        textLevel = texts[2];

        if (data.itemCategory == ItemCategory.Weapon)
        {
            // 무기와 기어의 데이터 세팅 (길이가 0 이상인 경우만 추가)
            if (data.damages.Length > 0)
            {
                statusRateList.Add(new IntSerialize { values = data.damages });
                rateIdx.Add(0);
            }
            if (data.counts.Length > 0)
            {
                statusRateList.Add(new IntSerialize { values = data.counts });
                rateIdx.Add(1);
            }
            if (data.pers.Length > 0)
            {
                statusRateList.Add(new IntSerialize { values = data.pers });
                rateIdx.Add(2);
            }
            if (data.sizes.Length > 0)
            {
                statusRateList.Add(new IntSerialize { values = data.sizes });
                rateIdx.Add(3);
            }

            // 최대 인덱스 구하기
            foreach (var item in statusRateList)
                maxmumInsideIdx = Mathf.Max(maxmumInsideIdx, item.values.Length);
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
                        case ItemType.M0_Default:
                            textDesc.text = "만능 소화기\n\n기본 근접 공격";
                            break;
                        case ItemType.M1_Rotating:
                            textDesc.text = "<b><color=#00FAFF>일반 화재</color></b>에 강함\n\n주위를 돌면서 공격";
                            break;
                        case ItemType.M2_MagneticField:
                            textDesc.text = "<b><color=#00FAFF>TEST</color></b>\n\n자기장으로 주변 공격";
                            break;
                        case ItemType.R0_TargetGun:
                            textDesc.text = "<b><color=#00FAFF>휘발유 등의 화재</color></b>에 강함\n\n가장 가까운 적 공격";
                            break;
                        case ItemType.R1_Cannon:
                            textDesc.text = "<b><color=#00FAFF>전기관련 화재</color></b>에 강함\n\n바라보는 방향 반대로 관통 공격";
                            break;
                        case ItemType.R2_Throw:
                            textDesc.text = "<b><color=#00FAFF>식용유 등의 화재</color></b>에 강함\n\n바라보는 방향으로 공격";
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
                        textDesc.text = string.Format(data.itemDesc[outsideRateIdx], increaseRate); // 무기 설명글
                    }
                }
                break;

            // Gears
            case ItemCategory.Gear:
                newIcon.gameObject.SetActive(false);
                textDesc.text = string.Format(data.itemDesc[0], data.gearRates[level]); // 기어 설명글    
                switch (data.itemType)
                {
                    case ItemType.G0_WeaponSpeed:
                        int ran = Random.Range(0, data.itemDesc.Length);
                        textDesc.text = string.Format(data.itemDesc[ran], data.gearRates[level]); // 기어 설명글    
                        break;
                }
                break;

            // Etc
            case ItemCategory.Etc:
                newIcon.gameObject.SetActive(false);
                textDesc.text = string.Format(data.itemDesc[0]); // 아이템 설명글
                textLevel.fontSize = 40;
                switch (data.itemType)
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
        if (level == 0) // 무기 레벨이 0일 때: 무기가 없으므로 무기를 새로 생성한다.
        {
            // 새로운 무기 객체를 생성
            GameObject newWeapon = new GameObject();
            weapon = newWeapon.AddComponent<Weapon>();
            weapon.Init(data); // 무기 초기화

            GameManager.instance.weaponCount++; // 게임 매니저에서 관리하는 전체 무기 개수 증가 (최대 5개)
        }
        else // 무기가 이미 있을 때 (레벨이 0이 아님)
        {
            weapon.WeaonLevelUp(increaseRate, rateIdx[outsideRateIdx], level); // 기존 무기의 레벨을 올림            
            outsideRateIdx++; // 다음 적용할 인덱스를 증가시킴 (ex: Damage -> Count)

            // 인덱스 값이 설정 범위를 넘어가는 경우 계속 조정해주는 로직
            while (outsideRateIdx >= data.itemDesc.Length || insideRateIdx >= statusRateList[outsideRateIdx].values.Length)
            {
                // 만약 외부 인덱스가 data.itemDesc의 범위를 넘을 경우, 인덱스를 0으로 초기화하고 내부 인덱스를 증가
                if (outsideRateIdx >= data.itemDesc.Length)
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
    }
}
