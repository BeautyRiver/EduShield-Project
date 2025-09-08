using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W0_DefaultWeapon : Weapon
{
    [SerializeField] private float xRange;
    public override void Attack()
    {
        StartCoroutine(M0_Bullet());
    }
    protected IEnumerator M0_Bullet()
    {
        float xDir = Mathf.Sign(playerMove.lastInputX);
        
        // 첫 번째 공격은 플레이어가 바라보는 방향, 두 번째는 반대 방향으로 발사
        for (int i = 0; i < count; i++)
        {
            // 첫 번째 발사 방향: 플레이어가 바라보는 방향
            float currentFireDirX = (i % 2 == 0) ? (xDir) : -(xDir);
            Vector3 dir = new Vector3(currentFireDirX, 0, 0);
            // 새로운 발사체 생성
            Transform bullet = GameManager.instance.poolManager.Get(PoolType.Bullet, prefabId).transform; //Bullet0 가져오기
            bullet.parent = transform;

            // 발사체 위치 설정 (약간의 높이 차이 추가)
            bullet.localScale = bulletSize;
            bullet.position = transform.position + new Vector3(0, i * 1f, 0); // 무기 개수에 따라 높이 증가
            bullet.Translate(dir * xRange); // 지정된 거리만큼 이동

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
