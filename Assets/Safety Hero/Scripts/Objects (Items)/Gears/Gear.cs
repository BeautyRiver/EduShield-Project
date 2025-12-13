using UnityEngine;

public abstract class Gear : MonoBehaviour
{
    public GearData gearData;
    public float rate;
    public int level; // 레벨 표시는 UI에서 하므로, 내부 로직용으로만 관리

    public float accumulatedRate = 0f; // 누적 증가율 (기본값 0에서 시작)

    protected GameManager gm;
    protected PlayerMove playerMove;

    protected virtual void Awake()
    {
        gm = GameManager.instance;
        playerMove = gm.player.GetComponent<PlayerMove>();
    }

    public virtual void Init(GearData newData)
    {
        // 1. 기본 세팅
        gearData = newData;
        transform.parent = playerMove.transform;
        transform.localPosition = Vector3.zero;
    }

    public virtual void GearLevelUp(float newRate)
    {
        // newRate: 10 (10%) 등이 들어옴 -> 0.1로 변환
        rate = newRate * 0.01f;
        accumulatedRate += rate;

        ApplyPlayerData();
    }

    protected virtual void ApplyToAllWeapons()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            ApplyGearToWeapon(weapon);
        }
    }

    protected abstract void ApplyPlayerData();
    protected abstract void ApplyGearToWeapon(Weapon weapon);
}