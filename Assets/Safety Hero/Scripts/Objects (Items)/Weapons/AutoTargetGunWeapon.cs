using System.Collections;
using UnityEngine;

public class W50_TargetGunWeapon : ProjectileWeapon
{
    public override void Attack()
    {
        StartCoroutine(R50_Bullet());
    }

    private IEnumerator R50_Bullet()
    {
        int realCount = (int)finalStats.count;

        for (int i = 0; i < realCount; i++)
        {
            if (targetScanner.nearestTarget == null)
                break;

            Vector3 targetPos = targetScanner.nearestTarget.position;
            Vector3 dir = (targetPos - transform.position).normalized;

            SpawnProjectile(transform.position, Quaternion.FromToRotation(Vector3.up, dir), dir, "R50_TargetGun");

            yield return new WaitForSeconds(finalStats.bulletDelay);
        }

        isAttacking = false;
    }
}
