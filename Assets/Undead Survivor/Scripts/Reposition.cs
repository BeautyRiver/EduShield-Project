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

    private void FixedUpdate()
    {
                
    }

    private void OnDrawGizmos()
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Area"))
        {
            Vector3 playerPos = GameManager.instance.player.transform.position;
            Vector3 myPos = transform.position;

            // 거리 계산
            float dirX = playerPos.x - myPos.x;
            float dirY= playerPos.y - myPos.y;

            float diffX = Mathf.Abs(dirX);
            float diffY = Mathf.Abs(dirY);

            dirX = dirX > 0 ? 1 : -1;
            dirY = dirY > 0 ? 1 : -1;



            switch (transform.tag)
            {
                // 땅일때 (배경)
                case "Ground":                   

                    if (Mathf.Abs(diffX - diffY) <= 0.1f) 
                    {
                        transform.Translate(Vector3.up * dirY * 40);
                        transform.Translate(Vector3.right * dirX * 40);
                    }
                    else if (diffX > diffY)
                    {
                        transform.Translate(Vector3.right * dirX * 40);
                    }

                    else if (diffX < diffY) 
                    {
                        transform.Translate(Vector3.up * dirY * 40);
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
