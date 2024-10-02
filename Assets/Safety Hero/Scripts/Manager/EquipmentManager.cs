using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class EquipmentManager : MonoBehaviour
{
    public static Action onItemCurrentState; // Action 이벤트
    public static Action coolDownImageEvent;

    public enum Category { Weapon, Gear, SwapWeaponUI }

    [Header("Settings")]
    public Category category;
    public GameObject itemGroup;
    private int rowCount;

    [Header("UI References")]
    public List<Image> equipImages;
    public List<Image> coolDownImages;
    public List<TextMeshProUGUI> equipLevelTexts;

    private List<ItemSetting> _data;
    private List<ItemSetting> _sortData;
    private void Awake()
    {
        // 초기화        
        InitialSettings();
    }

    private void OnEnable()
    {
        UpdateCurrentData();
        onItemCurrentState += UpdateCurrentData; // 이벤트 등록
        coolDownImageEvent += CoolDownImageChangeFillAmount;
    }
    private void OnDestroy()
    {
        onItemCurrentState -= UpdateCurrentData;
        coolDownImageEvent -= CoolDownImageChangeFillAmount;
    }

    public void UpdateCurrentData()
    {
        // 모든 아이템을 순회하며 레벨이 0보다 크고 아직 정렬 리스트에 없는 아이템을 추가
        foreach (ItemSetting item in _data)
        {
            if (item.level > 0 && !_sortData.Contains(item))
            {
                _sortData.Add(item);
            }
        }

        // 정렬된 데이터를 기반으로 UI 요소 업데이트
        for (int i = 0; i < _sortData.Count && i < rowCount; i++)
        {
            // 아이템 아이콘 & 텍스트 설정
            equipImages[i].sprite = _sortData[i].data.itemIcon;
            equipLevelTexts[i].text = $"Lv{_sortData[i].level}";

            // UI 요소 활성화
            equipImages[i].gameObject.SetActive(true);
            equipLevelTexts[i].gameObject.SetActive(true);
        }
    }
    private void InitialSettings()
    {
        // 데이터 구조 초기화
        _data = new List<ItemSetting>();
        _sortData = new List<ItemSetting>();
        equipImages = new List<Image>();
        equipLevelTexts = new List<TextMeshProUGUI>();
        coolDownImages = new List<Image>();

        // 아이템 그룹에서 모든 ItemSetting 컴포넌트 가져오기
        ItemSetting[] items = itemGroup.GetComponentsInChildren<ItemSetting>(true);

        // 현재 아이템 그룹에서 무기와 기어 데이터를 분류하여 추가
        foreach (ItemSetting item in items)
        {
            if (item.data.itemCategory == ItemData.ItemCategory.Weapon && (category == Category.Weapon || category == Category.SwapWeaponUI))
                _data.Add(item);

            else if (item.data.itemCategory == ItemData.ItemCategory.Gear && category == Category.Gear)
                _data.Add(item);
        }

        // 현재 장착(먹은 아이템)의 이미지와 레벨 텍스트 컴포넌트 설정
        for (int i = 0; i < transform.childCount; i++)
        {
            Image[] images = transform.GetChild(i).GetComponentsInChildren<Image>();
            equipLevelTexts.Add(images[0].gameObject.GetComponentInChildren<TextMeshProUGUI>(true));
            equipImages.Add(images[1]);

            if (category == Category.SwapWeaponUI)
                coolDownImages.Add(images[2]);
        }

        // 총 행(row) 수 저장
        rowCount = equipImages.Count;

        // 모든 장비 이미지와 레벨 텍스트 오브젝트 비활성화
        for (int i = 0; i < rowCount; i++)
        {
            equipImages[i].gameObject.SetActive(false);
            equipLevelTexts[i].gameObject.SetActive(false);
        }
    }

    private void CoolDownImageChangeFillAmount()
    {
        GameManager gm = GameManager.instance;
        // 모든 쿨다운 이미지를 초기화
        foreach (var item in coolDownImages)
        {
            item.fillAmount = 1f;
        }

        for (int i = 0; i < coolDownImages.Count; i++)
        {
            if (i != gm.weaponIndex)
            {
                // 지역 변수로 i 값을 고정
                int index = i;

                coolDownImages[index].DOKill();
                // 안전한 범위 내에서만 Tween 실행
                coolDownImages[index].DOFillAmount(0f, gm.swapDelay).OnComplete(() =>
                {
                    Vector3 originalVec = equipImages[index].transform.localScale;
                    equipImages[index].rectTransform.DOScale(originalVec * 1.2f, 0.2f).OnComplete(() =>
                    {
                        equipImages[index].rectTransform.DOScale(originalVec, 0.2f);
                        equipImages[index].DOFade(0.15f, 0.1f).SetLoops(2, LoopType.Yoyo);
                    });
                });
            }
        }

    }
}



