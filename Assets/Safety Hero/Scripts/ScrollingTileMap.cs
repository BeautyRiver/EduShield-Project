using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTileMap : MonoBehaviour
{
    private Collider2D coll;
    private Player player;
    public float tileMapSize = 40f; // 타일맵 이동 거리
    public float checkInterval = 0.5f; // 검사 간격 (초)

    private float nextCheckTime = 0f; // 다음 검사 시간
    private Vector3 previousPlayerPos; // 이전 플레이어 위치

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        player = GameManager.instance.player;
        previousPlayerPos = player.transform.position;
    }

    private void FixedUpdate()
    {
        // 현재 시간이 다음 검사 시간을 넘었는지 확인
        if (Time.time >= nextCheckTime)
        {
            Debug.Log("검사중");
            Vector3 playerPos = player.transform.position; // 플레이어 위치
            Vector3 myPos = transform.position; // 현재 오브젝트 위치
            float dirX = playerPos.x - myPos.x;
            float dirY = playerPos.y - myPos.y;

            float diffX = Mathf.Abs(dirX);
            float diffY = Mathf.Abs(dirY);

            dirX = Mathf.Sign(dirX);
            dirY = Mathf.Sign(dirY);

            if (diffX > tileMapSize * 1.5f || diffY > tileMapSize * 2)
            {
                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * tileMapSize * 3);
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * tileMapSize * 3);
                }
                else
                {
                    transform.Translate(new Vector3(dirX, dirY, 0) * tileMapSize * 3);
                }
            }

            previousPlayerPos = playerPos; // 플레이어 위치 업데이트
            nextCheckTime = Time.time + checkInterval; // 다음 검사 시간 설정
        }
    }
}
