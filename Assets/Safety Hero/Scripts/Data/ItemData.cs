using System.Collections;
using System.Collections.Generic;
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

    protected abstract void OnValidate();    
    public abstract string GetDescription(int level, int increaseRate);
    public abstract string GetLevelText(int level);
    public abstract bool IsNewIconActive(int level);
}
