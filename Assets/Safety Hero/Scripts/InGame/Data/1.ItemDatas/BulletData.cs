using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

[CreateAssetMenu(fileName = "Bullet", menuName = "Scriptable Objects/BulletData")]
public class BulletData : Data
{  
    [Tab("# 기본 스탯")]
    public float baseDamage;
    [Header("baseDamageInterval: 데미지 간격 x초당 때림")]
    public float baseDamageInterval = 2f;
    public float baseKnockback = 1.5f; 
    public int baseCount;
    public int basePer;
    public float baseDelay;
    public float baseBulletMoveSpeed;
    public float baseWeaponAttackSpeed;
    public float baseWeaponDuration; // 무기 지속시간 
    public float baseRange;
    public float baseRotationSpeed;
    public Vector3 baseScale;

    [Tab("# 레벨별 스탯")]
    [Header("데미지")]
    public int[] damages; // 데미지

    [Header("개수")]
    public int[] counts; // 개수

    [Header("관통력")]
    public int[] pers; // 관통력    

    [Header("크기 [10 = 10%]")]
    public int[] sizes;

    [Tab("# 무기 관련")]
    public GameObject bulletPrefab;
    public GameObject weaponType;

    // 에디터에서 값이 변경될 때 자동으로 호출
    protected override void OnValidate()
    {
        maxLevel = damages.Length + counts.Length + pers.Length + sizes.Length + 1;

        if (bulletPrefab != null)
            bulletPrefab.transform.localScale = baseScale;
    }

    public override void InitializeItemSetting(LevelUpItemSetting itemSetting)
    {
        // 무기와 기어의 데이터 세팅 (길이가 0 이상인 경우만 추가)
        if (damages.Length > 0)
        {
            itemSetting.statusRateList.Add(new S_int { values = damages });
            itemSetting.rateIdx.Add(0);
        }
        if (counts.Length > 0)
        {
            itemSetting.statusRateList.Add(new S_int { values = counts });
            itemSetting.rateIdx.Add(1);
        }
        if (pers.Length > 0)
        {
            itemSetting.statusRateList.Add(new S_int { values = pers });
            itemSetting.rateIdx.Add(2);
        }
        if (sizes.Length > 0)
        {
            itemSetting.statusRateList.Add(new S_int { values = sizes });
            itemSetting.rateIdx.Add(3);
        }

        // 최대 인덱스 구하기
        foreach (var item in itemSetting.statusRateList)
            itemSetting.maxmumInsideIdx = Mathf.Max(itemSetting.maxmumInsideIdx, item.values.Length);
    }

    public override void OnEnableSetting(LevelUpItemSetting itemSetting)
    {

        if (itemSetting.level == 0)
        {
            itemSetting.NewIcon.gameObject.SetActive(true);
            itemSetting.TextDesc.text = "<color=#99FF8A>새로운 무기!</color>\n\n<size=90%>" + itemDesc[0] + "</size>";
            itemSetting.TextLevel.text = "New Weapon!";
        }

        else if (itemSetting.level < maxLevel)
        {
            itemSetting.TextLevel.text = string.Format($"Lv.{itemSetting.level} → Lv.{itemSetting.level + 1}");

            itemSetting.NewIcon.gameObject.SetActive(false);

            // 비어있는 배열을 건너뛰기 위해 증가
            while (itemSetting.outsideRateIdx < itemSetting.statusRateList.Count &&
                   itemSetting.statusRateList[itemSetting.outsideRateIdx].values.Length == 0)
            {
                itemSetting.outsideRateIdx++;
            }

            if (itemSetting.outsideRateIdx < itemSetting.statusRateList.Count)
            {
                // 인덱스 범위 체크: insideRateIdx가 현재 배열 길이보다 크면 초기화
                if (itemSetting.insideRateIdx >= itemSetting.statusRateList[itemSetting.outsideRateIdx].values.Length)
                {
                    itemSetting.insideRateIdx = 0;
                }

                itemSetting.increaseRate = itemSetting.statusRateList[itemSetting.outsideRateIdx].values[itemSetting.insideRateIdx];

                itemSetting.TextDesc.text = string.Format(itemDesc[itemSetting.outsideRateIdx + 1], itemSetting.increaseRate);

            }
        }
    }

    public override void OnClickSetting(LevelUpItemSetting itemSetting)
    {
        if (itemSetting.level == 0)
        {
            // 새로운 무기 객체를 생성            
            var weapon = Instantiate(weaponType);
            itemSetting.weapon = weapon.GetComponent<Weapon>();
            itemSetting.weapon.transform.parent = GameManager.instance.player.transform;
            itemSetting.weapon.Init(this);
            GameManager.instance.weaponCount++;   
        }
        else
        {
            // 기존 무기의 레벨을 올림
            itemSetting.weapon.WeaponLevelUp(itemSetting.increaseRate, itemSetting.rateIdx[itemSetting.outsideRateIdx], itemSetting.level);

            itemSetting.outsideRateIdx++; // 다음 적용할 인덱스를 증가시킴

            // 인덱스 값이 설정 범위를 넘어가는 경우 계속 조정해주는 로직
            while (itemSetting.outsideRateIdx >= itemSetting.statusRateList.Count ||
                   itemSetting.insideRateIdx >= itemSetting.statusRateList[itemSetting.outsideRateIdx].values.Length)
            {
                if (itemSetting.outsideRateIdx >= itemSetting.statusRateList.Count)
                {
                    itemSetting.outsideRateIdx = 0;
                    itemSetting.insideRateIdx++;
                }

                if (itemSetting.insideRateIdx >= itemSetting.maxmumInsideIdx)
                    break;

                if (itemSetting.insideRateIdx >= itemSetting.statusRateList[itemSetting.outsideRateIdx].values.Length)
                    itemSetting.outsideRateIdx++;
            }
        }

        itemSetting.level++;
        itemSetting._currentLevel = itemSetting.level;
    }

}

