using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSingleDamage : Bullet
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(OwnerTag))
            return;

        if (collision.TryGetComponent(out IDamageable damageAble))
        {
            Debug.Log("BulletSingleDamage: OnTriggerEnter2D: DamagedLogic");
            damageAble.DamagedLogic(bulletCol, Damage);
            PerDown();
        }
    }    
}
