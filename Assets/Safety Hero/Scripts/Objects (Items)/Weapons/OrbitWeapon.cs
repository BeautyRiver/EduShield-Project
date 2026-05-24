using DG.Tweening;
using System.Collections;
using UnityEngine;

public class W1_RotateWeapon : AttachedWeapon, IRotatingable
{
    [Header("# 회전 무기 전용 스탯")]
    [Tooltip("초당 회전 각도")]
    [SerializeField] private float baseRotationSpeed = 180f;
    [Tooltip("0이면 영구 유지, 0보다 크면 일정 시간 후 사라짐 연출")]
    [SerializeField] private float baseWeaponDuration = 0f;
    [Tooltip("플레이어 중심으로부터 궤도 반경")]
    [SerializeField] private float baseOrbitRadius = 2f;

    // 런타임 계산 값
    private float currentRotationSpeed;
    private float currentWeaponDuration;
    private float currentOrbitRadius;

    public override void RecalculateStats()
    {
        base.RecalculateStats();

        PlayerData pData = gm.playerData;

        // 회전속도는 공속과 같이 빨라짐
        currentRotationSpeed = baseRotationSpeed * pData.attackSpeedMult;
        // 지속시간은 공속과 같이 길어짐 (기존 동작 유지)
        currentWeaponDuration = baseWeaponDuration * pData.attackSpeedMult;
        // 궤도 반경은 사거리 배율 적용
        currentOrbitRadius = baseOrbitRadius * pData.attackRangeMult;
    }

    protected override void Update()
    {
        base.Update();

        // 매 프레임 회전
        transform.Rotate(Vector3.back * currentRotationSpeed * Time.deltaTime);
    }

    public override void Attack()
    {
        StartCoroutine(M1_Bullet());
    }

    protected IEnumerator M1_Bullet()
    {
        Batch();

        // duration이 0이면 영구 유지, 0보다 크면 일정 시간 후 축소(사라짐 연출)
        if (currentWeaponDuration > 0f)
        {
            yield return new WaitForSeconds(currentWeaponDuration);

            for (int index = 0; index < finalStats.count; index++)
            {
                Transform bullet = transform.GetChild(index);
                bullet.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBounce);
            }
            isAttacking = false;
        }
    }

    public override void Batch()
    {
        int realCount = (int)finalStats.count;
        for (int index = 0; index < realCount; index++)
        {
            Transform bullet = EnsureChild(index);

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            // 원형 배치: 360도를 count로 나눠 각도 부여
            Vector3 rotVec = Vector3.forward * 360 * index / finalStats.count;
            bullet.Rotate(rotVec);
            bullet.localScale = Vector3.zero;
            bullet.Translate(bullet.up * currentOrbitRadius, Space.World);
            bullet.DOScale(finalStats.bulletSize, 0.5f).SetEase(Ease.OutBounce);

            BulletInit(bullet);
        }
    }

    public void Rotate()
    {
        transform.Rotate(Vector3.back * finalStats.bulletDelay * Time.deltaTime);
    }
}
