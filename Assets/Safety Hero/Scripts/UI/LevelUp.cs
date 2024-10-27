using DarkTonic.MasterAudio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour
{
    [Header("# UI manager")]
    public UIManager uiManager;
    [Header("# ---------------------")]
    private RectTransform rect;
    public Image blackWindow;
    public float showLeveUpDuration;
    [SerializeField] private List<ItemSetting> items;
    public List<ItemSetting> availableItems;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<ItemSetting>(true).ToList();
    }

    public void Show()
    {
        //SelectorController.SelectorEvent?.Invoke(); // Selector 이밴트 호출(배치)

        uiManager.BlackWindowFadeIn(); // 검은 배경 On
        Next(); // 섞기
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            btn.interactable = true;
        }

        GameManager.instance.Stop();
        rect.DOAnchorPos(Vector3.zero, showLeveUpDuration).SetEase(Ease.Linear).SetUpdate(true);

        MasterAudio.PlaySound("LevelUp");

        // 비율 기반으로 BGM 볼륨 감소
        /*float currentBGMVolume = PlayerPrefs.GetFloat("BGM");
        MasterAudio.PlaylistMasterVolume = currentBGMVolume * 0.25f;*/
    }
    public void Hide()
    {
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            btn.interactable = false;
        }

        uiManager.BlackWindowFadeaOut(); // 검은 배경 Off

        rect.DOAnchorPos(new Vector3(0, -1500f, 0), showLeveUpDuration).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
        {
            GameManager.instance.Resume();
        });

        MasterAudio.PlaySound("Select");

        // 원래 BGM 볼륨으로 복구
        //MasterAudio.PlaylistMasterVolume = PlayerPrefs.GetFloat("BGM");
    }

    public void FirstGiveWeapon(int index)
    {
        items[index].OnClick();        
    }

    private void Next()
    {
        // 모든 아이템 비활성화
        foreach (ItemSetting item in items)
        {
            item.gameObject.SetActive(false);
        }

        // 활성화 가능한 아이템을 담는 리스트
        availableItems = new List<ItemSetting>();

        bool allMaxLevel = true;

        foreach (ItemSetting item in items)
        {
            switch (item.bulletData.itemCategory)
            {
                case BulletData.ItemCategory.Bullet:
                    // 이미 획득한 무기이거나, 새로운 무기를 획득할 수 있는 경우
                    if (item.level > 0 || GameManager.instance.weaponCount < GameManager.instance.maxItemCount)
                    {
                        if (item.level < item.bulletData.maxLevel)
                        {
                            availableItems.Add(item);
                            allMaxLevel = false;
                        }
                    }
                    break;

                case BulletData.ItemCategory.Gear:
                    // 이미 획득한 기어이거나, 새로운 기어를 획득할 수 있는 경우
                    if (item.level > 0 || GameManager.instance.gearCount < GameManager.instance.maxItemCount)
                    {
                        if (item.level < item.bulletData.maxLevel)
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
            foreach (ItemSetting item in items)
            {
                if (item.bulletData.itemCategory == BulletData.ItemCategory.Etc)
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
