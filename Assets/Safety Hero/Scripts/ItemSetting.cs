using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static DataGuide;

[System.Serializable]
public class S_int
{
    public int[] values;
}

public class ItemSetting : MonoBehaviour
{
    [Header("# 아이템 데이터")]
    public DataGuide itemData;
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

    // UI 요소들
    private Image icon;
    private Image newIcon;
    private TextMeshProUGUI textName;
    private TextMeshProUGUI textDesc;
    private TextMeshProUGUI textLevel;

    // 프로퍼티를 통해 외부에서 접근 가능하도록 설정
    public Image Icon => icon;
    public Image NewIcon => newIcon;
    public TextMeshProUGUI TextName => textName;
    public TextMeshProUGUI TextDesc => textDesc;
    public TextMeshProUGUI TextLevel => textLevel;

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

        // 아이템 데이터 클래스의 초기화 메서드 호출
        itemData.InitializeItemSetting(this);
    }

    private void OnEnable()
    {
        // 아이템 데이터 클래스의 OnEnableSetting 메서드 호출
        itemData.OnEnableSetting(this);
    }

    // 아이템 클릭 시
    public void OnClick()
    {
        // 아이템 데이터 클래스의 OnClickSetting 메서드 호출
        itemData.OnClickSetting(this);

        EquipmentManager.onItemCurrentState?.Invoke(); // 이벤트 호출

        if (level == itemData.maxLevel)
        {
            GetComponent<Button>().interactable = false;
        }
    }
   
}
