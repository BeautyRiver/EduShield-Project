using UnityEngine;

// 데미지 받을 수 있는지
public interface IDamageable
{
    void DamagedLogic(Collider2D collision, float damage);
}
