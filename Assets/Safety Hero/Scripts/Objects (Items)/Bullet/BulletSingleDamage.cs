using System.Collections;
using System.Collections.Generic;
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
            //Debug.Log("BulletSingleDamage: OnTriggerEnter2D: DamagedLogic");
            damageAble.DamagedLogic(damage, bulletCol);
            PerDown();
        }
    }    
}
