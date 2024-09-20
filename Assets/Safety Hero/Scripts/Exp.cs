using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour
{
    public bool isMoving = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GameManager.instance.isLive)
            return;

        if (collision.CompareTag("Player"))
        {            
            MasterAudio.PlaySound("Coin"); // »ç¿îµå
            GameManager.instance.GetExp(1);
            isMoving = false;
            gameObject.SetActive(false);
        }
    }
}
