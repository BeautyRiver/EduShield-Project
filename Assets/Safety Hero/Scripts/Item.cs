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
                    gm.health = Mathf.Min(gm.maxHealth, gm.health + 15f);
                    MasterAudio.PlaySound("Heal");

                    gameObject.SetActive(false);
                    break;
                case ItemType.Magnet:
                    MasterAudio.PlaySound("Magnet");
                    StartCoroutine(GetMagnet());
                    break;
            }
            gm.GenerateEffect(1, gm.player.transform);
        }
    }

    IEnumerator GetMagnet()
    {
        float orignal = gm.player.scanner.expCollectionRange;
        gm.player.scanner.expCollectionRange = 999f;
        yield return new WaitForSeconds(0.1f);
        gm.player.scanner.expCollectionRange = orignal;
        gameObject.SetActive(false);
    }
}
