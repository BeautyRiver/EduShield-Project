using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LevelUpItemSetting : MonoBehaviour
{
    [Header("# 아이템 데이터")]
    public Data data;
    public Weapon weapon;
    public Gear gear;
    public int level;

    [Header("# 등급 시스템")]
    public Rarity rarity; // 이번에 뽑힌 등급 저장용 변수


    [Header("# 이번에 당첨된 강화 옵션")]
    public List<StatType> selectedOptions = new List<StatType>();

    // UI 요소들
    public Image icon;
    public TextMeshProUGUI textRairty;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textDesc;
    public TextMeshProUGUI textLevel;
    private void Awake()
    {
        data.InitializeItemSetting(this);
    }

    private void OnEnable()
    {
        // 아이템 데이터 클래스의 OnEnableSetting 메서드 호출
        data.OnEnableSetting(this);
    }

 
    // 아이템 클릭 시
    public void OnClick()
    {
        // 아이템 데이터 클래스의 OnClickSetting 메서드 호출
        data.OnClickSetting(this);
        UIManager.instance.levelUp.Hide();
    }
   
}
