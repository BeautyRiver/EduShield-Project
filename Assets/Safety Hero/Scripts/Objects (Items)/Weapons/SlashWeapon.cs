using System.Collections;
using UnityEngine;

public class W0_DefaultWeapon : ProjectileWeapon
{
    [SerializeField] private float xRange;

    public override void Attack()
    {
        StartCoroutine(M0_Bullet());
    }

    protected IEnumerator M0_Bullet()
    {
        float xDir = Mathf.Sign(playerMove.lastInputX);
        int realCount = (int)finalStats.count;

        for (int i = 0; i < realCount; i++)
        {
            // 짝/홀 번갈아 좌우로 발사
            float currentFireDirX = (i % 2 == 0) ? (xDir) : -(xDir);
            Vector3 dir = new Vector3(currentFireDirX, 0, 0);

            Vector3 spawnPosition = transform.position + (dir * xRange) + new Vector3(0, i * 1f, 0);
            Quaternion rotation = dir.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

            SpawnProjectile(spawnPosition, rotation, Vector3.zero, "M0_Default");

            yield return new WaitForSeconds(finalStats.bulletDelay);
        }

        isAttacking = false;
    }
}
