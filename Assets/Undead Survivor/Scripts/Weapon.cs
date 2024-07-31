using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id; // 무기의 고유 ID
    public int prefabId; // 생성할 불릿의 프리팹 ID
    public float damage; // 무기 데미지
    public int count; // 불릿 수
    [Header("# 근접: 회전 속도 / 원거리: 발사 텀(초당 발사)")]
    public float speed; // 무기의 회전 속도

    private float timer;
    private Player player;

    private void Awake()
    {
        player = GameManager.instance.player;
    }
    private void Update()
    {
        switch (id) // 무기 ID에 따른 행동
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime); // 무기 회전
                break;

            default:
                timer += Time.deltaTime;

                if (timer > speed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
        }

        // Test Code...
        if (Input.GetButtonDown("Jump"))
            LevelUp(20, 1); 
    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage; // 데미지 업데이트
        this.count += count; // 불릿 수 증가

        if (id == 0)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    // 초기 설정 함수
    public void Init(ItemData data)
    {
        // 기본 세팅
        gameObject.name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero; // 플레이어 안에서 위치 초기화

        // 속성 세팅
        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
            }
        }

        switch (id)
        {
            case 0:
                speed = 150; // 속도 설정
                Batch();
                break;

            default:
                speed = 0.5f; // 연사 속도 (초당)
                break;
        }

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
            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero); // 불릿 초기화 (데미지 설정 및 관통 설정 -1은 무한 관통)
        }
    }
    private void Fire()
    {
        if (player.scanner.nearestTarget == null)
            return;
        
        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.parent = transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir); // 불릿 초기화 (데미지 설정 및 관통 설정)

    }

}
