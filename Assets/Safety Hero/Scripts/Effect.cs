using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    private void OnEnable()
    {
        gameObject.transform.DOScale(new Vector2(1.5f,1.5f), 0.3f).OnComplete(() =>
        {
            gameObject.transform.DOScale(Vector2.zero, 0.3f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        });

    }
}
