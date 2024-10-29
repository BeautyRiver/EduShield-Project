using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour
{
    public bool isMoving = false;
    public int exp;
    [SerializeField]
    private Sprite[] expImages;
    [SerializeField]
    private SpriteRenderer SpriteRenderer;

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (exp >= 5)
            SpriteRenderer.sprite = expImages[2];
        else if (exp >= 3)
            SpriteRenderer.sprite = expImages[1];
        else if (exp >= 1)
            SpriteRenderer.sprite = expImages[0];

        if (!GameManager.instance.isLive)
            return;

        if (collision.CompareTag("Player"))
        {
            MasterAudio.PlaySound("Coin"); // »ç¿îµå
            GameManager.instance.GetExp(exp);
            isMoving = false;
            gameObject.SetActive(false);
        }
    }
}
