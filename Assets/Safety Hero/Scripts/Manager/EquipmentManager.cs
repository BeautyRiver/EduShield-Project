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

    public enum Category { Weapon, Gear }

    [Header("Settings")]
    public Category category;
    public GameObject itemGroup;
    private int rowCount;

    [Header("UI References")]
    public List<Image> equipImages;
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
    }
    private void OnDestroy()
    {
        onItemCurrentState -= UpdateCurrentData;
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
            // 만렙일때            
            if (_sortData[i].level == _sortData[i]._maxLevel)
                equipLevelTexts[i].text = $"<color=yellow>Max</color>";
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

        // 아이템 그룹에서 모든 ItemSetting 컴포넌트 가져오기
        ItemSetting[] items = itemGroup.GetComponentsInChildren<ItemSetting>(true);

        // 현재 아이템 그룹에서 무기와 기어 데이터를 분류하여 추가
        foreach (ItemSetting item in items)
        {
            if (item.data.itemCategory == ItemData.ItemCategory.Weapon && category == Category.Weapon)
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

   
}



