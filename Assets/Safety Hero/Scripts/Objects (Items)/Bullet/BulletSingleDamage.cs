using UnityEngine;

public class BulletSingleDamage : Bullet
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 자기 총알에 안맞게
        if (collision.CompareTag(this.ownerTag))
            return;

        if (collision.TryGetComponent(out IDamageable damageAble))
        {
            float finalDamage = RollCritDamage(out bool isCrit);
            damageAble.DamagedLogic(finalDamage, bulletCol, isCrit);
            PerDown();
        }
    }
}
