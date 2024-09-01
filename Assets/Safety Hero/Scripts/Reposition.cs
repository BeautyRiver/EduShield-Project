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
        if (!collision.CompareTag("Area"))
            return;

        Vector3 playerPos = GameManager.instance.player.transform.position; // 플레이어 포지션
        Vector3 myPos = transform.position; // 이 오브젝트의 포지션
        float dirX = playerPos.x - myPos.x;
        float dirY = playerPos.y - myPos.y;

        float diffX = Mathf.Abs(dirX);
        float diffY = Mathf.Abs(dirY);

        dirX = Mathf.Sign(dirX);
        dirY = Mathf.Sign(dirY);
        switch (transform.tag)
        {
            // 땅일때 (배경)
            case "Ground":
                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 80);
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 80);
                }
                else
                {
                    // 대각선으로 이동해야 할 경우, 어느 한 축으로만 이동하도록 수정
                    transform.Translate(new Vector3(dirX * 80, 0, 0));
                    transform.Translate(new Vector3(0, dirY * 80, 0));
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
}
