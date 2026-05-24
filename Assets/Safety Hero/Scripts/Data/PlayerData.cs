using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chracter Data", menuName = "Scriptable Objects/Player Data")]
[System.Serializable]
public class PlayerData : ScriptableObject
{
    [Header("# 캐릭터 아이디")]
    public int characterId = 0; // 선택된 캐릭터

    [Header("# 최대 체력 비율")]
    public float maxHpMult = 1f; // 최대 체력 배율

    [Header("# 데미지 배율")]
    public float damageMult = 1f; // 데미지 배율

    [Header("# 공격속도 배율")]
    public float attackSpeedMult = 1f; // 공격속도 배율

    [Header("# 공격범위 배율")]
    public float attackRangeMult= 1f; // 공격범위 배율 (적 탐지/궤도 반경)

    [Header("# 크기 배율")]
    public float sizeMult = 1f; // 발사체/오라 크기 배율

    [Header("# 이동속도 배율")]
    public float speedMult = 1f; // 이동속도 배율

    [Header("# 치명타")]
    [Range(0f, 1f)]
    public float critChance = 0f;        // 치명타 확률 (0~1)
    public float critDamageMult = 2f;    // 치명타 데미지 배율

    [Header("# 캐릭터 애니메이션")]
    public RuntimeAnimatorController animCon;
    //public int gold = 0; // 골드량
}
