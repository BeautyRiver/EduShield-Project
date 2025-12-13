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
    [SerializeField] private List<LevelUpItemSetting> items;
    public List<LevelUpItemSetting> availableItems;
    private void Awake()
    {     
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<LevelUpItemSetting>(true).ToList();
    }

    public void Show()
    {
        GlobalManager.instance.ChangeGameState(GameState.LevelUp); // 레벨업 중
        gameObject.SetActive(true);
        Next(); // 섞기
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            btn.interactable = true;
        }
        buttons[0].Select();
        MasterAudio.PlaySound("LevelUp");
    }
    public void Hide()
    {
        GlobalManager.instance.ChangeGameState(GameState.Playing); // 레벨업 완료
        MasterAudio.PlaySound("Select");
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            btn.interactable = false;
        }        
        gameObject.SetActive(false);     
    }

    public void FirstGiveWeapon(int index)
    {
        GetComponentsInChildren<LevelUpItemSetting>(true)[index].OnClick();
    }

    private void Next()
    {
        // 1. 모든 아이템 일단 비활성화 (초기화)
        foreach (LevelUpItemSetting item in items)
        {
            item.gameObject.SetActive(false);
        }

        // 2. 등장 가능한 아이템 추리기
        availableItems = new List<LevelUpItemSetting>();

        foreach (LevelUpItemSetting item in items)
        {
            // [무기 처리]
            if (item.data is BulletData)
            {
                // 조건: 이미 가지고 있거나(Level>0) OR 새 무기 슬롯이 남아있으면 OK
                if (item.level > 0 || GameManager.instance.curretWeaponCount < GameManager.instance.maxItemCount)
                {
                    availableItems.Add(item);
                }
            }
            // [기어 처리] - ※ 주의: GearData도 무한 성장으로 리팩토링해야 완벽히 작동합니다!
            else if (item.data is GearData)
            {
                // 조건: 이미 가지고 있거나(Level>0) OR 새 기어 슬롯이 남아있으면 OK
                if (item.level > 0 || GameManager.instance.gearCount < GameManager.instance.maxItemCount)
                {
                    availableItems.Add(item);
                }
            }
            // [회복/기타 아이템]
            else if (item.data is EtcData)
            {
                availableItems.Add(item);
            }
        }

        // 3. 랜덤으로 3개(혹은 4개) 뽑기
        int itemsToActivate = Mathf.Min(3, availableItems.Count);
        List<int> selectedItems = new List<int>();

        while (selectedItems.Count < itemsToActivate)
        {
            int randIndex = Random.Range(0, availableItems.Count);
            if (!selectedItems.Contains(randIndex))
            {
                selectedItems.Add(randIndex);
            }
        }

        // 4. 선택된 아이템 화면에 켜주기
        foreach (int index in selectedItems)
        {
            availableItems[index].gameObject.SetActive(true);
        }
    }

}
