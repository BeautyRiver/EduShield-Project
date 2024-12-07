using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletOverTimeDamage : Bullet
{
    private Dictionary<int, float> damageTimers = new Dictionary<int, float>();

    public override void Init(float damage, int per, Vector3 dir, int id, float knockBack, float interval)
    {
        base.Init(damage, per, dir, id, knockBack, interval);
        
        if (rigid != null)
            rigid.linearVelocity = Vector2.zero;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var damageAble = collision.GetComponent<IDamageable>();
        if (damageAble != null)
        {
            int enemyId = collision.GetInstanceID();

            if (!damageTimers.ContainsKey(enemyId))
            {
                damageTimers[enemyId] = Time.time;
                damageAble.DamagedLogic(bulletCol, Damage);
                PerDown();
            }
            else if (Time.time - damageTimers[enemyId] >= DamageInterval)
            {
                damageTimers[enemyId] = Time.time;
                damageAble.DamagedLogic(bulletCol, Damage);
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
