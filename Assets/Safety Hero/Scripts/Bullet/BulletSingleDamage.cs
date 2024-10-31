using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSingleDamage : Bullet
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().DamagedLogic(bulletCol, Damage);
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
