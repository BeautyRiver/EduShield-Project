using DarkTonic.MasterAudio;
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
                    MasterAudio.PlaySound("Heal");
                    GameObject healEffect = GameManager.instance.pool.Get(PoolManager.PoolType.Effect, 1); // »˙ ¿Ã∆Â∆Æ
                    healEffect.transform.parent = GameManager.instance.player.transform;
                    healEffect.transform.localPosition = Vector3.zero;
                    gameObject.SetActive(false);
                    break;
                case ItemType.Magnet:
                    MasterAudio.PlaySound("Magnet");
                    StartCoroutine(GetMagnet());
                    break;         
            }
        }
    }
    IEnumerator GetMagnet()
    {
        float orignal = GameManager.instance.player.scanner.expCollectionRange;
        GameManager.instance.player.scanner.expCollectionRange = 999f;
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.player.scanner.expCollectionRange = orignal;
        gameObject.SetActive(false);
    }
}
