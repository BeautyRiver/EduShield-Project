using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour
{
    private RectTransform rect;
    [SerializeField] private Item[] items;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    public void Show()
    {
        Next();

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
        GameManager.instance.Resume());

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

        // 소비 아이템과 골드 아이템 외의 모든 아이템이 만렙인지 확인
        List<Item> availableItems = new List<Item>();
        bool allMaxLevel = true;

        foreach (Item item in items)
        {
            if (item.data.itemType != ItemData.ItemType.Heal &&
                item.data.itemType != ItemData.ItemType.Gold &&
                item.level < item.data.damages.Length)
            {
                availableItems.Add(item);
                allMaxLevel = false;
            }
        }

        if (allMaxLevel)
        {
            // 모든 아이템이 만렙이라면 소비 아이템과 골드 아이템만 활성화
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
