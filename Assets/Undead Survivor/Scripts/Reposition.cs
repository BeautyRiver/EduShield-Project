using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    private Collider2D coll;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();    
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Area"))
        {
            Vector3 playerPos = GameManager.instance.player.transform.position; // 플레이어 포지션
            Vector3 myPos = transform.position; // 이 오브젝트의 포지션

            // 거리 계산
            float distX = playerPos.x - myPos.x;
            float distY= playerPos.y - myPos.y;

            float dirX = distX > 0 ? 1 : -1;
            float dirY = distY > 0 ? 1 : -1;

            distX = Mathf.Abs(distX);
            distY = Mathf.Abs(distY);
            switch (transform.tag)
            {
                
                // 땅일때 (배경)
                case "Ground":
                    Debug.Log(transform.name + " 범위 벗어남");
                    if (distX > distY)
                    {
                        transform.Translate(Vector2.right * dirX * 40);
                    }
                    else if (distY > distX)
                    {
                        transform.Translate(Vector2.up * dirY * 40);
                    }
                    break;

                    // Enemy일때
                case "Enemy":
                    if (coll.enabled == true)
                    {
                        Vector3 dist = playerPos - myPos;
                        Vector3 ran = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
                        transform.Translate(ran + dist * 2);
                    }
                    break;
            }
        }
        else
            return;

    }
}
