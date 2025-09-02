using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Box : MonoBehaviour
{
    public float health;
    public float maxHealth;
    private Animator anim;
    private BoxCollider2D coll;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        health = maxHealth;
        anim.SetBool("Dead", false);
        coll.enabled = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            health -= 1;
            anim.SetTrigger("Hit"); // 맞는 애니메이션 재생                                    
            MasterAudio.PlaySound("Hit"); // 사운드 재생

            if (health <= 0) // 체력 0 이하 사망
            {
                int selectIdx = (Random.Range(0, 10) >= 9) ? 1 : 2; // (1 = 힐 / 2 = 자석)                
                GameObject itemObj = GameManager.instance.poolManager.Get(PoolType.Item, selectIdx); // 아이템 드랍시키기
                itemObj.transform.position = transform.position;
                coll.enabled = false; // 콜라이더 끄기

                anim.SetBool("Dead", true);

                if (GameManager.instance.isGameActive)
                    MasterAudio.PlaySound("Dead");
            }
        }
        else
            return;
    }

    private void Dead()
    {
        gameObject.SetActive(false);
    }

  

}



