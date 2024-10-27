using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ItemData.cs
public abstract class ItemData : ScriptableObject
{
    public enum ItemCategory { Bullet, Gear, Etc }
    public enum ItemType { }
    public ItemCategory Category;
    public ItemType Type;
    [Header("# 근접: 0 ~ 49 / 원거리: 50 ~ 99\n" +
        "# 기어: 100 ~ 199 / 기타: 200 ~ 299")]

    [Header("# 속성")]
    public int itemId;
    public string itemName;
    public Sprite itemIcon;
    public int maxLevel;

    [TextArea]
    public string[] itemDesc;

    // 추상 메서드 선언
    protected abstract void OnValidate();
    public abstract void OnEnableSetting(ItemSetting itemSetting);
    public abstract void OnClickSetting(ItemSetting itemSetting);

}
