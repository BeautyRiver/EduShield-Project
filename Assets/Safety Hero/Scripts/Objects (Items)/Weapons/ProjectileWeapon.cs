using DarkTonic.MasterAudio;
using UnityEngine;

// 발사형 무기 베이스: 풀에서 Bullet을 가져와 발사 후 독립 비행시킨다.
// 발사된 Bullet은 자체 Collider/Rigidbody로 충돌/수명을 처리하고,
// 일정 시간 후 풀로 반환된다.
public abstract class ProjectileWeapon : Weapon
{
    // 발사체 1발 생성 헬퍼.
    // - position: 월드 좌표 스폰 위치
    // - rotation: 월드 좌표 회전
    // - fireDir: Bullet.Init에 전달될 발사 방향 (Vector3.zero면 정지 발사체)
    // - soundKey: MasterAudio 사운드 키 (null이면 재생 안함)
    protected Transform SpawnProjectile(Vector3 position, Quaternion rotation, Vector3 fireDir, string soundKey = null)
    {
        Transform bullet = poolManager.Get(currentBulletData.bulletPrefab).transform;
        bullet.parent = transform;
        bullet.position = position;
        bullet.rotation = rotation;
        bullet.localScale = finalStats.bulletSize;

        BulletInit(bullet, fireDir);

        if (!string.IsNullOrEmpty(soundKey))
            MasterAudio.PlaySound(soundKey);

        return bullet;
    }
}
