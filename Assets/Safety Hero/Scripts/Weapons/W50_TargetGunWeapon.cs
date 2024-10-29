using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W50_TargetGunWeapon : Weapon
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
 
    protected override void Attack()
    {
        StartCoroutine(R50_Bullet());

    }

    protected override void LevelUpException(int rateIndex)
    {
        
    }

    // 총
    private IEnumerator R50_Bullet()
    {
        for (int i = 0; i < count; i++)
        {
            if (player.scanner.nearestTarget == null)
                yield break;

            Vector3 targetPos = player.scanner.nearestTarget.position;
            Vector3 dir = (targetPos - transform.position).normalized;

            // 총알 발사
            Transform bullet = GameManager.instance.pool.Get(PoolManager.PoolType.Weapon, prefabId).transform;
            bullet.parent = transform;

            bullet.localScale = bulletSize;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);

            // 불렛 초기화
            BulletInit(bullet, dir);

            // 발사 사운드
            MasterAudio.PlaySound("R50_TargetGun");

            // 발사 후 약간의 딜레이 추가
            yield return new WaitForSeconds(bulletDelay); // 총알 사이의 딜레이 설정 (0.1초, 필요에 따라 조정 가능)
        }
        isAttacking = false;
    }
}
