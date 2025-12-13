using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Data;

[System.Serializable]
public class S_int
{
    public int[] values;
}

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

    [Header("# 현재 강화Index")]
    public int outsideRateIdx = 0;
    public int insideRateIdx = 0;
    public int maxmumInsideIdx = 0;
    public float increaseRate = 0;

    public int _currentLevel;
    
    // UI 요소들
    public Image icon;
    public TextMeshProUGUI textRairty;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textDesc;
    public TextMeshProUGUI textLevel;

    [Header("# 이번에 당첨된 강화 옵션")]
    public List<StatType> selectedOptions = new List<StatType>();

    private void Awake()
    {

        // 공통 텍스트 필드 설정
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        textName = texts[0];
        textDesc = texts[1];
        textLevel = texts[2];
        textName.text = data.itemName;

        // 아이템 데이터 클래스의 초기화 메서드 호출
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
