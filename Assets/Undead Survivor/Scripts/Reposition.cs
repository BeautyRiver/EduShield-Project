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
            Vector3 playerPos = GameManager.instance.player.transform.position;
            Vector3 myPos = transform.position;

            // 거리 계산
            float diffX = Mathf.Abs(playerPos.x - myPos.x);
            float diffY = Mathf.Abs(playerPos.y - myPos.y);

            Vector3 playerDir = GameManager.instance.player.inputVec;
            float dirX = playerDir.x < 0 ? -1 : 1;
            float dirY = playerDir.y < 0 ? -1 : 1;

            switch (transform.tag)
            {
                case "Ground":
                    if (diffX > diffY) // x축의 차이가 더 크면 x축으로 이동해서 배치하면 되므로
                    {
                        transform.Translate(Vector3.right * dirX * 40);
                    }
                    else if (diffX < diffY) // y축의 차이가 더 크면 y축으로 이동해서 배치하면 되므로
                    {
                        transform.Translate(Vector3.up * dirY * 40);
                    }

                    break;
                case "Enemy":
                    if (coll.enabled)
                    {
                        transform.Translate(playerDir * 20 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));
                    }
                    break;
            }
        }
        else
            return;

    }
}
