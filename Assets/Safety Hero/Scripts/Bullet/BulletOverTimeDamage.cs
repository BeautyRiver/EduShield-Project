using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletOverTimeDamage : Bullet
{
    private Dictionary<int, float> damageTimers = new Dictionary<int, float>();

    public override void Init(float damage, int per, Vector3 dir, int id, float knockBack, float interval)
    {
        base.Init(damage, per, dir, id, knockBack, interval);
        // 이동하지 않는 경우 속도를 0으로 설정
        rigid.velocity = Vector2.zero;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            int enemyId = collision.GetInstanceID();

            if (!damageTimers.ContainsKey(enemyId))
            {
                damageTimers[enemyId] = Time.time;
                collision.GetComponent<Enemy>().DamgedLogic(bulletCol, Damage);
                PerDown();
            }
            else if (Time.time - damageTimers[enemyId] >= DamageInterval)
            {
                damageTimers[enemyId] = Time.time;
                collision.GetComponent<Enemy>().DamgedLogic(bulletCol, Damage);
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
