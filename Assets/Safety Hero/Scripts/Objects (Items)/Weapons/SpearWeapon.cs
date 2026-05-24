using System.Collections;
using UnityEngine;

public class W52_ThrowWeapon : ProjectileWeapon
{
    public override void Attack()
    {
        StartCoroutine(R52_Bullet());
    }

    private IEnumerator R52_Bullet()
    {
        int realCount = (int)finalStats.count;

        for (int i = 0; i < realCount; i++)
        {
            Vector3 dir = new Vector3(playerMove.lastInputVec.x, playerMove.lastInputVec.y, 0).normalized;

            float random = Random.Range(-4, 5) * 0.05f;
            Vector3 spreadOffset = Vector3.Cross(dir, Vector3.forward) * ((i - (finalStats.count / 2)) * random);
            Vector3 startPosition = transform.position + spreadOffset;

            // 창은 회전 랜덤 (날아가는 동안 빙글빙글)
            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));

            SpawnProjectile(startPosition, rotation, dir, "R52_Throw");

            yield return new WaitForSeconds(finalStats.bulletDelay);
        }

        isAttacking = false;
    }
}
