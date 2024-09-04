using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chracter Data", menuName = "Scriptble Object/Character Data")]
[System.Serializable]
public class PlayerData : ScriptableObject
{
    [Header("Character Info")]
    public int characterId = 0; // 선택된 캐릭터

    [Header("Health Settings")]
    public float maxHpMult = 1f; // 최대 체력 배율

    [Header("Damage Settings")]
    public float damageMult = 1f; // 데미지 배율

    [Header("Attack Speed Settings")]
    [Header("낮을 수록 빠름")]
    public float atkDelay = 1f; // 공격 딜레이 배율
    [Header("높을 수록 빠름")]
    public float atkSpeedMult = 1f; // 공격속도 배율

    [Header("Movement Settings")]
    public float speedMult = 1f; // 이동속도 배율
    //public int gold = 0; // 골드량
}
