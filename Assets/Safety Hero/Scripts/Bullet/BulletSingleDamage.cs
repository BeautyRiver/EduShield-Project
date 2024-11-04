using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSingleDamage : Bullet
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger에 들어는 왔음");
        if (collision is IDamageable damageable)
        {
            Debug.Log($"Damaged to {collision.name}, Damage: {Damage}");
            damageable.DamagedLogic(bulletCol, Damage);
            PerDown();
        }
    }
    /*private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || Per == -100)
            return;

        gameObject.SetActive(false);
    }*/
}
