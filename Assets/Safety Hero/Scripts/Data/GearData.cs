using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

[CreateAssetMenu(fileName = "Gear", menuName = "Scriptble Object/GearData")]
public class GearData : DataGuide
{    
    [Tab("# 기어 능력치")]
    [Header("배율방식 / 50 = 50%증가")]
    public int[] gearRates;
    
    [Tab("# 기어 관련")]
    public GameObject gearType;

    // 에디터에서 값이 변경될 때 자동으로 호출
    protected override void OnValidate()
    {
        maxLevel = gearRates.Length;        
    }

    public override void InitializeItemSetting(ItemSetting itemSetting)
    {
        // 기어의 경우 특별한 초기화가 필요하지 않음
    }

    public override void OnEnableSetting(ItemSetting itemSetting)
    {
        if (itemSetting.TextLevel != null)
        {
            itemSetting.TextLevel.text = "Now LV. " + (itemSetting.level); // 레벨 표기
        }

        if (itemSetting.level == 0)
        {
            itemSetting.NewIcon.gameObject.SetActive(true);
            itemSetting.TextDesc.text = "<color=#99FF8A>새로운 강화!</color>\n\n<size=90%>" +
                string.Format(itemDesc[0], gearRates[itemSetting.level]) + "</size>";
        }
        else
        {
            itemSetting.NewIcon.gameObject.SetActive(false);
            itemSetting.TextDesc.text = string.Format(itemDesc[0], gearRates[itemSetting.level]); // 기어 설명글
        }
    }

    public override void OnClickSetting(ItemSetting itemSetting)
    {
        if (itemSetting.level == 0)
        {
            GameObject newGear = Instantiate(gearType);
            itemSetting.gear = newGear.GetComponent<Gear>();
            itemSetting.gear.Init(this);
            GameManager.instance.gearCount++;
        }
        else
        {
            float newRate = gearRates[itemSetting.level];
            itemSetting.gear.GearLevelUp(newRate);
            itemSetting.gear.level = itemSetting.level;
        }

        itemSetting.level++;
    }
}
