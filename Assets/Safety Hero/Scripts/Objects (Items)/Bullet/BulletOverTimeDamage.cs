using System.Collections.Generic;
using UnityEngine;

public class BulletOverTimeDamage : Bullet
{
    private Dictionary<int, float> damageTimers = new Dictionary<int, float>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(ownerTag))
            return;

        if (collision.TryGetComponent(out IDamageable damageAble))
        {
            int enemyId = collision.GetInstanceID();

            if (!damageTimers.ContainsKey(enemyId))
            {
                damageTimers[enemyId] = Time.time;
                ApplyDamage(damageAble);
            }
            else if (Time.time - damageTimers[enemyId] >= damageInterval)
            {
                damageTimers[enemyId] = Time.time;
                ApplyDamage(damageAble);
            }
        }
    }

    private void ApplyDamage(IDamageable damageAble)
    {
        float finalDamage = RollCritDamage(out bool isCrit);
        damageAble.DamagedLogic(finalDamage, bulletCol, isCrit);
        PerDown();
    }
}
