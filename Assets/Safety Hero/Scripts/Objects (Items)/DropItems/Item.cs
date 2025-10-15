using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected GameManager gm;
    private void Start()
    {
        gm = GameManager.instance;
    }
 
    // 아이템의 고유 효과. 자식 클래스에서 반드시 이 메서드를 구현해야 함
    public abstract void Use();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Use();
            collision.gameObject.GetComponent<PlayerInGame>().PlayerGenerateEffect();
            gameObject.SetActive(false);
        }
    }
}
