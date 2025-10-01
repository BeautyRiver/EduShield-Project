using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W51_CannonWeapon : Weapon
{

    public override void Attack()
    {
        StartCoroutine(R51_Bullet());
    }

    // 총
    private IEnumerator R51_Bullet()
    {
        bool isReverse = false;
        for (int i = 0; i < finalStats.count; i++)
        {
            // isReverse 플래그에 따라 발사 방향 결정 (true면 정방향, false면 반대 방향)
            Vector3 dir = isReverse ? playerMove.lastInputVec.normalized : -playerMove.lastInputVec.normalized;

            Transform bullet = poolManager.Get(currentData.bulletPrefab).transform;
            bullet.parent = transform;
            Vector3 spreadOffset = Vector3.zero;

            float random = Random.Range(-4, 5) * 0.2f;
            // 발사 방향에 따라 발사체 간격을 조절 (오른쪽/왼쪽, 위쪽/아래쪽 모두 지원)
            spreadOffset = Vector3.Cross(dir, Vector3.forward) * ((i - (finalStats.count / 2)) * random);

            // 발사체의 시작 위치를 조정
            Vector3 startPosition = transform.position + spreadOffset;

            bullet.localScale = finalStats.bulletSize;
            bullet.position = startPosition;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);  // 발사 방향에 맞게 회전 설정

            // 불렛 초기화
            BulletInit(bullet, dir);

            // 발사 사운드
            MasterAudio.PlaySound("R51_Cannon");

            // 발사 후 약간의 딜레이 추가
            yield return new WaitForSeconds(finalStats.bulletDelay);  // 총알 사이의 딜레이 설정 (0.1초)        

            isReverse = !isReverse; // 매번 방향을 반대로 변경
        }

        isAttacking = false;
    }
}
