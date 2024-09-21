using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
[ExecuteInEditMode]

public class ForDebug : MonoBehaviour
{
    public GameObject itemParent;
    public Sprite[] uiImages;
    public Item[] items;
    public ItemData[] itemData;

    public TextMeshProUGUI debugText;
    private bool isInvinsible;
    private bool is2xSpeed;

    private void Start()
    {
        items = itemParent.GetComponentsInChildren<Item>();
        int idx = 0;
        foreach (Item item in items)
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!isInvinsible)
            {
                isInvinsible = true;
                GameManager.instance.health = 99999999999;
                debugText.text = "公利";
            }
            else
            {
                isInvinsible = false;
                GameManager.instance.health = 100;
                debugText.text = "公利 秦力";
            }
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GameManager.instance.player.speed += 1f;
            debugText.text = "加档 刘啊";
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            GameManager.instance.player.speed -= 1f;
            debugText.text = "加档 皑家";
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (!is2xSpeed)
            {
                is2xSpeed = true;
                Time.timeScale = 5f;
                debugText.text = "5硅加";
            }
            else
            {
                is2xSpeed = false;
                Time.timeScale = 1f;
                debugText.text = "5硅加 秦力";
            }
        }
    }
}
#endif