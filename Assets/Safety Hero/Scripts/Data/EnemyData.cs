using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("# 기본 스탯")]
    public int id;
    public int health;
    public float speed;
    public float damage;
    public int exp;
    public RuntimeAnimatorController animCon; // 애니메이터도 데이터에 포함
    public AnimatorOverrideController overrideCon; // Test
    // --- 여기부터 추가 ---
    [Header("# 사용할 프리팹")]
    public GameObject enemyPrefab; 
    public GameObject hitEffectPrefab;
    public GameObject damageTextPrefab;
    public GameObject dropExpPrefab;
    public GameObject dropRewardItemPrefab;
}
