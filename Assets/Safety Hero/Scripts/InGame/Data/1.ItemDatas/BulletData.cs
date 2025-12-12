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
    public float baseWeaponDuration;
    public float baseRange;
    public float baseRotationSpeed;
    public Vector3 baseScale;

    [Tab("# 무한 성장 스탯 (레벨업 당 증가량)")]
    [Header("기본 증가량 (여기에 등급 배율이 곱해짐)")]
    public float damageGrowth;      // 예: 5
    public float countGrowth;       // 예: 0.1 (10번 찍으면 1발 추가)
    public float perGrowth;         // 예: 0
    public float scaleGrowth;       // 예: 0.05 (5%)   

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
        // 1. 랜덤 등급 뽑기 & 배율 가져오기
        itemSetting.rarity = Utils.GetRandomRarity();
        float multiplier = Utils.GetRarityMultiplier(itemSetting.rarity);
        Color color = Utils.GetRarityColor(itemSetting.rarity);

        // 2. 텍스트 & 색상 설정
        itemSetting.textName.color = color; // 이름 색깔 변경 (전설은 금색!)

        if (itemSetting.level == 0)
        {
            itemSetting.textLevel.text = "New!";
            itemSetting.newIcon.gameObject.SetActive(true);

            itemSetting.textDesc.text = defalutDesc;
        }
        else
        {
            itemSetting.textLevel.text = itemSetting.rarity.ToString();
            itemSetting.newIcon.gameObject.SetActive(false);

            // 3. 증가량 계산 및 설명글 작성            
            string desc = "";

            if (damageGrowth > 0)
                desc += $"Damage +{damageGrowth * multiplier:F1}\n"; // 소수점 1자리까지

            if (countGrowth > 0)
                desc += $"Count +{countGrowth * multiplier:F1}\n";

            if (perGrowth > 0)
                desc += $"Per +{perGrowth * multiplier:F1}\n";

            // 설명 텍스트 적용
            itemSetting.textDesc.text = desc;
        }
    }

    public override void OnClickSetting(LevelUpItemSetting itemSetting)
    {
        if (itemSetting.level == 0)
        {
            // 새 무기 얻기 (기존 코드 유지)
            var weaponObj = Instantiate(weaponType, GameManager.instance.player.transform);
            itemSetting.weapon = weaponObj.GetComponent<Weapon>();
            itemSetting.weapon.Init(this);
            GameManager.instance.weaponCount++;
        }
        else
        {
            // [중요] 무기 레벨업 (등급 정보를 함께 넘겨줌)
            // 아직 Weapon에 LevelUp 함수를 안 만들어서 여기서 빨간 줄이 뜰 거예요!
            // 일단 주석 처리해두거나, 다음 단계에서 Weapon.cs를 고치면 해결됩니다.

            // itemSetting.weapon.LevelUp(itemSetting.rarity); 
        }

        itemSetting.level++;

    }

}
