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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Use(); // 아이템 고유의 효과를 실행
            gm.GenerateEffect(0, gm.player.transform); // 공통 효과 실행
            gameObject.SetActive(false); // 아이템 비활성화 (공통)
        }
    }

    // 아이템의 고유 효과. 자식 클래스에서 반드시 이 메서드를 구현해야 함
    protected abstract void Use();
}
