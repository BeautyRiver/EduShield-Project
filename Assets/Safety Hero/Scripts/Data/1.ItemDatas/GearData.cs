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
        // 1. 공통: 랜덤 등급 & 색상
        itemSetting.rarity = Utils.GetRandomRarity();
        float multiplier = Utils.GetRarityMultiplier(itemSetting.rarity);
        Color color = Utils.GetRarityColor(itemSetting.rarity);

        itemSetting.icon.sprite = itemSetting.data.itemIcon;
        itemSetting.textName.text = itemSetting.data.itemName;
        
        string desc = "";

        // Case 1: New -> "0%에서 시작"
        if (itemSetting.level == 0)
        {
            itemSetting.textRairty.text = "New Gear";
            itemSetting.textLevel.text = "New!";            

            // 처음 획득 시 적용될 수치 계산
            float initialRate = rateGrowth * multiplier;

            desc += $"Stat: 0% <color=#00FF00>→ {initialRate:F1}%</color>";

        }
        // Case 2: Upgrade -> "현재%에서 증가"
        else
        {
            itemSetting.textRairty.color = color;
            itemSetting.textRairty.text = itemSetting.rarity.ToString();
            itemSetting.textLevel.text = $"Lv. {itemSetting.level}";

            float currentRate = itemSetting.gear.accumulatedRate * 100f;
            float addedRate = (rateGrowth * multiplier);
            float nextRate = currentRate + addedRate;

            desc += $"Stat: {currentRate:F1}% <color=#00FF00>→ {nextRate:F1}%</color>";
        }

        itemSetting.textDesc.text = desc;
    }

    // [클릭] 실제 적용
    public override void OnClickSetting(LevelUpItemSetting itemSetting)
    {
        float multiplier = Utils.GetRarityMultiplier(itemSetting.rarity);
        float finalIncrease = rateGrowth * multiplier;

        if (itemSetting.level == 0)
        {
            // 새 기어 장착
            GameObject newGear = Instantiate(gearType, GameManager.instance.player.gearObject.transform);
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