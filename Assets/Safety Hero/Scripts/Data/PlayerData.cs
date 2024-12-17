using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chracter Data", menuName = "Scriptble Object/Character Data")]
[System.Serializable]
public class PlayerData : ScriptableObject
{
    [Header("캐릭터 아이디")]
    public int characterId = 0; // 선택된 캐릭터

    [Header("최대 체력 비율")]
    public float maxHpMult = 1f; // 최대 체력 배율

    [Header("데미지 배율")]
    public float damageMult = 1f; // 데미지 배율

    [Header("공격속도 배율")]
    public float attackSpeedMult = 1f; // 공격속도 배율

    [Header("공격범위 배율")]
    public float attackRangeMult= 1f; // 공격범위 배율

    [Header("이동속도 배율")]
    public float speedMult = 1f; // 이동속도 배율
    //public int gold = 0; // 골드량
}
