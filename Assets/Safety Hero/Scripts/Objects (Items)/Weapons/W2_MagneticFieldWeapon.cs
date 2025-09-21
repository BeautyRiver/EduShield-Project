using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W2_MagneticFieldWeapon : Weapon, IBatchable
{
    public override void Attack()
    {
        StartCoroutine(M2_Bullet());
    }

    // 자기장
    protected IEnumerator M2_Bullet()
    {
        while (gameManager.isGameActive)
        {
            yield return null;
            Transform bullet;
            if (transform.childCount <= 0)
            {
                bullet = poolManager.Get(bulletPrefab).transform; // Bullet2 가져오기
                bullet.parent = transform; // 부모 설정
                bullet.localPosition = Vector3.zero; // 로컬 위치 초기화
                bullet.localRotation = Quaternion.identity; // 로컬 회전 초기화
                bullet.localScale = Vector3.zero; // 로컬 크기 0으로 초기화
                Batch();
            }
            else
            {
                bullet = transform.GetChild(0);
            }
            BulletInit(bullet);
        }
    }

    public void Batch()
    {
        Transform bullet = transform.GetChild(0);
        bullet.DOScale(bulletSize, 0.5f).SetEase(Ease.OutBack);
    }

}
