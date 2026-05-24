using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

[CreateAssetMenu(fileName = "Bullet", menuName = "Scriptable Objects/BulletData")]
public class BulletData : Data
{
    [Tab("# 기본 스탯 (초기값)")]
    public float baseDamage;
    public float baseDamageInterval = 2f;
    public float baseKnockback = 1.5f;
    public float baseCount;
    public float basePer;
    public float baseDelay; // TODO: 나중에 딜레이는 곱연산으로 처리하기
    public float baseBulletMoveSpeed;
    public float baseWeaponAttackSpeed;
    public Vector3 baseScale;

    [Tab("# 무한 성장 스탯 (레벨업 당 증가량)")]
    [Header("기본 증가량 (여기에 등급 배율이 곱해짐)")]
    public float damageGrowth;      // 데미지 증가량
    public float countGrowth;       // 탄환 수 증가량
    public float perGrowth;         // 관통력 증가량
    public float scaleGrowth;       // 탄환 크기 증가량

    [Tab("# 무기 관련")]
    public GameObject bulletPrefab;
    public GameObject weaponType;

    // 에디터에서 값이 변경될 때 자동으로 호출
    protected void OnValidate()
    {
        //maxLevel = damages.Length + counts.Length + pers.Length + sizes.Length + 1;

        if (bulletPrefab != null)
            bulletPrefab.transform.localScale = baseScale;
    }

    public override void InitializeItemSetting(LevelUpItemSetting itemSetting)
    {

    }

    public override void OnEnableSetting(LevelUpItemSetting itemSetting)
    {
        // 1. 공통: 랜덤 등급 & 색상
        itemSetting.rarity = Utils.GetRandomRarity();
        float multiplier = Utils.GetRarityMultiplier(itemSetting.rarity);
        Color color = Utils.GetRarityColor(itemSetting.rarity);

        itemSetting.icon.sprite = itemSetting.data.itemIcon;
        itemSetting.textName.text = itemSetting.data.itemName;
        

        itemSetting.selectedOptions.Clear(); // 초기화

        string desc = "";

        // Case 1: 새로운 무기 (Level 0) -> "0에서 시작!"
       if (itemSetting.level == 0)
        {
            itemSetting.textRairty.text = "New Weapon";
            itemSetting.textLevel.text = "New!";
            desc += defalutDesc;
            //if (baseDamage > 0) desc += $"Dmg: 0 <color=#00FF00>→ {baseDamage}</color>\n";
            //if (baseCount > 0) desc += $"Count: 0 <color=#00FF00>→ {baseCount}</color>\n";
            //if (basePer > 0) desc += $"Per: 0 <color=#00FF00>→ {basePer}</color>\n";
        }

        // Case 2: 강화 (Level > 0) -> "현재에서 더하기!"
        else
        {
            itemSetting.textRairty.color = color;
            itemSetting.textRairty.text = itemSetting.rarity.ToString();
            itemSetting.textLevel.text = $"Lv. {itemSetting.level}";
            Weapon myWeapon = itemSetting.weapon;

            List<StatType> candidates = new List<StatType>();
            if (damageGrowth > 0) candidates.Add(StatType.Damage);  
            if (countGrowth > 0) candidates.Add(StatType.Count);
            if (perGrowth > 0) candidates.Add(StatType.Per);
            if (scaleGrowth > 0) candidates.Add(StatType.Size);

            // 2. [랜덤 개수 결정] 1개 ~ 후보 전체 개수 중 랜덤 (최소 1개 보장)
            // candidates가 비어있으면 0이 되므로 Max(1, count) 처리
            int maxCount = candidates.Count;
            int pickCount = (maxCount > 0) ? Random.Range(1, maxCount + 1) : 0;

            // 3. [셔플 & 뽑기] 후보 리스트를 섞어서 앞에서부터 pickCount만큼 가져옴
            for (int i = 0; i < pickCount; i++)
            {
                StatType temp = candidates[i];
                int randomIndex = Random.Range(i, maxCount);
                candidates[i] = candidates[randomIndex];
                candidates[randomIndex] = temp;
            }

            // 뽑힌 옵션들 itemSetting에 저장
            for (int i = 0; i < pickCount; i++)
            {
                itemSetting.selectedOptions.Add(candidates[i]);
            }

            // 4. [UI 표시] 저장된 옵션만 계산해서 텍스트 생성
            foreach (StatType stat in itemSetting.selectedOptions)
            {
                switch (stat)
                {
                    case StatType.Damage:
                        float curDmg = myWeapon.finalStats.damage;
                        float nxtDmg = curDmg + (damageGrowth * multiplier);
                        desc += $"Dmg: {curDmg:F1} <color=#00FF00>→ {nxtDmg:F1}</color>\n";
                        break;
                    case StatType.Count:
                        float curCnt = myWeapon.finalStats.count;
                        float nxtCnt = curCnt + (countGrowth * multiplier);
                        desc += $"Count: {curCnt:F1} <color=#00FF00>→ {nxtCnt:F1}</color>\n";
                        break;
                    case StatType.Per:
                        float curPer = myWeapon.finalStats.per;
                        float nxtPer = curPer + (perGrowth * multiplier);
                        desc += $"Per: {curPer:F1} <color=#00FF00>→ {nxtPer:F1}</color>\n";
                        break;
                    case StatType.Size:
                        float curSz = myWeapon.finalStats.bulletSize.x * 100f;
                        float nxtSz = curSz + (scaleGrowth * multiplier * 100f);
                        desc += $"Size: {curSz:F0}% <color=#00FF00>→ {nxtSz:F0}%</color>\n";
                        break;
                }
            }
        }

        itemSetting.textDesc.text = desc;
    }

    public override void OnClickSetting(LevelUpItemSetting itemSetting)
    {
        if (itemSetting.level == 0)
        {
            // 새 무기 얻기 (기존 코드 유지)
            var weaponObj = Instantiate(weaponType, GameManager.instance.player.weaponObject.transform);
            itemSetting.weapon = weaponObj.GetComponent<Weapon>();
            itemSetting.weapon.Init(this);
            GameManager.instance.curretWeaponCount++;
        }
        else
        {
            // [중요] 무기 레벨업 (등급 정보를 함께 넘겨줌)
            itemSetting.weapon.LevelUp(itemSetting.rarity, itemSetting.selectedOptions); 
        }

        itemSetting.level++;

    }

}
