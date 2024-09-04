using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("# 무기 세팅")]
    public int id; // 무기의 고유 ID
    public int prefabId; // 생성할 불릿의 프리팹 ID
    public float damage; // 무기 데미지
    public int count; // 무기 개수
    public int per; // 관통력

    [Header("# 근접: 회전 속도 / 원거리: 발사 텀(초당 발사)")]
    public float weaponDealay; // 무기의 딜레이 속도
    private float[] rWeaponTimers = { 0, 0, 0, 0, 0 };
    private Player player;

    private void Awake()
    {
        player = GameManager.instance.player;        
    }
    private void Update()
    {
        if (GameManager.instance.isLive)
        {
            switch (id)
            {
                case 0: // 회전무기
                    transform.Rotate(Vector3.back * weaponDealay * Time.deltaTime);
                    break;

                case 50: // 단발총
                    rWeaponTimers[0] += Time.deltaTime;
                    if (rWeaponTimers[0] > weaponDealay)
                    {
                        rWeaponTimers[0] = 0f;
                        StartCoroutine(AutoFireRangedWeapon());
                    }
                    break;
                case 51: // 대포
                    rWeaponTimers[1] += Time.deltaTime;
                    if (rWeaponTimers[1] > weaponDealay)
                    {
                        rWeaponTimers[1] = 0f;
                        StartCoroutine(AutoFireRangedWeapon());
                    }
                    break;
                case 52: // 창던지기
                    rWeaponTimers[2] += Time.deltaTime;
                    if (rWeaponTimers[2] > weaponDealay)
                    {
                        rWeaponTimers[2] = 0f;
                        StartCoroutine(DirFireRangedWeapon());
                    }
                    break;
            }
        }
    }
    // 초기 설정 함수
    public void Init(ItemData data)
    {
        // 기본 세팅
        gameObject.name = "Equip Weapon: " + data.itemType.ToString();
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero; // 플레이어 안에서 위치 초기화

        // 속성 세팅
        id = data.itemId; // 아이디 설정
        damage = data.baseDamage * GameManager.instance.playerData.damageMult; // 기본 데미지 설정
        count = data.baseCount; // 기본 개수 설정
        per = data.basePer; // 기본 관통력 설정

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.prefab == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
            }
        }

        switch (id)
        {
            // 근접 무기
            case 0: // 삽
                weaponDealay = 100f * GameManager.instance.playerData.atkSpeedMult; // 캐릭터별 무기 회전 속도 설정
                Batch();
                break;

            // 원거리 무기
            case 50: // 총
                weaponDealay = 0.5f * GameManager.instance.playerData.atkDelay; // 캐릭터별 무기 연사속도 설정
                break;

            case 51: // 대포
                weaponDealay = 3f * GameManager.instance.playerData.atkDelay; // 캐릭터별 무기 연사속도 설정
                break;

            case 52:
                weaponDealay = 3f * GameManager.instance.playerData.atkDelay; // 캐릭터별 무기 연사속도 설정
                break;

        }

       /* // 손 무기 세팅
        Hand hand = player.hands[(int)data.itemType];
        hand.spriter.sprite = data.hand;
        hand.gameObject.SetActive(true);*/

        // 기어(추가된 능력치) 적용
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void WeaonLevelUp(float damage, int count, int per)
    {
        this.damage = damage * Character.Damage; // 데미지 업데이트
        this.count += count; // 불릿 수 증가
        this.per += per;

        // 회전 무기는 다시 자연스럽게 추가시키기 위해서 재배치
        if (id == 0)
            Batch();

        // 기어 강화 적용
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    // 불릿 배치 함수 (회전 무기)
    private void Batch()
    {
        for (int index = 0; index < count; index++) // 불릿 수만큼 반복
        {
            Transform bullet;
            if (index < transform.childCount) // 자식 수보다 인덱스가 작으면
            {
                bullet = transform.GetChild(index); // 기존 자식 사용
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform; // 부모 설정
            }

            bullet.localPosition = Vector3.zero; // 로컬 위치 초기화
            bullet.localRotation = Quaternion.identity; // 로컬 회전 초기화

            Vector3 rotVec = Vector3.forward * 360 * index / count; // 불릿 회전 벡터 계산
            bullet.Rotate(rotVec); // 불릿 회전
            bullet.Translate(bullet.up * 1.5f, Space.World); // 지정된 거리만큼 이동
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // 불릿 초기화 (데미지 설정 및 관통 설정 -100은 무한 관통)
        }
    }

    #region 원거리 무기
    // 원거리 무기 순차 발사 함수 (count만큼 딜레이를 두고 순차적으로 발사)
    private IEnumerator AutoFireRangedWeapon()
    {
        if (player.scanner.nearestTarget == null)
            yield break;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;

        for (int i = 0; i < count; i++)
        {
            // 총알 발사
            Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.parent = transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            bullet.GetComponent<Bullet>().Init(damage, per, dir);

            // 발사 후 약간의 딜레이 추가
            yield return new WaitForSeconds(0.1f); // 총알 사이의 딜레이 설정 (0.1초, 필요에 따라 조정 가능)
        }
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
    private IEnumerator DirFireRangedWeapon()
    {
        Vector3 dir = new Vector3(player.lastInputVec.x, player.lastInputVec.y, 0);
        
        dir = dir.normalized;

        for (int i = 0; i < count; i++)
        {
            Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.parent = transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);

            bullet.GetComponent<Bullet>().Init(damage, per, dir);

            // 발사 후 약간의 딜레이 추가
            yield return new WaitForSeconds(0.1f); // 총알 사이의 딜레이 설정 (0.1초, 필요에 따라 조정 가능)
        }
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
    #endregion
}
