using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id; // 무기의 고유 ID
    public int prefabId; // 생성할 불릿의 프리팹 ID
    public float damage; // 무기 데미지
    public int count; // 무기 수
    [Header("# 근접: 회전 속도 / 원거리: 발사 텀(초당 발사)")]
    public float speed; // 무기의 회전 속도
    private float timer_1;
    private float timer_2;
    private Player player;

    private void Awake()
    {
        player = GameManager.instance.player;
    }
    private void Update()
    {
        if (GameManager.instance.isLive)
        {
            switch (id) // 무기 ID에 따른 행동
            {
                case 0: // 회전무기 (삽)
                    transform.Rotate(Vector3.back * speed * Time.deltaTime); // 무기 회전
                    break;

                case 1: // 단발총                    
                    timer_1 += Time.deltaTime;

                    if (timer_1 > speed)
                    {
                        timer_1 = 0f;
                        FireWeaon_1();
                    }
                    break;
                case 2: // 대포
                    timer_2 += Time.deltaTime;
                    if (timer_2 > speed)
                    {
                        timer_2 = 0f;
                        FireWeaon_2();
                    }
                    break;

            }
        }
    }
    public void WeaonLevelUp(float damage, int count)
    {
        this.damage = damage * Character.Damage; // 데미지 업데이트
        this.count += count; // 불릿 수 증가

        // 회전 무기는 다시 자연스럽게 추가시키기 위해서 재배치
        if (id == 0)
            Batch();

        // 기어 강화 적용
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    // 초기 설정 함수
    public void Init(ItemData data)
    {
        // 기본 세팅
        gameObject.name = "Equip Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero; // 플레이어 안에서 위치 초기화

        // 속성 세팅
        id = data.itemId; // 아이디 설정
        damage = data.baseDamage * Character.Damage; // 데미지 설정
        count = data.baseCount + Character.Count; // 개수 or 관통 수 설정

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
            }
        }

        switch (id)
        {
            case 0: // 회전 무기
                speed = 150 * GameManager.instance.playerData.atkSpeedMult; // 캐릭터별 무기 회전 속도 설정
                Batch();
                break;

            case 1: // 단발 총
                speed = 0.5f * GameManager.instance.playerData.atkDelayMult; // 캐릭터별 무기 연사속도 설정
                break;

            case 2: // 대포
                speed = 1.5f * GameManager.instance.playerData.atkDelayMult; // 캐릭터별 무기 연사속도 설정
                break;
        }

       /* // 손 무기 세팅
        Hand hand = player.hands[(int)data.itemType];
        hand.spriter.sprite = data.hand;
        hand.gameObject.SetActive(true);*/

        // 기어(추가된 능력치) 적용
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }
    // 불릿 배치 함수
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
    private void FireWeaon_1()
    {
        // 플레이어의 스캐너가 가장 가까운 타겟을 찾지 못하면 함수 종료
        if (player.scanner.nearestTarget == null)
            return;     
        
        Vector3 targetPos = player.scanner.nearestTarget.position; // 가장 가까운 타겟의 위치        
        Vector3 dir = targetPos - transform.position; // 방향 계산        
        dir = dir.normalized;        

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.parent = transform;        
        bullet.position = transform.position;         
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir); // 불릿의 회전을 타겟 방향으로 설정
                                                                      // 
        bullet.GetComponent<Bullet>().Init(damage, count, dir); // 불릿을 초기화 (데미지와 관통 횟수 설정)        
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range); // 사운드 효과 재생

    }

    private void FireWeaon_2()
    {
        // 플레이어의 스캐너가 가장 가까운 타겟을 찾지 못하면 함수 종료
        if (player.scanner.nearestTarget == null)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position; // 가장 가까운 타겟의 위치        
        Vector3 dir = targetPos - transform.position; // 방향 계산        
        dir = dir.normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.parent = transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir); // 불릿의 회전을 타겟 방향으로 설정        

        bullet.GetComponent<Bullet>().Init(damage, count, dir); // 불릿을 초기화 (데미지와 관통 횟수 설정)        
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range); // 사운드 효과 재생
    }

}
