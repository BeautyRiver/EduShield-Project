using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Etc", menuName = "Scriptble Object/EtcData")]
// EtcData.cs
public class EtcData : ItemData
{
    public enum EtcType
    {
        E0_Heal = 200, E1_Gold,
    }

    public EtcType eType;    

    // 기타 필요한 필드...

    protected override void OnValidate()
    {
        // 기타 아이템의 경우 특별한 검증이 필요하지 않음
    }

    public override void InitializeItemSetting(ItemSetting itemSetting)
    {
        // 기타 아이템의 경우 특별한 초기화가 필요하지 않음
    }

    public override void OnEnableSetting(ItemSetting itemSetting)
    {
        itemSetting.NewIcon.gameObject.SetActive(false);
        itemSetting.TextDesc.text = itemDesc[0];
        itemSetting.TextLevel.fontSize = 40;

        switch (eType)
        {
            case EtcType.E0_Heal:
                itemSetting.TextLevel.text = "특별한 맛";
                break;
            case EtcType.E1_Gold:
                itemSetting.TextLevel.text = "부자가 되보자";
                break;
            default:
                itemSetting.TextLevel.text = "아이템";
                break;
        }
    }

    public override void OnClickSetting(ItemSetting itemSetting)
    {
        switch (eType)
        {
            case EtcType.E0_Heal:
                GameManager.instance.health = Mathf.Min(GameManager.instance.maxHealth, GameManager.instance.health + 15f);
                break;
            case EtcType.E1_Gold:
                Debug.Log("15골드 획득");
                break;
                // 기타 타입에 대한 처리
        }
    }
}
