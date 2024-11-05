using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSingleDamage : Bullet
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageAble = collision.GetComponent<IDamageable>();
        if (damageAble != null)
        {
            damageAble.DamagedLogic(bulletCol, Damage);
            PerDown();
        }
    }    
}
