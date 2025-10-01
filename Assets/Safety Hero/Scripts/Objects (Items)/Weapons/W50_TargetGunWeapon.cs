using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W50_TargetGunWeapon : Weapon
{
    public override void Attack()
    {
        StartCoroutine(R50_Bullet());
    }

    // 총
    private IEnumerator R50_Bullet()
    {
        for (int i = 0; i < finalStats.count; i++)
        {
            if (targetScanner.nearestTarget == null)
                break;

            Vector3 targetPos = targetScanner.nearestTarget.position;
            Vector3 dir = (targetPos - transform.position).normalized;

            // 총알 발사
            Transform bullet = poolManager.Get(currentData.bulletPrefab).transform;
            bullet.parent = transform;

            bullet.localScale = finalStats.bulletSize;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);

            // 불렛 초기화
            BulletInit(bullet, dir);

            // 발사 사운드
            MasterAudio.PlaySound("R50_TargetGun");

            // 발사 후 약간의 딜레이 추가
            yield return new WaitForSeconds(finalStats.bulletDelay); // 총알 사이의 딜레이 설정 (0.1초, 필요에 따라 조정 가능)
        }
        isAttacking = false;
    }
}
