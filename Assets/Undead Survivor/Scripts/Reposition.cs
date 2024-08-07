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
            
            switch (transform.tag)
            {
                // 땅일때 (배경)
                case "Ground":
                    // 거리 계산
                    float diffX = playerPos.x - myPos.x;
                    float diffY = playerPos.y - myPos.y;

                    float dirX = diffX < 0 ? -1 : 1;
                    float dirY = diffY < 0 ? -1 : 1;

                    diffX = Mathf.Abs(diffX);
                    diffY = Mathf.Abs(diffY);

                    if (diffX > diffY) // x축의 차이가 더 크면 x축으로 이동해서 배치하면 되므로
                    {
                        transform.Translate(Vector3.right * dirX * 60);
                    }
                    else if (diffX < diffY) // y축의 차이가 더 크면 y축으로 이동해서 배치하면 되므로
                    {
                        transform.Translate(Vector3.up * dirY * 60);
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
