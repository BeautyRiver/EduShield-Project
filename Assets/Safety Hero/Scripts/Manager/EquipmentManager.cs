using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EquipmentType
{
    Weapon,
    Gear
}

public class EquipmentManager : MonoBehaviour
{
    [Header("# Settings")]
    public EquipmentType type; // 인스펙터에서 무기용인지 기어용인지 설정

    [Header("# UI References")]
    // 슬롯의 부모 오브젝트 (Grid Layout Group이 있는 곳)
    [SerializeField] private List<Image> icons = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> textLevels = new List<TextMeshProUGUI>();

    private void Awake()
    {
        // 1. UI 컴포넌트들을 자동으로 찾아 연결 (이름이나 구조에 따라 수정 필요)
        // 예시 구조: EquipmentManager -> SlotParent(Grid) -> Slot(Image + Text)

        var slotParent = transform;
        // 만약 직접 할당하고 싶다면 이 부분은 주석 처리하세요.
        if (slotParent != null)
        {
            // 부모 아래의 모든 슬롯을 찾음 (비활성화된 것도 포함)
            for (int i = 1; i < slotParent.childCount; i++)
            {
                Transform slot = slotParent.GetChild(i);                
                Image[] images = slot.GetComponentsInChildren<Image>(true);

                // 배경 이미지가 있다면 images[1], 없다면 images[0] (구조에 맞춰 조정 필요)
                if (images.Length > 1) icons.Add(images[1]);
                else icons.Add(images[0]);

                textLevels.Add(slot.GetComponentInChildren<TextMeshProUGUI>(true));
            }
        }
    }

    private void OnEnable()
    {
        // UI가 켜질 때마다 데이터 갱신
        UpdateUI();
    }

    public void UpdateUI()
    {
        // 플레이어가 없으면 패스 (게임 시작 전 등)
        if (GameManager.instance == null || GameManager.instance.player == null)
            return;

        switch (type)
        {
            case EquipmentType.Weapon:
                LoadWeapons();
                break;
            case EquipmentType.Gear:
                LoadGears();
                break;
        }
    }

    private void LoadWeapons()
    {
        // 플레이어가 가진 모든 Weapon 컴포넌트를 가져옴
        Weapon[] weapons = GameManager.instance.player.GetComponentsInChildren<Weapon>(true);

        for (int i = 0; i < icons.Count; i++)
        {
            if (i < weapons.Length)
            {
                // 데이터가 있으면 UI 켜고 정보 입력
                icons[i].sprite = weapons[i].currentBulletData.itemIcon; // 아이콘 설정
                textLevels[i].text = $"Lv.{weapons[i].level}"; // 레벨 설정

                // 슬롯의 부모(슬롯 자체)를 켜야 함. icons[i]는 이미지 컴포넌트이므로 그 부모나 본인을 활성화
                // 여기서는 아이콘 이미지만 껐다 켰다 하는 게 아니라, 슬롯 전체를 제어하는 게 좋음
                icons[i].transform.parent.gameObject.SetActive(true);
                icons[i].gameObject.SetActive(true);
            }
            else
            {
                // 데이터가 없으면 슬롯 숨기기 (혹은 빈 슬롯 이미지로 변경)
                // icons[i].transform.parent.gameObject.SetActive(false); // 슬롯 자체를 숨기거나
                icons[i].gameObject.SetActive(false); // 아이콘만 숨기거나 (배경은 유지)
                textLevels[i].text = "";
            }
        }
    }

    private void LoadGears()
    {
        // 플레이어가 가진 모든 Gear 컴포넌트를 가져옴
        Gear[] gears = GameManager.instance.player.GetComponentsInChildren<Gear>(true);

        for (int i = 0; i < icons.Count; i++)
        {
            if (i < gears.Length)
            {
                icons[i].sprite = gears[i].gearData.itemIcon;
                textLevels[i].text = $"Lv.{gears[i].level}";

                icons[i].transform.parent.gameObject.SetActive(true);
                icons[i].gameObject.SetActive(true);
            }
            else
            {
                icons[i].gameObject.SetActive(false);
                textLevels[i].text = "";
            }
        }
    }
}