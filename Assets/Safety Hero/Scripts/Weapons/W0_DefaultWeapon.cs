using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W0_DefaultWeapon : Weapon
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
        StartCoroutine(M0_Bullet());
    }

    protected override void LevelUpException(int rateIndex)
    {
        // Nothing...
    }


    protected IEnumerator M0_Bullet()
    {
        // 첫 번째 공격은 플레이어가 바라보는 방향, 두 번째는 반대 방향으로 발사
        for (int i = 0; i < count; i++)
        {
            // 첫 번째 발사 방향: 플레이어가 바라보는 방향
            Vector3 dir = (i % 2 == 0) ? new Vector3(player.lastXInputVec, 0, 0).normalized : new Vector3(-player.lastXInputVec, 0, 0).normalized;

            // 새로운 발사체 생성
            Transform bullet = GameManager.instance.pool.Get(PoolManager.PoolType.Weapon, prefabId).transform;
            bullet.parent = transform;

            // 발사체 위치 설정 (약간의 높이 차이 추가)
            bullet.localScale = bulletSize;
            bullet.position = transform.position + new Vector3(0, i * 1f, 0); // 무기 개수에 따라 높이 증가
            bullet.Translate(bullet.right * dir.x * 0.2f); // 지정된 거리만큼 이동


            // 발사 방향에 따라 발사체 회전 설정 (왼쪽으로 발사될 때는 180도 회전)
            if (dir.x < 0)
                bullet.localRotation = Quaternion.Euler(0, 180, 0); 

            else
                bullet.localRotation = Quaternion.identity; 

            // 발사체 초기화
            BulletInit(bullet);
            MasterAudio.PlaySound("M0_Default");

            // 딜레이
            yield return new WaitForSeconds(bulletDelay); 
        }

        // 공격이 끝나면 상태 초기화
        isAttacking = false;
    }
}
