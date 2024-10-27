using DarkTonic.MasterAudio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W1_RotateWeapon : Weapon, IBatchable, IRotatingable
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        Rotate();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void Attack()
    {
        StartCoroutine(M1_Bullet());
    }

    protected override void LevelUpException(int rateIndex)
    {
        switch (rateIndex)
        {
            case 1: // 카운트 증가                
                Batch();
                break;

            case 3: // 크기[범위] 증가                
                Batch();
                break;
        }
    }


    protected IEnumerator M1_Bullet()
    {
        Batch();
        yield return new WaitForSeconds(durationTime);
        Transform bullet;
        for (int index = 0; index < count; index++) // 불릿 수만큼 반복
        {
            bullet = transform.GetChild(index); // 기존 자식 사용
            bullet.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBounce); // 크기를 0.5초 동안 자연스럽게 축소
        }
        isAttacking = false;
    }

    // 불릿 배치 함수 (회전 무기)
    public void Batch()
    {
        for (int index = 0; index < count; index++) // 불릿 수만큼 반복
        {
            Transform bullet;
            if (index < transform.childCount) // 자식 존재 시
            {
                bullet = transform.GetChild(index); // 기존 자식 사용
            }
            else
            {
                bullet = gm.pool.Get(PoolManager.PoolType.Weapon, prefabId).transform;
                bullet.parent = transform; // 부모 설정
            }

            bullet.localPosition = Vector3.zero; // 로컬 위치 초기화
            bullet.localRotation = Quaternion.identity; // 로컬 회전 초기화

            // 초기 회전 설정
            Vector3 rotVec = Vector3.forward * 360 * index / count; // 불릿 회전 벡터 계산
            bullet.Rotate(rotVec); // 불릿 회전                        
            bullet.localScale = Vector3.zero;
            bullet.Translate(bullet.up * attackRange, Space.World); // 지정된 거리만큼 이동               
            bullet.DOScale(bulletSize, 0.5f).SetEase(Ease.OutBounce); // 크기를 0.5초 동안 자연스럽게 확장

            BulletInit(bullet);
        }
    }

    public void Rotate()
    {
        transform.Rotate(Vector3.back * bulletDelay * Time.deltaTime); // 무기 회전
    }
}
