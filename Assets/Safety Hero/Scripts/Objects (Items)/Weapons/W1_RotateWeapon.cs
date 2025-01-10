using DarkTonic.MasterAudio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W1_RotateWeapon : Weapon, IBatchable, IRotatingable
{
    protected override void Update()
    {
        base.Update();

        // 회전 로직 추가
        transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
    }

    public override void Attack()
    {
        StartCoroutine(M1_Bullet());
    }

    protected IEnumerator M1_Bullet()
    {
        Batch();

        // durationTime이 0일 경우 무한 지속, 0보다 클 경우 지정 시간 후 축소
        if (weaponDuration > 0f)
        {
            yield return new WaitForSeconds(weaponDuration);

            Transform bullet;
            for (int index = 0; index < count; index++)
            {
                bullet = transform.GetChild(index);
                bullet.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBounce);
            }
            isAttacking = false;
        }
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
                bullet = gm.poolManager.Get(PoolType.Bullet, prefabId).transform; // Bullet1 가져오기
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
