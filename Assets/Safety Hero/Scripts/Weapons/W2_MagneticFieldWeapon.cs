using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W2_MagneticFieldWeapon : Weapon, IBatchable
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
        StartCoroutine(M2_Bullet());
    }

    protected override void LevelUpException(int rateIndex)
    {
        switch (rateIndex)
        {   
            case 3: // 크기[범위] 증가                
                Batch();
                break;
        }
    }

    // 자기장
    protected IEnumerator M2_Bullet()
    {
        while (!gm.isGameRealEnd)
        {
            yield return null;
            Transform bullet;
            if (transform.childCount <= 0)
            {
                bullet = gm.pool.Get(PoolManager.PoolType.Weapon, prefabId).transform;
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
