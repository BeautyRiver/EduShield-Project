using System.Collections;
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
            //Debug.Log("BulletOverTimeDamage: OnTriggerStay2D: DamagedLogic");
            int enemyId = collision.GetInstanceID();

            if (!damageTimers.ContainsKey(enemyId))
            {
                damageTimers[enemyId] = Time.time;
                damageAble.DamagedLogic(damage, bulletCol);
                PerDown();
            }
            else if (Time.time - damageTimers[enemyId] >= damageInterval)
            {
                damageTimers[enemyId] = Time.time;
                damageAble.DamagedLogic(damage, bulletCol);
                PerDown();
            }
        }
    }

    /*private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            int enemyId = collision.GetInstanceID();
            if (damageTimers.ContainsKey(enemyId))
            {
                damageTimers.Remove(enemyId);
            }
        }
    }*/
}
