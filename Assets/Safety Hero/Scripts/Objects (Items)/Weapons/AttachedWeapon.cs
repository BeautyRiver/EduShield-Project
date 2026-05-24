using UnityEngine;

// 부착형 무기 베이스: 자식 오브젝트로 effect/bullet을 유지하며 weapon과 함께 움직인다.
// 발사형과 달리 풀에서 한 번 가져오면 재사용한다 (Batch 패턴).
// 회전 무기, 자기장, 오라, 펫 등이 여기 해당.
public abstract class AttachedWeapon : Weapon, IBatchable
{
    // 자식 개수를 보장한다. 부족하면 풀에서 가져와 자식으로 부착, 충분하면 기존 자식 그대로.
    // 위치/회전/스케일 설정은 호출자가 책임진다.
    protected Transform EnsureChild(int index)
    {
        if (index < transform.childCount)
            return transform.GetChild(index);

        Transform child = poolManager.Get(currentBulletData.bulletPrefab).transform;
        child.parent = transform;
        return child;
    }

    // count(또는 그 외 기준)이 바뀌었을 때 자식들을 재배치/재구성하는 책임.
    // 기어(예: G3_Range)에서 스탯 변경 후 호출됨.
    public abstract void Batch();
}
