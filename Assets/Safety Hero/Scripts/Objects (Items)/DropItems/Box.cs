using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Box : MonoBehaviour, IDamageable
{
    public float health;
    public float maxHealth;

    public LootTable lootTable;

    private Animator anim;
    private BoxCollider2D coll;

    private PoolManager poolManager;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        poolManager = PoolManager.instance;
    }

    private void OnEnable()
    {
        health = maxHealth;
        anim.SetBool("Dead", false);
        coll.enabled = true;
    }

    public void DamagedLogic(Collider2D collision, float damage)
    {
        if (collision.CompareTag("Enemy"))
            return;

        health -= damage;
        anim.SetTrigger("Hit"); // 맞는 애니메이션 재생                                    
        MasterAudio.PlaySound("Hit"); // 사운드 재생

        if (health <= 0) // 체력 0 이하 사망
        {
            GameObject itemPrefabToDrop = lootTable.GetRandomItem();

            // 만약 뽑힌 아이템이 있다면, 생성한다.
            if (itemPrefabToDrop != null)
            {
                GameObject itemObj = poolManager.Get(itemPrefabToDrop);
                itemObj.transform.position = transform.position;
            }

            coll.enabled = false;
            anim.SetBool("Dead", true);

            if (GameManager.instance.isGameActive)
                MasterAudio.PlaySound("Dead");
        }
    }

    private void Dead()
    {
        gameObject.SetActive(false);
    }

    
}



