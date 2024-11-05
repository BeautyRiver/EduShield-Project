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
            Debug.Log($"Damaged to {collision.name}, Damage: {Damage}");
            damageAble.DamagedLogic(bulletCol, Damage);
            PerDown();
        }
    }    
}
