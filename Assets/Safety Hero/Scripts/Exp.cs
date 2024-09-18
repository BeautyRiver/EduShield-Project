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
            GameManager.instance.GetExp(1);
            isMoving = false;
            gameObject.SetActive(false);
        }
    }
}
