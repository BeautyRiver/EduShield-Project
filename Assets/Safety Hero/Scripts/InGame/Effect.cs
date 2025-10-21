using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public enum EffectType { EnemyEffect, PlayerEffect }
    public EffectType effectType;
    private void OnEnable()
    {
        if (effectType == EffectType.EnemyEffect)
        {
            gameObject.transform.DOScale(new Vector2(1.5f, 1.5f), 0.3f).OnComplete(() =>
            {
                gameObject.transform.DOScale(Vector2.zero, 0.3f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            });
        }
    }

    private void Dead()
    {
        gameObject.SetActive(false);
    }
}
