using System.Collections;
using DarkTonic.MasterAudio;
using UnityEditor.EditorTools;
using UnityEngine;
using static Weapon;

public class Shooter : MonoBehaviour
{
    private TargetScanner targetScanner;
    private PoolManager poolManager;

    private void Awake()
    {
        targetScanner = GetComponent<TargetScanner>();
    }

    public void Fire(GameObject bulletPrefab, WeaponStats stats, string playSoundName)
    {
        // 사용할 프리팹이나 스탯 정보가 없으면 발사X
        if (bulletPrefab == null || stats == null)
        {
            Debug.LogError("Bullet Prefab or WeaponStats is not provided to the Shooter component.", this);
            return;
        }
        StartCoroutine(ShootRoutine(bulletPrefab, stats, playSoundName));
    }

    private IEnumerator ShootRoutine(GameObject bulletPrefab, WeaponStats stats, string playSoundName)
    {
        // stats에 정의된 발사 횟수만큼 반복
        for (int i = 0; i < stats.count; i++)
        {
            // 타겟이 없으면 발사를 중단
            if (targetScanner == null || targetScanner.nearestTarget == null)
            {
                yield break;
            }

            // 가장 가까운 타겟의 위치를 향하는 방향을 계산
            Vector3 targetPos = targetScanner.nearestTarget.position;
            Vector3 dir = (targetPos - transform.position).normalized;

            // 풀 매니저에서 총알을 가져옴
            Transform bullet = poolManager.Get(bulletPrefab).transform;
            bullet.position = transform.position;
            bullet.localScale = stats.bulletSize;

            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);

            // 총알을 초기화
            bullet.GetComponent<Bullet>()?.Init(
                dir,
                this.gameObject.tag, // Shooter가 붙어있는 오브젝트의 태그
                stats.per,
                stats.damage,
                stats.bulletMoveSpeed,
                stats.knockBackAmout,
                stats.damageInterval
            );

            MasterAudio.PlaySound(playSoundName);

            // stats에 정의된 총알 사이의 딜레이만큼 대기
            if (stats.bulletDelay > 0)
            {
                yield return new WaitForSeconds(stats.bulletDelay);
            }
        }
    }
}
