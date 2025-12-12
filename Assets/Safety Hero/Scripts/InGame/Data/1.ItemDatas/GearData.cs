using UnityEngine;
using VInspector;

[CreateAssetMenu(fileName = "Gear", menuName = "Scriptable Objects/GearData")]
public class GearData : Data
{
    [Tab("# 기어 능력치 (무한 성장)")]
    [Header("레벨업 당 기본 증가량 (단위: %)")]
    // 예: 10 이라고 적으면 10% 증가 (Common 기준)
    public float rateGrowth;

    [Tab("# 기어 관련")]
    public GameObject gearType;
    

    public override void InitializeItemSetting(LevelUpItemSetting itemSetting)
    {
        // 초기화 로직 없음
    }

    // [핵심] 등급 뽑기 & 텍스트 설정
    public override void OnEnableSetting(LevelUpItemSetting itemSetting)
    {
        // 1. 랜덤 등급 뽑기 & 배율 가져오기
        itemSetting.rarity = Utils.GetRandomRarity();
        float multiplier = Utils.GetRarityMultiplier(itemSetting.rarity);
        Color color = Utils.GetRarityColor(itemSetting.rarity);

        // 2. 텍스트 & 색상 설정
        itemSetting.textName.color = color; // 이름 색깔 변경 (전설은 금색!)

        if (itemSetting.level == 0)
        {
            itemSetting.textLevel.text = "New!";
            itemSetting.newIcon.gameObject.SetActive(true);

            itemSetting.textDesc.text = defalutDesc;
        }
        else
        {
            itemSetting.textLevel.text = itemSetting.rarity.ToString();
            itemSetting.newIcon.gameObject.SetActive(false);

            // 3. 증가량 계산 및 설명글 작성            
            string desc = "";

            if (rateGrowth > 0)
            {
                float finalRate = rateGrowth * multiplier;
                itemSetting.textDesc.text = $"Stat +{finalRate:F1}%";
            }

            // 설명 텍스트 적용
            itemSetting.textDesc.text = desc;
        }     
    }

    // [클릭] 실제 적용
    public override void OnClickSetting(LevelUpItemSetting itemSetting)
    {
        float multiplier = Utils.GetRarityMultiplier(itemSetting.rarity);
        float finalIncrease = rateGrowth * multiplier;

        if (itemSetting.level == 0)
        {
            // 새 기어 장착
            GameObject newGear = Instantiate(gearType, GameManager.instance.player.transform);
            itemSetting.gear = newGear.GetComponent<Gear>();
            itemSetting.gear.Init(this); // Init 안에서도 초기 수치 적용 필요
            GameManager.instance.gearCount++;
            itemSetting.gear.GearLevelUp(finalIncrease);
        }
        else
        {
            itemSetting.gear.GearLevelUp(finalIncrease);
        }

        itemSetting.level++;
    }
}