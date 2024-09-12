using System.Collections;
using System.Collections.Generic;
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
}
#endif