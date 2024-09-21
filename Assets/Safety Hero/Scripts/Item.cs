using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { Heal, Magnet }
    public ItemType itemType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Compare");
            switch (itemType)
            {
                case ItemType.Heal:                    
                    GameManager.instance.health = Mathf.Min(GameManager.instance.maxHealth, GameManager.instance.health + 15f);
                    break;
                case ItemType.Magnet:
                    StartCoroutine(GetMagnet());
                    break;         
            }
            gameObject.SetActive(false);
        }
    }
    IEnumerator GetMagnet()
    {
        float orignal = GameManager.instance.player.scanner.expCollectionRange;
        GameManager.instance.player.scanner.expCollectionRange = 999f;
        yield return null;
        yield return null;
        GameManager.instance.player.scanner.expCollectionRange = orignal;
    }
}
