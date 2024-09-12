using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour
{
    private RectTransform rect;
    public Image blackWindow;
    [SerializeField] private List<Item> items;
    public List<Item> availableItems;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true).ToList();
    }

    public void Show()
    {
        CurrentData.OnItemCurrentState?.Invoke(); // 이벤트 호출

        blackWindow.DOFade(0.75f, 0.5f).SetUpdate(true); // 검은 배경 On
        Next(); // 섞기
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            btn.interactable = true;
        }

        GameManager.instance.Stop();
        rect.DOAnchorPos(Vector3.zero, 0.1f).SetEase(Ease.Linear).SetUpdate(true);

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

        blackWindow.DOFade(0f, 0.5f).SetUpdate(true); // 검은 배경 Off

        rect.DOAnchorPos(new Vector3(0, -1500f, 0), 0.1f).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
        {
            GameManager.instance.Resume();
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
        availableItems = new List<Item>();

        bool allMaxLevel = true;

        foreach (Item item in items)
        {
            switch (item.data.itemCategory)
            {
                case ItemData.ItemCategory.Weapon:
                    // 이미 획득한 무기이거나, 새로운 무기를 획득할 수 있는 경우
                    if (item.level > 0 || GameManager.instance.weaponCount < GameManager.instance.maxItemCount)
                    {
                        if (item.level < item.data.maxLevel)
                        {
                            availableItems.Add(item);
                            allMaxLevel = false;
                        }
                    }
                    break;

                case ItemData.ItemCategory.Gear:
                    // 이미 획득한 기어이거나, 새로운 기어를 획득할 수 있는 경우
                    if (item.level > 0 || GameManager.instance.gearCount < GameManager.instance.maxItemCount)
                    {
                        if (item.level < item.data.maxLevel - 1)
                        {
                            availableItems.Add(item);
                            allMaxLevel = false;
                        }
                    }
                    break;
            }
        }

        // 모든 무기와 기어가 최대 레벨에 도달했다면 Etc 아이템만 활성화
        if (allMaxLevel)
        {
            foreach (Item item in items)
            {
                if (item.data.itemCategory == ItemData.ItemCategory.Etc)
                {
                    availableItems.Add(item);
                }
            }            
        }

        // 활성화할 아이템 수를 결정 (최대 3개)
        int itemsToActivate = Mathf.Min(3, availableItems.Count);

        // 랜덤으로 아이템 선택
        List<int> selectedItems = new List<int>();
        while (selectedItems.Count < itemsToActivate)
        {
            int randIndex = Random.Range(0, availableItems.Count);
            if (!selectedItems.Contains(randIndex))
            {
                selectedItems.Add(randIndex);
            }
        }

        // 선택된 아이템 활성화
        foreach (int index in selectedItems)
        {
            availableItems[index].gameObject.SetActive(true);
        }
    }

}
