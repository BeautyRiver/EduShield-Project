using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]

public class ForDebug : MonoBehaviour
{
    public GameObject itemParent;
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
            idx++;
        }

    }
}
#endif