using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VInspector;

// DataGuide.cs
public abstract class Data : ScriptableObject
{
    [Header("# 근접: 0 ~ 49 / 원거리: 50 ~ 99\n" +
        "# 기어: 100 ~ 199 / 기타: 200 ~ 299")]

    [Tab("* 기본속성")]
    public int itemId;
    public string itemName;
    public Sprite itemIcon;
    //public int maxLevel;

    [TextArea]
    public string defalutDesc;

    // 추상 메서드 선언
    public abstract void InitializeItemSetting(LevelUpItemSetting itemSetting);
    public abstract void OnEnableSetting(LevelUpItemSetting itemSetting);
    public abstract void OnClickSetting(LevelUpItemSetting itemSetting);

}
