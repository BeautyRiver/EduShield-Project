using System.Collections;
using UnityEngine;

public class W51_CannonWeapon : ProjectileWeapon
{
    public override void Attack()
    {
        StartCoroutine(R51_Bullet());
    }

    private IEnumerator R51_Bullet()
    {
        bool isReverse = false;
        int realCount = (int)finalStats.count;

        for (int i = 0; i < realCount; i++)
        {
            // isReverse 플래그에 따라 발사 방향 결정
            Vector3 dir = isReverse ? playerMove.lastInputVec.normalized : -playerMove.lastInputVec.normalized;

            // 발사 방향과 직교한 축으로 분산 오프셋
            float random = Random.Range(-4, 5) * 0.2f;
            Vector3 spreadOffset = Vector3.Cross(dir, Vector3.forward) * ((i - (finalStats.count / 2)) * random);
            Vector3 startPosition = transform.position + spreadOffset;

            SpawnProjectile(startPosition, Quaternion.FromToRotation(Vector3.up, dir), dir, "R51_Cannon");

            yield return new WaitForSeconds(finalStats.bulletDelay);

            isReverse = !isReverse;
        }

        isAttacking = false;
    }
}
