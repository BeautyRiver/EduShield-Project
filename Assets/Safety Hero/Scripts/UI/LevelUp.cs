using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour
{
    private RectTransform rect;
    public GameObject blackWindow;
    [SerializeField] private List<Item> items;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true).ToList();
    }

    public void Show()
    {
        CurrentData.OnItemCurrentState?.Invoke(); // 이벤트 호출

        blackWindow.SetActive(true); // 검은 배경 On
        Next(); // 섞기
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            btn.interactable = true;
        }

        GameManager.instance.Stop();
        rect.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp); // 음향재생
        AudioManager.instance.EffectBgm(true); // 배경음 필터 끄기
    }
    public void Hide()
    {
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            btn.interactable = false;
        }

        rect.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        {
            GameManager.instance.Resume();
            blackWindow.SetActive(false); // 검은 배경 Off

        });

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select); // 음향재생
        AudioManager.instance.EffectBgm(false); // 배경음 필터 끄기

    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    private void Next()
    {
        // 모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }


        // 활성화 가능한 아이템을 담는 리스트
        List<Item> availableItems = new List<Item>();
        bool allMaxLevel = true;
        bool weaponMaxLevel = true;
        bool gearMaxLevel = true;
        // 현재 무기와 기어의 개수 확인
        int currentWeaponCount = GameManager.instance.weaponCount;
        int currentGearCount = GameManager.instance.gearCount;
        int maxItemCount = GameManager.instance.maxItemCount;

        // 기타 아이템이 아니 아이템들 만렙인지 확인
        foreach (Item item in items)
        {
            // 무기와 기어의 경우, 최대 개수에 도달하지 않았고 최대 레벨이 아닌 경우에만 추가
            if (item.data.itemCategory != ItemData.ItemCategory.Etc )
            {
                if (item.level >= item.data.damages.Length)
                { 
                }
                availableItems.Add(item);
                allMaxLevel = false;

            }

        }
        Debug.Log($"allMaxLevel : {allMaxLevel.ToString()}");

        // 모든 아이템이 만렙이라면 소비 아이템과 골드 아이템만 활성화
        if (allMaxLevel)
        {
            foreach (Item item in items)
            {
                if (item.data.itemType == ItemData.ItemType.Heal || item.data.itemType == ItemData.ItemType.Gold)
                {
                    item.gameObject.SetActive(true);
                }
            }
            return;
        }

        // 활성화할 아이템 수를 결정 (최대 3개)
        int itemsToActivate = Mathf.Min(3, availableItems.Count);

        // 랜덤으로 아이템 선택
        List<int> selectedItem = new List<int>();
        while (selectedItem.Count < itemsToActivate)
        {
            int randIndex = Random.Range(0, availableItems.Count);
            if (!selectedItem.Contains(randIndex))
            {
                selectedItem.Add(randIndex);
            }
        }

        // 선택된 아이템 활성화
        foreach (int index in selectedItem)
        {
            availableItems[index].gameObject.SetActive(true);
        }
    }

}
