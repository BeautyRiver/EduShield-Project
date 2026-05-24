using UnityEngine;

// 데미지 받을 수 있는지
public interface IDamageable
{
    void DamagedLogic(float damage, Collider2D collision = null, bool isCrit = false);
}
