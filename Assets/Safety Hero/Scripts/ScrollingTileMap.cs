using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTileMap : MonoBehaviour
{
    public Transform player;  // 플레이어의 위치
    public float tileSize;    // 타일맵 하나의 크기 (타일맵이 정사각형이라고 가정)
    public Transform[] tilemaps;  // A, B, C, D 타일맵

    private Vector3 lastPlayerPosition;
    public Vector3 debugPos;

    void Start()
    {
        lastPlayerPosition = player.position;  // 시작할 때 플레이어 위치 저장
    }

    void Update()
    {
        Vector3 playerMovement = player.position - lastPlayerPosition;
        debugPos = playerMovement;
        if (Mathf.Abs(playerMovement.x) > 10 || playerMovement.magnitude > tileSize / 2)  // 플레이어가 타일맵 크기만큼 이동했을 때
        {
            RepositionTiles(playerMovement);
            lastPlayerPosition = player.position;
        }
    }

    void RepositionTiles(Vector3 movement)
    {
        foreach (Transform tilemap in tilemaps)
        {
            if (movement.x > 0 && tilemap.position.x < player.position.x - tileSize)
            {
                tilemap.position += new Vector3(tileSize * 2, 0, 0);  // 오른쪽으로 재배치
            }
            else if (movement.x < 0 && tilemap.position.x > player.position.x + tileSize)
            {
                tilemap.position -= new Vector3(tileSize * 2, 0, 0);  // 왼쪽으로 재배치
            }
            if (movement.y > 0 && tilemap.position.y < player.position.y - tileSize)
            {
                tilemap.position += new Vector3(0, tileSize , 0);  // 위로 재배치
            }
            else if (movement.y < 0 && tilemap.position.y > player.position.y + tileSize)
            {
                tilemap.position -= new Vector3(0, tileSize , 0);  // 아래로 재배치
            }
        }
    }
}
