using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { Heal, Magnet }
    public ItemType itemType;
    public GameManager gm;
    private void Awake()
    {
        gm = GameManager.instance;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Compare");
            switch (itemType)
            {
                case ItemType.Heal:
                    gm.player.health = Mathf.Min(gm.player.maxHealth, gm.player.health + 15f);
                    MasterAudio.PlaySound("Heal");

                    gameObject.SetActive(false);
                    break;
                case ItemType.Magnet:
                    MasterAudio.PlaySound("Magnet");
                    StartCoroutine(GetMagnet());
                    break;
            }
            gm.GenerateEffect(0, gm.player.transform);
        }
    }

    IEnumerator GetMagnet()
    {
        yield return null;
    }
}
