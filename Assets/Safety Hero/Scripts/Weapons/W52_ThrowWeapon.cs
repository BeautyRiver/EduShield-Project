using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W52_ThrowWeapon : Weapon
{   
    protected override void Attack()
    {
        StartCoroutine(R52_Bullet());
    }

    // 창
    private IEnumerator R52_Bullet()
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 dir = new Vector3(player.lastInputVec.x, player.lastInputVec.y, 0).normalized;
            Transform bullet = GameManager.instance.poolManager.Get(PoolType.Bullet, 5).transform; // Bullet52 가져오기
            bullet.parent = transform;
            Vector3 spreadOffset = Vector3.zero;

            float random = Random.Range(-4, 5) * 0.05f;
            // 발사 방향에 따라 발사체 간격을 조절 (오른쪽/왼쪽, 위쪽/아래쪽 모두 지원)
            spreadOffset = Vector3.Cross(dir, Vector3.forward) * ((i - (count / 2)) * random);


            // 발사체의 시작 위치를 조정
            Vector3 startPosition = transform.position + spreadOffset;

            bullet.localScale = bulletSize;
            bullet.position = startPosition;
            //bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);  // 발사 방향에 맞게 회전 설정
            bullet.rotation = Quaternion.Euler(new Vector3(bullet.transform.eulerAngles.x, bullet.transform.eulerAngles.y, Random.Range(0, 360f)));

            //불렛 초기화
            BulletInit(bullet, dir);

            // 발사 사운드
            MasterAudio.PlaySound("R52_Throw");

            // 발사 후 약간의 딜레이 추가
            yield return new WaitForSeconds(bulletDelay);  // 총알 사이의 딜레이 설정 (0.1초)
        }
        isAttacking = false;
    }
}
