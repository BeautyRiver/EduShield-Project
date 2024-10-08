using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

#if UNITY_EDITOR
[ExecuteInEditMode]

public class ForDebug : MonoBehaviour
{
    public GameObject itemParent;
    public Sprite[] uiImages;
    public ItemSetting[] items;
    public ItemData[] itemData;

    public TextMeshProUGUI debugText;
    private bool isInvinsible;
    private bool is2xSpeed;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!isInvinsible)
            {
                isInvinsible = true;
                GameManager.instance.health = 99999999999;
                debugText.text = "무적";
            }
            else
            {
                isInvinsible = false;
                GameManager.instance.health = 100;
                debugText.text = "무적 해제";
            }
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GameManager.instance.player.speed += 1f;
            debugText.text = "속도 증가";
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            GameManager.instance.player.speed -= 1f;
            debugText.text = "속도 감소";
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            //is2xSpeed = true;
            GameManager.instance.nowTimeScale += 0.5f;
            Time.timeScale = GameManager.instance.nowTimeScale;
            debugText.text = $"+0.5배속 / 현재 TimeScale: {GameManager.instance.nowTimeScale}";
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            GameManager.instance.nowTimeScale -= 0.5f;
            Time.timeScale = GameManager.instance.nowTimeScale;
            debugText.text = $"-0.5배속 / 현재 TimeScale: {GameManager.instance.nowTimeScale}";
        }


        // 디버깅용 레벨업
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.instance.GetExp(GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)]); // 최대 인덱스를 초과하지 않게
        }
#endif
    }

    public void InitializeItems()
    {
        items = itemParent.GetComponentsInChildren<ItemSetting>();
        int idx = 0;
        foreach (ItemSetting item in items)
        {
            item.data = itemData[idx];
            item.gameObject.name = itemData[idx].name;

            switch (item.data.itemCategory)
            {
                case ItemData.ItemCategory.Weapon:
                    item.GetComponent<Image>().sprite = uiImages[0];
                    break;
                case ItemData.ItemCategory.Gear:
                    item.GetComponent<Image>().sprite = uiImages[1];
                    break;
                case ItemData.ItemCategory.Etc:
                    item.GetComponent<Image>().sprite = uiImages[2];
                    break;
                default:
                    break;
            }
            idx++;
        }
    }
}
#endif