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
        if (!collision.CompareTag("Area") || !coll.CompareTag("Ground"))
            return;

        Vector3 playerPos = GameManager.instance.player.transform.position; 
        Vector3 myPos = transform.position; 
        float dirX = playerPos.x - myPos.x;
        float dirY = playerPos.y - myPos.y;

        float diffX = Mathf.Abs(dirX);
        float diffY = Mathf.Abs(dirY);

        dirX = Mathf.Sign(dirX);
        dirY = Mathf.Sign(dirY);

        if (coll.enabled == true)
        {
            if (collision is IRepositionable repositionable)
            {
                Vector3 dist = playerPos - myPos;
                Vector3 ran = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
                transform.Translate(ran + dist * 2);
            }
            else
                collision.gameObject.SetActive(false);
        }   

    }
}