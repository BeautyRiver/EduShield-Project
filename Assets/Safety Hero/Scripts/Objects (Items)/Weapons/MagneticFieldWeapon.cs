using DG.Tweening;
using System.Collections;
using UnityEngine;

public class W2_MagneticFieldWeapon : AttachedWeapon
{
    public override void Attack()
    {
        StartCoroutine(M2_Bullet());
    }

    // 자기장: 플레이어 위치에 고정된 단일 효과 오브젝트 유지
    protected IEnumerator M2_Bullet()
    {
        while (GlobalManager.instance.gameState == GameState.Playing)
        {
            yield return null;

            Transform bullet;
            if (transform.childCount <= 0)
            {
                // 최초 생성: 풀에서 가져와 부착하고 Batch 호출
                bullet = EnsureChild(0);
                bullet.localPosition = Vector3.zero;
                bullet.localRotation = Quaternion.identity;
                bullet.localScale = Vector3.zero;
                Batch();
            }
            else
            {
                bullet = transform.GetChild(0);
            }

            BulletInit(bullet);
        }
    }

    public override void Batch()
    {
        Transform bullet = transform.GetChild(0);
        bullet.DOScale(finalStats.bulletSize, 0.5f).SetEase(Ease.OutBack);
    }
}
