using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxReward : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D coll;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }
    private void OnEnable()
    {
        anim.SetBool("Open", false);
        coll.enabled = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("Open",true);
            Debug.Log("»óÀÚ ¿ÀÇÂ~");
        }
    }
    private void Dead()
    {
        gameObject.SetActive(false);
    }
}
