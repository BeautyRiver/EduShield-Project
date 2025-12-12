using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Etc", menuName = "Scriptable Objects/EtcData")]
// EtcData.cs
public class EtcData : Data
{
    public enum EtcType
    {
        E0_Heal = 200, E1_Gold,
    }

    public EtcType eType;    

    // 기타 필요한 필드...


    public override void InitializeItemSetting(LevelUpItemSetting itemSetting)
    {
        // 기타 아이템의 경우 특별한 초기화가 필요하지 않음
    }

    public override void OnEnableSetting(LevelUpItemSetting itemSetting)
    {
        itemSetting.newIcon.gameObject.SetActive(false);
        itemSetting.textDesc.text = growthDesc[0];
        itemSetting.textLevel.fontSize = 40;

        switch (eType)
        {
            case EtcType.E0_Heal:
                itemSetting.textLevel.text = "특별한 맛";
                break;
            case EtcType.E1_Gold:
                itemSetting.textLevel.text = "부자가 되보자";
                break;
            default:
                itemSetting.textLevel.text = "아이템";
                break;
        }
    }

    public override void OnClickSetting(LevelUpItemSetting itemSetting)
    {
        switch (eType)
        {
            case EtcType.E0_Heal:
                GameManager.instance.player.health = Mathf.Min(GameManager.instance.player.maxHealth, GameManager.instance.player.health + 15f);
                break;
            case EtcType.E1_Gold:
                Debug.Log("15골드 획득");
                break;
                // 기타 타입에 대한 처리
        }
    }
}
