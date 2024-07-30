using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id; // 무기의 고유 ID
    public int prefabId; // 생성할 불릿의 프리팹 ID
    public float damage; // 무기 데미지
    public int count; // 불릿 수
    public float speed; // 무기의 회전 속도

    private void Start()
    {
        Init(); // 초기 설정 실행
    }

    private void Update()
    {
        switch (id) // 무기 ID에 따른 행동
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime); // 시간에 따라 무기 회전
                break;

            default:
                break;
        }

        if (Input.GetButtonDown("Jump")) // 점프 버튼(스페이스 바) 눌렀을 때
            LevelUp(20, 1); // 레벨 업 함수 실행
    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage; // 데미지 업데이트
        this.count += count; // 불릿 수 증가

        if (id == 0)
            Batch(); // 배치 함수 호출
    }

    public void Init() // 초기 설정 함수
    {
        switch (id)
        {
            case 0:
                speed = 150; // 속도 설정
                Batch(); // 배치 함수 호출
                break;

            default:
                break;
        }
    }

    private void Batch() // 불릿 배치 함수
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
                bullet = GameManager.instance.pool.Get(prefabId).transform; // 새 불릿 생성
                bullet.parent = transform; // 부모 설정
            }

            bullet.localPosition = Vector3.zero; // 로컬 위치 초기화
            bullet.localRotation = Quaternion.identity; // 로컬 회전 초기화

            Vector3 rotVec = Vector3.forward * 360 * index / count; // 불릿 회전 벡터 계산
            bullet.Rotate(rotVec); // 불릿 회전
            bullet.Translate(bullet.up * 1.5f, Space.World); // 지정된 거리만큼 이동
            bullet.GetComponent<Bullet>().Init(damage, -1); // 불릿 초기화 (데미지 설정 및 무한 관통)
        }
    }
}
