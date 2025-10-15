using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TMP_InputField;

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
    private GameManager gm;
    private void Awake()
    {     
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<ItemSetting>(true).ToList();
    }

    public void Show()
    {
        GameManager.instance.ChangeState(GameState.LevelUp); // 레벨업 중
        gameObject.SetActive(true);
        uiManager.BlackWindowFadeIn(); // 검은 배경 On
        Next(); // 섞기
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            btn.interactable = true;
        }
        buttons[0].Select();
        MasterAudio.PlaySound("LevelUp");

        // 비율 기반으로 BGM 볼륨 감소
        /*float currentBGMVolume = PlayerPrefs.GetFloat("BGM");
        MasterAudio.PlaylistMasterVolume = currentBGMVolume * 0.25f;*/
    }
    public void Hide()
    {
        GameManager.instance.ChangeState(GameState.Playing); // 레벨업 완료
        MasterAudio.PlaySound("Select");
        uiManager.BlackWindowFadeOut(); // 검은 배경 Off
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            btn.interactable = false;
        }        
        gameObject.SetActive(false);

        // 원래 BGM 볼륨으로 복구
        //MasterAudio.PlaylistMasterVolume = PlayerPrefs.GetFloat("BGM");
    }

    public void FirstGiveWeapon(int index)
    {
        GetComponentsInChildren<ItemSetting>(true)[index].OnClick();
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
            if (item.itemData is BulletData)
            {
                // 이미 획득한 무기이거나, 새로운 무기를 획득할 수 있는 경우
                if (item.level > 0 || GameManager.instance.weaponCount < GameManager.instance.maxItemCount)
                {
                    if (item.level < item.itemData.maxLevel)
                    {
                        availableItems.Add(item);
                        allMaxLevel = false;
                    }
                }
            }
            else if (item.itemData is GearData)
            {
                // 이미 획득한 기어이거나, 새로운 기어를 획득할 수 있는 경우
                if (item.level > 0 || GameManager.instance.gearCount < GameManager.instance.maxItemCount)
                {
                    if (item.level < item.itemData.maxLevel)
                    {
                        availableItems.Add(item);
                        allMaxLevel = false;
                    }
                }
            }        
        }

        // 모든 무기와 기어가 최대 레벨에 도달했다면 Etc 아이템만 활성화
        if (allMaxLevel)
        {
            foreach (ItemSetting item in items)
            {
                if (item.itemData is EtcData)
                {
                    availableItems.Add(item);
                }
            }            
        }

        // 활성화할 아이템 수를 결정 (최대 4개)
        int itemsToActivate = Mathf.Min(4, availableItems.Count);

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
